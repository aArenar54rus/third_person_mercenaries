using Arenar.Items;
using System;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Arenar.Character
{
    public class CharacterAttackComponent : ICharacterAttackComponent, ITickable
    {
        public event Action onReloadStart; 
        public event Action onReloadEnd; 
        public event Action<float, float> onReloadProgress; 
        public event Action<int, int> onUpdateWeaponClipSize;
        
        
        private ICharacterEntity character;
        private TickableManager tickableManager;
        private EffectsSpawner effectsSpawner;
        
        private ICharacterAimComponent characterAimComponent;
        private IInventoryComponent inventoryComponent;
        
        private ICharacterAnimationComponent<CharacterAnimationComponent.Animation, CharacterAnimationComponent.AnimationValue> characterAnimationComponent;
        
        private CharacterAimAnimationDataStorage characterAimAnimationData;
        private CharacterAnimatorDataStorage characterAnimatorData;
        
        private int _equippedWeaponIndex = 0;
        private bool _lockAction = false;

        private Tween _progressActionTween;


        public int CharacterDamage { get; set; } = 0;

        public bool HasProcess => _lockAction;
        
        private bool IsFirearmWeaponEquipped => InventoryComponent.EquippedFirearmWeapons != null;


        private ICharacterAimComponent CharacterAimComponent
        {
            get
            {
                if (characterAimComponent == null)
                    character.TryGetCharacterComponent<ICharacterAimComponent>(out characterAimComponent);
                return characterAimComponent;
            }
        }

        private IInventoryComponent InventoryComponent
        {
            get
            {
                if (inventoryComponent == null)
                    character.TryGetCharacterComponent<IInventoryComponent>(out inventoryComponent);
                return inventoryComponent;
            }
        }
        
        private bool CanMakeDistanceAttack => IsFirearmWeaponEquipped && !InventoryComponent.CurrentActiveFirearmWeapon.IsShootLock;
        
        
        [Inject]
        public void Construct(ICharacterEntity character,
                              TickableManager tickableManager,
                              ICharacterDataStorage<CharacterAimAnimationDataStorage> characterAimAnimationDataStorage,
                              ICharacterDataStorage<CharacterAnimatorDataStorage> characterAnimatorDataStorage)
        {
            this.character = character;
            this.tickableManager = tickableManager;

            characterAimAnimationData = characterAimAnimationDataStorage.Data;
            characterAnimatorData = characterAnimatorDataStorage.Data;
        }
        
        public void Initialize()
        {
            CharacterDamage = 0;
            _lockAction = false;

            if (character.TryGetCharacterComponent<ICharacterAnimationComponent>(out ICharacterAnimationComponent animationComponent))
            {
                if (animationComponent is CharacterAnimationComponent neededAnimationComponent)
                    characterAnimationComponent = neededAnimationComponent;
            }
        }

        public void DeInitialize()
        {

        }

        public void OnActivate()
        {
            characterAnimatorData.AnimationReactionsTriggerController.onCompleteAction += CompleteAction;
            characterAnimatorData.AnimationReactionsTriggerController.onAnimationEventTriggered += AnimationEventTriggeredHandler;
            tickableManager.Add(this);
            
            CharacterDamage = 0;
            _lockAction = false;
        }

        public void OnDeactivate()
        {
            characterAnimatorData.AnimationReactionsTriggerController.onCompleteAction -= CompleteAction;
            characterAnimatorData.AnimationReactionsTriggerController.onAnimationEventTriggered -= AnimationEventTriggeredHandler;
            tickableManager.Remove(this);
        }
        
        public void MakeReload()
        {
            if (_lockAction
                || InventoryComponent.CurrentActiveFirearmWeapon.ClipSize >= InventoryComponent.CurrentActiveFirearmWeapon.ClipSizeMax)
                return;
            
            _lockAction = true;

            InventoryComponent.CurrentActiveFirearmWeapon.SetAimStatus(false);
            if (characterAnimatorData.IsReloadByAnimation)
            {
                        
            }
            else
            {
                onReloadStart?.Invoke();
                onReloadProgress?.Invoke(0, 2.0f);
                float progress = 0;

                _progressActionTween = DOTween.To(
                        () => progress, 
                        x => progress = x, 
                        InventoryComponent.CurrentActiveFirearmWeapon.ReloadSpeed,
                        InventoryComponent.CurrentActiveFirearmWeapon.ReloadSpeed)
                    .OnUpdate(() =>
                    {
                        onReloadProgress?.Invoke(progress,
                            InventoryComponent.CurrentActiveFirearmWeapon.ReloadSpeed);
                    }).OnComplete(() =>
                    {
                        InventoryComponent.CurrentActiveFirearmWeapon.ReloadClip();
                        
                        onReloadEnd?.Invoke();
                        onUpdateWeaponClipSize?.Invoke(InventoryComponent.CurrentActiveFirearmWeapon.ClipSize,
                            InventoryComponent.CurrentActiveFirearmWeapon.ClipSizeMax);

                        if (inventoryComponent.CurrentActiveFirearmWeapon.ClipComponent is FirearmWeaponClipComponent
                            || InventoryComponent.CurrentActiveFirearmWeapon.ClipSize >= InventoryComponent.CurrentActiveFirearmWeapon.ClipSizeMax)
                        {
                            InventoryComponent.CurrentActiveFirearmWeapon.SetAimStatus(true);
                            _lockAction = false;
                        }
                        else
                        {
                            MakeReload();
                        }
                    });
            }
        }

        public void PlayAction()
        {
            if (_lockAction)
                return;
            
            if (!CharacterAimComponent.IsAim)
            {
                MakeMeleeAttack();
            }
            else
            {
                if (InventoryComponent.CurrentActiveFirearmWeapon == null)
                    return;
                
                if (!CanMakeDistanceAttack)
                    return;
                
                if (InventoryComponent.CurrentActiveFirearmWeapon.ClipSize == 0)
                {
                    MakeReload();
                    
                    return;
                }

                MakeFirearmAttack();
            }
        }

        public void Tick()
        {
            if (InventoryComponent == null || InventoryComponent.CurrentActiveFirearmWeapon == null)
                return;
            
            PaintLaserBeam();
        }

        public void CompleteAction()
        {
            _lockAction = false;
        }
        
        public void AnimationEventTriggeredHandler(string animationEvent)
        {
            if (animationEvent == AnimationEventKeys.COMPLETE_MELEE_ATTACK_ANIM_TRIGGER)
                _lockAction = false;
        }

        private void MakeMeleeAttack()
        {
            if (InventoryComponent.CurrentActiveMeleeWeapon == null)
            {
                return;
            }
            
            characterAnimationComponent.SetAnimationValue(CharacterAnimationComponent.AnimationValue.SwordAttack, 1);
            InventoryComponent.CurrentActiveMeleeWeapon.MakeMeleeAttack();
            _lockAction = true;
        }

        private void MakeFirearmAttack()
        {
            Vector3 direction = characterAimAnimationData.BodyPistolAimPointObject.position
                - InventoryComponent.CurrentActiveFirearmWeapon.GunMuzzleTransform.position;
            direction = direction.normalized;
                
            inventoryComponent.CurrentActiveFirearmWeapon.MakeShot(direction, CharacterDamage, false);
            onUpdateWeaponClipSize?.Invoke(InventoryComponent.CurrentActiveFirearmWeapon.ClipSize, InventoryComponent.CurrentActiveFirearmWeapon.ClipSizeMax);
            characterAnimationComponent.PlayAnimation(CharacterAnimationComponent.Animation.Shoot);
                
            _lockAction = true;
        }

        private void PaintLaserBeam()
        {
            if (!IsFirearmWeaponEquipped)
                return;

            if (!_lockAction && CharacterAimComponent.IsAim && CharacterAimComponent.AimProgress >= 1)
            {
                InventoryComponent.CurrentActiveFirearmWeapon.SetAimStatus(true);
                InventoryComponent.CurrentActiveFirearmWeapon.SetLaserPosition(characterAimAnimationData.BodyPistolAimPointObject.position);
            }
            else
            {
                InventoryComponent.CurrentActiveFirearmWeapon.SetAimStatus(false);
            }
        }
    }
}