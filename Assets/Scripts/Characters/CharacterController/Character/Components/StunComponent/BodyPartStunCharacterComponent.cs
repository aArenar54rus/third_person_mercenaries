using System.Collections.Generic;
using UnityEngine;
using Zenject;


namespace Arenar.Character
{
    public class BodyPartStunCharacterComponent : IStunCharacterComponent {
        private ICharacterEntity _character;
        private EnemyCharacterParameters _enemyCharacterParameters;
        
        private ICharacterAnimationComponent<CharacterAnimationComponent.Animation,
                CharacterAnimationComponent.AnimationValue> _characterAnimationComponent;

        private CharacterAnimationComponent.AnimationValue _lastPlayedStunAnimation;
        private Dictionary<ECharacterDamageContainerBodyType, StunHealthData> _bodyPartsHealths = new();


        public bool IsStunned { get; private set; } = false;


        [Inject]
        public void Construct(ICharacterEntity character,
                                              ICharacterDataStorage<EnemyCharacterDataStorage> enemyCharacterDataStorage) {
            _enemyCharacterParameters = enemyCharacterDataStorage.Data.EnemyCharacterParameters;
            _character = character;
        }
        
        public void Initialize()
        {
            if (_character.TryGetCharacterComponent<ICharacterAnimationComponent>(
                        out ICharacterAnimationComponent animationComponent))
            {
                if (animationComponent is CharacterAnimationComponent neededAnimationComponent)
                    _characterAnimationComponent = neededAnimationComponent;
            }
        }
        
        public void DeInitialize() {}

        public void OnActivate()
        {
            int healthMax = _enemyCharacterParameters.BaseHealth;
            foreach (var stunData in _enemyCharacterParameters.PartDatas) {
                _bodyPartsHealths.Add(stunData.Key,
                        new StunHealthData((int)(stunData.Value.StunScorePercent * healthMax)));
            }
        }
        
        public void OnDeactivate()
        {
            if (_lastPlayedStunAnimation != CharacterAnimationComponent.AnimationValue.None) {
                _characterAnimationComponent.onAnimationEvent -= OnStunAnimationComplete;
                _characterAnimationComponent.SetAnimationValue(_lastPlayedStunAnimation, 0);
            }
            
            IsStunned = false;
        }

        public void AddStunPoints(int stunPoints, ECharacterDamageContainerBodyType bodyType) {
            if (IsStunned)
                return;
            
            if (!_bodyPartsHealths.ContainsKey(bodyType))
                return;

            _bodyPartsHealths[bodyType].StunHealth -= stunPoints;
            if (_bodyPartsHealths[bodyType].StunHealth <= 0) {
                MakeStun(bodyType);
            }
        }
        
        private void MakeStun(ECharacterDamageContainerBodyType bodyType) {
            if (_lastPlayedStunAnimation != CharacterAnimationComponent.AnimationValue.None) {
                _characterAnimationComponent.onAnimationEvent -= OnStunAnimationComplete;
                _characterAnimationComponent.SetAnimationValue(_lastPlayedStunAnimation, 0);
                IsStunned = false;
                return;
            }

            switch (bodyType) {
                case ECharacterDamageContainerBodyType.Body:
                    _lastPlayedStunAnimation = CharacterAnimationComponent.AnimationValue.StunBody;
                    break;
                case ECharacterDamageContainerBodyType.Head:
                    _lastPlayedStunAnimation = CharacterAnimationComponent.AnimationValue.StunHead;
                    break;
                case ECharacterDamageContainerBodyType.HandLeft:
                    _lastPlayedStunAnimation = CharacterAnimationComponent.AnimationValue.StunHandLeft;
                    break;
                case ECharacterDamageContainerBodyType.HandRight:
                    _lastPlayedStunAnimation = CharacterAnimationComponent.AnimationValue.StunHandRight;
                    break;
                case ECharacterDamageContainerBodyType.LegLeft:
                    _lastPlayedStunAnimation = CharacterAnimationComponent.AnimationValue.StunLegLeft;
                    break;
                case ECharacterDamageContainerBodyType.LegRight:
                    _lastPlayedStunAnimation = CharacterAnimationComponent.AnimationValue.StunLegRight;
                    break;
                default:
                    Debug.Log($"Unknown body type {bodyType}. Check Stun character component.");
                    return;
            }
            
            _characterAnimationComponent.SetAnimationValue(_lastPlayedStunAnimation, 1);
            _characterAnimationComponent.onAnimationEvent += OnStunAnimationComplete;
            IsStunned = true;
        }
        
        private void OnStunAnimationComplete(string animationEventKey) {
            if (animationEventKey != AnimationEventKeys.COMPLETE_STUN)
                return;

            _characterAnimationComponent.SetAnimationValue(_lastPlayedStunAnimation, 0);
            _characterAnimationComponent.onAnimationEvent -= OnStunAnimationComplete;
            IsStunned = false;
        }
    }
}