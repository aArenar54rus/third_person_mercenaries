using Arenar.Services.LevelsService;
using Zenject;


namespace Arenar.Character
{
    public abstract class AiCharacterBaseLogicComponent : IAiCharacterBaseLogicComponent, IFixedTickable
    {
        private ICharacterEntity _characterEntity;
        private AIState[] _aiStates;
        private ILevelsService _levelsService;
        private TickableManager _tickableManager;
        private AiStateMachineController _aiStateMachine;


        public bool IsControlBlocked { get; set; } = true;


        [Inject]
        public void Construct(
            ILevelsService levelsService,
            ICharacterEntity characterEntity,
            TickableManager tickableManager,
            AIState[] aiStates)
        {
            _aiStates = aiStates;
            _characterEntity = characterEntity;
            _tickableManager = tickableManager;
            _levelsService = levelsService;
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
            if (IsControlBlocked)
                _tickableManager.AddFixed(this);
            _aiStateMachine.Initialize();
            _aiStateMachine.OnStart();
            IsControlBlocked = false;
        }

        public void OnDeactivate()
        {
            IsControlBlocked = true;
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

        public void FixedTick()
        {
            if (!IsControlBlocked)
                _aiStateMachine.OnFixedTick();
        }
    }
}