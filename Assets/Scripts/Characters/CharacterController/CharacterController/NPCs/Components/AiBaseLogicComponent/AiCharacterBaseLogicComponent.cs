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


        public bool IsControlBlocked { get; set; }


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
            _aiStateMachine.Initialize();
        }

        public void DeInitialize()
        {
            IsControlBlocked = true;
            _aiStateMachine.DeInitialize();
            _tickableManager.RemoveFixed(this);
        }

        public void OnStart()
        {
            _aiStateMachine.OnStart();
            IsControlBlocked = false;
            _tickableManager.AddFixed(this);
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