namespace Arenar.SpawnerFactory
{
    public interface ISpawnerFactoriesService
    {
        void Initialize();

        void DeInitialize();

        TFactoryProvider GetFactoryProvider<TFactoryProvider>()
            where TFactoryProvider : IFactoryProvider<ISpawnerElement>;
    }
}