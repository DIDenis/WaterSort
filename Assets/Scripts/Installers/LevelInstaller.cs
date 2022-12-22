using UnityEngine;
using Zenject;
using System.Collections.Generic;

public class LevelInstaller : MonoInstaller
{
    [SerializeField] SpriteRenderer spillingWaterPrefab;
    [SerializeField] List<Color> colors;

    public override void InstallBindings()
    {
        Container.Bind<Solver>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<SpriteRenderer>().FromInstance(spillingWaterPrefab);
        
        Container.Bind<RandomSorter>().AsSingle().NonLazy();
        Container.Bind<Flask>().FromComponentsInHierarchy().AsTransient();
        Container.Bind<List<Color>>().FromInstance(colors);
    }
}