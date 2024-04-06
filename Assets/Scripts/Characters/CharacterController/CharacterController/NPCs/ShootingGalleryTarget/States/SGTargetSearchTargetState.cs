using System.Collections;
using System.Collections.Generic;
using Arenar.Services.LevelsService;
using UnityEngine;
using Zenject;


namespace Arenar.Character
{
    public class SGTargetSearchTargetState : AIState
    {
        private ISearchTargetComponent _searchTargetComponent;
        private ICharacterEntity _playerCharacter;


        private ISearchTargetComponent SearchTargetComponent
        {
            get
            {
                if (_searchTargetComponent == null)
                    _character.TryGetCharacterComponent(out _searchTargetComponent);
                return _searchTargetComponent;
            }
        }
        
        
        [Inject]
        private void Construct(ICharacterEntity character,
            CharacterSpawnController characterSpawnController)
        {
            _character = character;
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
            if (SearchTargetComponent.CharacterEntityTarget == null)
            {
                if (_playerCharacter.TryGetCharacterComponent<ICharacterLiveComponent>(
                        out ICharacterLiveComponent characterLiveComponent)
                    && characterLiveComponent.IsAlive)
                {
                    SearchTargetComponent.AddAggression(1000, _playerCharacter);
                    _aiStateMachineController.SwitchState<SGTargetAttackState>();
                    return;
                }
            }
            else
            {
                _aiStateMachineController.SwitchState<SGTargetAttackState>();
                return;
            }
        }

        public override void OnStateAsyncUpdate() { }

        public override void OnStateEnd() { }
    }
}