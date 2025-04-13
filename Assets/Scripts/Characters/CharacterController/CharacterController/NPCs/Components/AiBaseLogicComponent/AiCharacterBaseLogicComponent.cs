using Zenject;

namespace Arenar.Character
{
    public class AiCharacterBaseLogicComponent : IAiCharacterBaseLogicComponent, IFixedTickable
    {
        private ICharacterEntity characterEntity;
        private AIState[] aiStates;
        
        private TickableManager tickableManager;
        private AiStateMachineController aiStateMachine;


        public bool IsControlBlocked { get; set; } = true;


        [Inject]
        public void Construct(
            ICharacterEntity characterEntity,
            TickableManager tickableManager,
            AIState[] aiStates)
        {
            this.aiStates = aiStates;
            this.characterEntity = characterEntity;
            this.tickableManager = tickableManager;
        }
        
        public void Initialize()
        {
            aiStateMachine = new AiStateMachineController(characterEntity, aiStates);
        }

        public void DeInitialize()
        {
            aiStateMachine = null;
        }

        public void OnActivate()
        {
            if (IsControlBlocked)
                tickableManager.AddFixed(this);
            aiStateMachine.Initialize();
            aiStateMachine.OnStart();
            IsControlBlocked = false;
        }

        public void OnDeactivate()
        {
            IsControlBlocked = true;
            tickableManager.RemoveFixed(this);
            aiStateMachine.DeInitialize();
        }

        public void SwitchState<T>() where T : IAIState 
        {
            aiStateMachine.SwitchState<T>();
        }

        public void SwitchStateAsync<T>() where T : IAIState
        {
            aiStateMachine.SwitchStateAsync<T>();
        }

        public T GetStateInstance<T>() where T : IAIState
        {
            return aiStateMachine.GetStateInstance<T>();
        }

        public void FixedTick()
        {
            if (!IsControlBlocked)
                aiStateMachine.OnFixedTick();
        }
    }
}