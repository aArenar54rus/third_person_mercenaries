using UnityEngine;


namespace Arenar.Character
{
    public abstract class AIState : IAIState
    {
        protected ICharacterEntity character;
        protected AiStateMachineController aiStateMachineController;
        
        
        public Vector3 MoveDirection { get; protected set; }
        public Vector3 RotationDirection { get; protected set; }
        public bool IsStateReady { get; protected set; }
        
        
        public void SetupAiStateMachineBehaviour(AiStateMachineController aiStateMachineController) =>
            this.aiStateMachineController = aiStateMachineController;

        public virtual void Initialize(ICharacterEntity character) =>
            this.character = character;

        public abstract void DeInitialize();
        
        public abstract void OnStateBegin();

        public abstract void OnStateSyncUpdate();

        public abstract void OnStateAsyncUpdate();

        public abstract void OnStateEnd();
    }
}