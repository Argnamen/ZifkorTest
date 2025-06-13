using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class DogsController : IInitializable, IDisposable
{
    private const string BreedsApiUrl = "https://dogapi.dog/api/v2/breeds";

    private readonly RequestQueueSystem _requestQueue;
    private readonly DogsView _dogsView;
    private CancellationTokenSource _cts;

    [Inject]
    public DogsController(
        RequestQueueSystem requestQueue,
        DogsView dogsView)
    {
        _requestQueue = requestQueue;
        _dogsView = dogsView;
    }

    public void Initialize()
    {
        _cts = new CancellationTokenSource();
#pragma warning disable CS4014 // Предупреждение о "fire-and-forget"
        LoadInitialBreeds();
    }

    public void Dispose()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    private async Task LoadInitialBreeds()
    {
        try
        {
            await LoadBreeds(showLoading: true);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Breeds loading was canceled");
        }
        catch (Exception e)
        {
            Debug.LogError($"Initial breeds loading failed: {e.Message}");
            _dogsView.ShowError("Failed to load breeds. Please try again.");
        }
    }

    public async Task LoadBreeds(bool showLoading = false)
    {
        _dogsView.ClearBreeds();

        if (showLoading) _dogsView.ShowLoading(true);

        try
        {
            var json = await _requestQueue.EnqueueRequest<string>(
                BreedsApiUrl,
                _cts.Token
            );

            Debug.Log($"Breeds API response: {json}");

            var response = JsonUtility.FromJson<DogApiResponse>(json);

            if (response?.data == null || response.data.Count == 0)
            {
                Debug.LogError("No breeds data in response");
                _dogsView.ShowError("No breeds data available");
                return;
            }

            // Преобразуем данные API в нашу модель
            var breeds = response.data.Select(b => new DogBreed()
            {
                id = b.id,
                name = b.attributes.name,
                description = b.attributes.description,
                lifeSpan = $"{b.attributes.life.min}-{b.attributes.life.max} years",
                weight = $"Male: {b.attributes.male_weight.min}-{b.attributes.male_weight.max} kg, " +
                         $"Female: {b.attributes.female_weight.min}-{b.attributes.female_weight.max} kg",
                hypoallergenic = b.attributes.hypoallergenic
            }).ToList();

            _dogsView.DisplayBreeds(breeds);
        }
        catch (Exception e)
        {
            Debug.LogError($"Load breeds failed: {e.Message}");
            _dogsView.ShowError("Loading failed. Check connection.");
        }
        finally
        {
            if (showLoading) _dogsView.ShowLoading(false);
        }
    }

    public async Task LoadBreedDetails(string breedId)
    {
        if (string.IsNullOrEmpty(breedId))
        {
            Debug.LogError("Invalid breed ID");
            return;
        }

        _dogsView.ShowLoading(true);

        try
        {
            var url = $"{BreedsApiUrl}/{breedId}";
            var json = await _requestQueue.EnqueueRequest<string>(url, _cts.Token);

            Debug.Log($"Breed details response: {json}");

            var response = JsonUtility.FromJson<DogBreedDetailsResponse>(json);

            if (response?.data == null)
            {
                Debug.LogError("Invalid breed details format");
                _dogsView.ShowError("Couldn't load breed details");
                return;
            }

            _dogsView.ShowBreedDetails(response.data);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Breed details loading canceled");
        }
        catch (Exception e)
        {
            Debug.LogError($"Load breed details failed: {e.Message}");
            _dogsView.ShowError("Failed to load details");
        }
        finally
        {
            _dogsView.ShowLoading(false);
        }
    }

    // Модели данных
    [Serializable]
    private class DogBreedsResponse { public List<DogBreed> data; }

    [Serializable]
    private class DogBreedDetailsResponse { public DogBreed data; }
}