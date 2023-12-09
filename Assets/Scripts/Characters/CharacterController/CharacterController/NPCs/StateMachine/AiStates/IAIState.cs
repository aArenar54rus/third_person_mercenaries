using UnityEngine;


namespace Arenar.Character
{
    public interface IAIState
    {
        Vector3 MoveDirection { get; }
        Vector3 RotationDirection { get; }
        bool IsStateReady { get; }


        void Initialize(ICharacterEntity character);
        void DeInitialize();
        void SetupAiStateMachineBehaviour(AiStateMachineController aiStateMachineController);
        void OnStateBegin();
        void OnStateSyncUpdate();
        void OnStateAsyncUpdate();
        void OnStateEnd();
    }
}