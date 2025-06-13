using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class WeatherController : IInitializable, IDisposable
{
    private readonly RequestQueueSystem _requestQueue;
    private readonly WeatherView _weatherView;
    private CancellationTokenSource _cts;
    private const string WeatherApiUrl = "https://api.weather.gov/gridpoints/TOP/32,81/forecast";

    public WeatherController(RequestQueueSystem requestQueue, WeatherView weatherView)
    {
        _requestQueue = requestQueue;
        _weatherView = weatherView;
    }

    public void Initialize()
    {
        StartWeatherUpdates();
    }

    public void Dispose()
    {
        StopWeatherUpdates();
    }

    private async void StartWeatherUpdates()
    {
        _cts = new CancellationTokenSource();

        _weatherView.ShowLoading(true);

        while (!_cts.IsCancellationRequested)
        {
            try
            {
                var json = await _requestQueue.EnqueueRequest<string>(WeatherApiUrl, _cts.Token);
                var weatherData = JsonUtility.FromJson<WeatherResponse>(json);

                // Проверка всех уровней вложенности
                if (weatherData?.properties?.periods == null || weatherData.properties.periods.Length == 0)
                {
                    Debug.LogError("No valid weather periods found in response");
                    return;
                }

                // Берем первый период (текущую погоду)
                var currentPeriod = weatherData.properties.periods[0];
                _weatherView.UpdateWeather(currentPeriod);
            }
            catch (TaskCanceledException)
            {
                return;
            }
            catch (Exception e)
            {
                Debug.LogError($"Weather update failed: {e.Message}");
            }

            _weatherView.ShowLoading(false);

            try
            {
                await Task.Delay(5000, _cts.Token);
            }
            catch (TaskCanceledException)
            {
                return;
            }
        }
    }

    public void StopWeatherUpdates()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }
}
