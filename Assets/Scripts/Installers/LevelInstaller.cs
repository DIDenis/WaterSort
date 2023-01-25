using UnityEngine;
using Zenject;

namespace WaterSort
{
    public class LevelInstaller : MonoInstaller
    {
        [SerializeField] SpriteRenderer spillingWaterPrefab;
        [SerializeField] RandomSorter.Settings sorterSettings;

        public override void InstallBindings()
        {
            Container.Bind<Solver>().FromComponentInHierarchy().AsSingle().NonLazy();
            Container.Bind<SpriteRenderer>().FromInstance(spillingWaterPrefab);
            
            Container.Bind<RandomSorter.Settings>().FromInstance(sorterSettings);
            Container.Bind<RandomSorter>().AsSingle().NonLazy();
            Container.Bind<Flask>().FromComponentsInHierarchy().AsTransient();
        }
    }
}