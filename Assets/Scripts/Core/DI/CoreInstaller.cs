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
        // ������� ��������
        Container.Bind<IRequestFactory>().To<RequestFactory>().AsSingle();

        // ������� ��������
        Container.BindInterfacesAndSelfTo<RequestQueueSystem>().AsSingle();

        // UI
        Container.BindInstance(_weatherView).AsSingle();
        Container.BindInstance(_dogsView).AsSingle();
        Container.BindInstance(_breedPopup).AsSingle();

        // �����������
        Container.BindInterfacesAndSelfTo<WeatherController>().AsSingle();
        Container.BindInterfacesAndSelfTo<DogsController>().AsSingle();
    }
}
