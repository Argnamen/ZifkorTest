using System.ComponentModel;
using UnityEngine;
using Zenject;

public class CoreInstaller : MonoInstaller
{
    [SerializeField] private WeatherView _weatherView;
    [SerializeField] private DogsView _dogsView;
    [SerializeField] private BreedPopup _breedPopup;

    public override void InstallBindings()
    {
        // Фабрика запросов
        Container.Bind<IRequestFactory>().To<RequestFactory>().AsSingle();

        // Очередь запросов
        Container.BindInterfacesAndSelfTo<RequestQueueSystem>().AsSingle();

        // UI
        Container.BindInstance(_weatherView).AsSingle();
        Container.BindInstance(_dogsView).AsSingle();
        Container.BindInstance(_breedPopup).AsSingle();

        // Контроллеры
        Container.BindInterfacesAndSelfTo<WeatherController>().AsSingle();
        Container.BindInterfacesAndSelfTo<DogsController>().AsSingle();
    }
}
