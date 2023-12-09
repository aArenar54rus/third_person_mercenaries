using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Arenar.Character
{
    public class SearchEnemyState : AIState
    {
        private ICharacterAnimationComponent<CharacterAnimationComponent.Animation, CharacterAnimationComponent.AnimationValue>
            characterAnimationComponent;

        private ICharacterLiveComponent characterLiveComponent;
        
        
        public override void Initialize(ICharacterEntity character)
        {
            base.Initialize(character);
            character.TryGetCharacterComponent(out characterAnimationComponent);
            character.TryGetCharacterComponent(out characterLiveComponent);
        }

        public override void DeInitialize()
        {
            characterLiveComponent.OnCharacterGetDamageBy -= OnCharacterGetDamageBy;
        }
        
        public override void OnStateSyncUpdate()
        {
            throw new System.NotImplementedException();
        }

        public override void OnStateAsyncUpdate()
        {
            throw new System.NotImplementedException();
        }
        
        public override void OnStateBegin()
        {
            characterLiveComponent.OnCharacterGetDamageBy += OnCharacterGetDamageBy;
        }

        public override void OnStateEnd()
        {
            characterLiveComponent.OnCharacterGetDamageBy -= OnCharacterGetDamageBy;
        }

        private void OnCharacterGetDamageBy(ICharacterEntity aggressor)
        {
            
        }
    }
}