using Zenject;

namespace Arenar.Character
{
    public class AiCharacterBaseLogicComponent : IAiCharacterBaseLogicComponent, IFixedTickable
    {
        private ICharacterEntity _characterEntity;
        private AIState[] _aiStates;
        
        private TickableManager _tickableManager;
        private AiStateMachineController _aiStateMachine;
        
        private IStunCharacterComponent _stunCharacterComponent;


        public bool IsAiEnabled { get; set; } = true;


        [Inject]
        public void Construct(
            ICharacterEntity characterEntity,
            TickableManager tickableManager,
            AIState[] aiStates)
        {
            _aiStates = aiStates;
            _characterEntity = characterEntity;
            _tickableManager = tickableManager;
        }
        
        public void Initialize()
        {
            _aiStateMachine = new AiStateMachineController(_characterEntity, _aiStates);
        }

        public void DeInitialize()
        {
            _aiStateMachine = null;
        }

        public void OnActivate()
        {
            _tickableManager.AddFixed(this);
            _aiStateMachine.Initialize();
            _aiStateMachine.OnStart();
            IsAiEnabled = true;
            
            _characterEntity.TryGetCharacterComponent<IStunCharacterComponent>(out _stunCharacterComponent);
        }

        public void OnDeactivate()
        {
            IsAiEnabled = false;
            _tickableManager.RemoveFixed(this);
            _aiStateMachine.DeInitialize();
        }

        public void SwitchState<T>() where T : IAIState 
        {
            _aiStateMachine.SwitchState<T>();
        }

        public void SwitchStateAsync<T>() where T : IAIState
        {
            _aiStateMachine.SwitchStateAsync<T>();
        }

        public T GetStateInstance<T>() where T : IAIState
        {
            return _aiStateMachine.GetStateInstance<T>();
        }

        public void FixedTick() {
            if (!IsAiEnabled)
                return;
            
            if (_stunCharacterComponent != null && _stunCharacterComponent.IsStunned)
                return;
            
            _aiStateMachine.OnFixedTick();
        }
    }
}