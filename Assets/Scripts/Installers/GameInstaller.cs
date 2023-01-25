using Zenject;

namespace WaterSort
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<Game>().FromComponentInHierarchy().AsSingle().NonLazy();
            Container.Bind<GameView>().FromComponentInHierarchy().AsSingle().NonLazy();
        }
    }
}