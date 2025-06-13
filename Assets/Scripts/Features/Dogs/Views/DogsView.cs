using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DogsView : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform _breedsContainer;
    [SerializeField] private GameObject _breedItemPrefab;
    [SerializeField] private GameObject _loadingIndicator;
    [SerializeField] private GameObject _errorPanel;
    [SerializeField] private TMP_Text _errorText;
    [SerializeField] private BreedPopup _breedPopup;
    [SerializeField] private ScrollRect _scrollRect;

    [Header("Settings")]
    [SerializeField] private int _maxVisibleBreeds = 10;
    [SerializeField] private float _scrollDelay = 0.1f;

    private List<Button> _activeBreedButtons = new();
    private bool _isInitialized;

    private void Awake()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        if (_isInitialized) return;

        _errorPanel.SetActive(false);
        _isInitialized = true;
    }

    public void ShowLoading(bool isLoading)
    {
        _loadingIndicator.SetActive(isLoading);

        // Блокируем кнопки на время загрузки
        foreach (var button in _activeBreedButtons)
        {
            button.interactable = !isLoading;
        }
    }

    public void DisplayBreeds(List<DogBreed> breeds)
    {
        ClearBreeds();

        if (breeds == null || breeds.Count == 0)
        {
            ShowError("No breeds data available");
            return;
        }

        if (_breedItemPrefab == null || _breedsContainer == null)
        {
            Debug.LogError("UI references not set!");
            return;
        }

        foreach (var breed in breeds.Take(_maxVisibleBreeds))
        {
            try
            {
                var item = Instantiate(_breedItemPrefab, _breedsContainer);

                // Безопасное получение компонентов
                var textFields = item.GetComponentsInChildren<TMP_Text>(true);
                if (textFields.Length < 2) // Минимум 2 текстовых поля
                {
                    Debug.LogError($"Breed item prefab requires at least 2 TextMeshPro components (found {textFields.Length})");
                    continue;
                }

                // Основная информация
                textFields[0].text = breed.name;
                textFields[1].text = $"Life: {breed.lifeSpan} | Weight: {breed.weight?.Split(',')?.FirstOrDefault() ?? "N/A"}";

                // Опциональное поле (гипоаллергенность)
                if (textFields.Length > 2)
                {
                    textFields[2].text = breed.hypoallergenic ? "Hypoallergenic" : "";
                    textFields[2].color = breed.hypoallergenic ? Color.green : Color.red;
                }

                var button = item.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.AddListener(() => ShowBreedDetails(breed));
                    _activeBreedButtons.Add(button);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to create breed item: {e.Message}");
            }
        }
    }

    public void ShowBreedDetails(DogBreed breed)
    {
        if (breed == null)
        {
            Debug.LogError("Breed data is null");
            return;
        }

        string fullDetails = $"{breed.description}\n\n" +
                           $"<b>Life Span:</b> {breed.lifeSpan}\n" +
                           $"<b>Weight:</b> {breed.weight}\n" +
                           $"<b>Hypoallergenic:</b> {(breed.hypoallergenic ? "Yes" : "No")}";

        _breedPopup.Show(
            title: breed.name,
            description: fullDetails
        );
    }

    public void ShowError(string message)
    {
        _errorText.text = message;
        _errorPanel.SetActive(true);
        Debug.LogError(message);
    }

    public void HideError()
    {
        _errorPanel.SetActive(false);
    }

    public void ClearBreeds()
    {
        foreach (Transform child in _breedsContainer)
        {
            Destroy(child.gameObject);
        }
        _activeBreedButtons.Clear();
    }

    private System.Collections.IEnumerator ScrollToTop()
    {
        yield return new WaitForSeconds(_scrollDelay);
        _scrollRect.normalizedPosition = new Vector2(0, 1);
    }

    // Загрузка иконки (если есть URL в данных)
    private System.Collections.IEnumerator LoadBreedIcon(string url, Image targetImage)
    {
        if (string.IsNullOrEmpty(url)) yield break;

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var texture = DownloadHandlerTexture.GetContent(request);
                targetImage.sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    Vector2.zero
                );
            }
        }
    }
}
