using DG.Tweening;
using System.Collections.Generic;
using Zenject;

namespace Arenar.Character
{
    public class BodyPartStunCharacterComponent : IStunCharacterComponent {
        private ICharacterEntity _character;
        private EnemyCharacterParameters _enemyCharacterParameters;
        
        private ICharacterAnimationComponent<CharacterAnimationComponent.Animation,
                CharacterAnimationComponent.AnimationValue> _characterAnimationComponent;

        private Dictionary<ECharacterDamageContainerBodyType, StunHealthData> _bodyPartsHealths = new();

        private Tween _tween;

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
            OnStunAnimationComplete();
            _tween?.Kill(false);
        }

        public void AddStunPoints(DamageData damageData) {
            if (IsStunned)
                return;
            
            if (!_bodyPartsHealths.ContainsKey(damageData.BodyPart))
                return;

            _bodyPartsHealths[damageData.BodyPart].StunHealth -= damageData.AddedStunPoint;
            if (_bodyPartsHealths[damageData.BodyPart].StunHealth <= 0) {
                MakeStun(damageData.BodyPart);
            }
        }
        
        private void MakeStun(ECharacterDamageContainerBodyType bodyType) {
            _characterAnimationComponent.SetAnimatorValue(CharacterAnimationComponent.AnimationValue.StunIndex, (int)bodyType);
            _characterAnimationComponent.SetAnimatorValue(CharacterAnimationComponent.AnimationValue.StunStart, 1);

            _tween = DOVirtual.DelayedCall(_enemyCharacterParameters.StunTime, OnStunAnimationComplete);
            IsStunned = true;
        }
        
        private void OnStunAnimationComplete() {
            _characterAnimationComponent.SetAnimatorValue(CharacterAnimationComponent.AnimationValue.StunIndex, 0);
            _characterAnimationComponent.SetAnimatorValue(CharacterAnimationComponent.AnimationValue.StunStart, 0);
            IsStunned = false;
        }
    }
}