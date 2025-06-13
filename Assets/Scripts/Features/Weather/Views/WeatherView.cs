using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WeatherView : MonoBehaviour
{
    [SerializeField] private Image _weatherIcon;
    [SerializeField] private TMP_Text _weatherText;
    [SerializeField] private GameObject _loadingIndicator;

    public void UpdateWeather(WeatherPeriod period)
    {
        if (period == null)
        {
            Debug.LogError("Received null weather period");
            return;
        }

        _weatherText.text = $"{period.name} - {period.temperature}°{period.temperatureUnit}";

        // Загрузка иконки (если нужно)
        StartCoroutine(LoadWeatherIcon(period.icon));
    }

    public void ShowLoading(bool isLoading)
    {
        _loadingIndicator.SetActive(isLoading);
    }

    private IEnumerator LoadWeatherIcon(string url)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                _weatherIcon.sprite = Sprite.Create(texture,
                    new Rect(0, 0, texture.width, texture.height),
                    Vector2.zero);
            }
        }
    }
}
