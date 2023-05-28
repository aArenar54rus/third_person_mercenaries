namespace CatSimulator.Character
{
    public interface ICharacterComponent
    {
        void Initialize();

        void DeInitialize();

        void OnStart();
    }
}