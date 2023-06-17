namespace Arenar.Character
{
    public interface ICharacterComponent
    {
        void Initialize();

        void DeInitialize();

        void OnStart();
    }
}