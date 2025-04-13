using Zenject;


namespace Arenar.Character
{
    public class SGTargetSearchTargetState : AIState
    {
        private ICharacterAggressionComponent _aggressionComponent;
        private ICharacterEntity _playerCharacter;


        private ICharacterAggressionComponent AggressionComponent
        {
            get
            {
                if (_aggressionComponent == null)
                    character.TryGetCharacterComponent(out _aggressionComponent);
                return _aggressionComponent;
            }
        }
        
        
        [Inject]
        private void Construct(ICharacterEntity character,
                               CharacterSpawnController characterSpawnController)
        {
            base.character = character;
            _playerCharacter = characterSpawnController.PlayerCharacter;
        }
        
        public override void DeInitialize()
        {
            
        }

        public override void OnStateBegin()
        {
            
        }

        public override void OnStateSyncUpdate()
        {
            if (AggressionComponent.MaxAggressionTarget == null)
            {
                if (_playerCharacter.TryGetCharacterComponent<ICharacterLiveComponent>(
                        out ICharacterLiveComponent characterLiveComponent)
                    && characterLiveComponent.IsAlive)
                {
                    AggressionComponent.AddAggressionScore(_playerCharacter, 1000);
                    aiStateMachineController.SwitchState<SGTargetAttackState>();
                    return;
                }
            }
            else
            {
                aiStateMachineController.SwitchState<SGTargetAttackState>();
                return;
            }
        }

        public override void OnStateAsyncUpdate() { }

        public override void OnStateEnd() { }
    }
}