namespace Arenar.Character
{
    public interface IAiCharacterBaseLogicComponent : ICharacterComponent
    {
        bool IsControlBlocked { get; set; }
        
        
        void SwitchState<T>() where T : IAIState;
        void SwitchStateAsync<T>() where T : IAIState;

        T GetStateInstance<T>() where T : IAIState;
    }
}