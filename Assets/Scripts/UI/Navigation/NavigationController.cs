using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class NavigationController : MonoBehaviour
{
    [SerializeField] private Button _weatherTabButton;
    [SerializeField] private Button _dogsTabButton;
    [SerializeField] private GameObject _weatherTab;
    [SerializeField] private GameObject _dogsTab;
    [SerializeField] private GameObject _breedTab;

    [Inject] private DogsController _dogsController;
    [Inject] private WeatherController _weatherController;

    private void Start()
    {
        _weatherTabButton.onClick.AddListener(() => SetTabActive(_weatherTab));
        _dogsTabButton.onClick.AddListener(() => SetTabActive(_dogsTab));

        SetTabActive(_weatherTab);
    }

    private void SetTabActive(GameObject activeTab)
    {
        _weatherTab.SetActive(activeTab == _weatherTab);
        _dogsTab.SetActive(activeTab == _dogsTab);

        _breedTab.SetActive(false);

        if (activeTab == _weatherTab)
        {
            _weatherController.Initialize();
            _dogsController.Dispose();
        }
        else if (activeTab == _dogsTab)
        {
            _dogsController.Initialize();
            _weatherController.StopWeatherUpdates();
        }
    }
}
