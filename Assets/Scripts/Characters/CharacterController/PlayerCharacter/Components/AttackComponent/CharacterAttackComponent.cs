using Arenar.Items;
using System;
using Arenar.Services.InventoryService;
using Arenar.Services.SaveAndLoad;
using DG.Tweening;
using TakeTop.PreferenceSystem;
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
        private EffectsSpawner _effectsSpawner;
        
        private ICharacterInputComponent characterInputComponent;
        private ICharacterAimComponent characterAimComponent;
        private IInventoryComponent inventoryComponent;
        
        private ICharacterAnimationComponent<CharacterAnimationComponent.Animation, CharacterAnimationComponent.AnimationValue> characterAnimationComponent;
        
        private CharacterAimAnimationDataStorage characterAimAnimationData;
        private CharacterAnimatorDataStorage characterAnimatorData;
        
        private int _equippedWeaponIndex = 0;
        private bool _lockAction = false;

        private Tween _progressActionTween;
        
        
        public bool IsFirearmWeaponEquipped => InventoryComponent.EquippedFirearmWeapons != null;


        private ICharacterInputComponent CharacterInputComponent
        {
            get
            {
                if (characterInputComponent == null)
                    character.TryGetCharacterComponent<ICharacterInputComponent>(out characterInputComponent);
                return characterInputComponent;
            }
        }

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
        
        private bool CanMakeDistanceAttack =>
            IsFirearmWeaponEquipped && CharacterInputComponent.AttackAction && !InventoryComponent.CurrentActiveWeapon.IsShootLock;

        private bool IsReload =>
            CharacterInputComponent.ReloadAction;
        
        
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
            character.TryGetCharacterComponent<ICharacterInputComponent>(out characterInputComponent);

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
            tickableManager.Add(this);
            _lockAction = false;
        }

        public void OnDeactivate()
        {
            characterAnimatorData.AnimationReactionsTriggerController.onCompleteAction -= CompleteAction;
            tickableManager.Remove(this);
        }

        public void Tick()
        {
            if (InventoryComponent == null || InventoryComponent.CurrentActiveWeapon == null)
                return;
            
            PaintLaserBeam();
            
            if (CharacterInputComponent == null || _lockAction)
                return;

            if (IsReload && InventoryComponent.CurrentActiveWeapon.ClipSize < InventoryComponent.CurrentActiveWeapon.ClipSizeMax)
            {
                Reload();
                return;
            }

            if (!CharacterAimComponent.IsAim)
            {
                TryMakeMeleeAttack();
            }
            else
            {
                if (!CanMakeDistanceAttack)
                    return;
                
                if (InventoryComponent.CurrentActiveWeapon.ClipSize == 0)
                {
                    Reload();
                    
                    return;
                }
                    
                Vector3 direction = characterAimAnimationData.BodyPistolAimPointObject.position
                    - InventoryComponent.CurrentActiveWeapon.GunMuzzleTransform.position;
                direction = direction.normalized;
                
                inventoryComponent.CurrentActiveWeapon.MakeShot(direction, false);
                onUpdateWeaponClipSize?.Invoke(InventoryComponent.CurrentActiveWeapon.ClipSize, InventoryComponent.CurrentActiveWeapon.ClipSizeMax);
                characterAnimationComponent.PlayAnimation(CharacterAnimationComponent.Animation.Shoot);
                
                _lockAction = true;
            }
        }

        public void CompleteAction()
        {
            _lockAction = false;
        }

        private void Reload()
        {
            _lockAction = true;

            InventoryComponent.CurrentActiveWeapon.SetAimStatus(false);
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
                        InventoryComponent.CurrentActiveWeapon.ReloadSpeed,
                        InventoryComponent.CurrentActiveWeapon.ReloadSpeed)
                    .OnUpdate(() =>
                    {
                        onReloadProgress?.Invoke(progress,
                            InventoryComponent.CurrentActiveWeapon.ReloadSpeed);
                    }).OnComplete(() =>
                    {
                        onReloadEnd?.Invoke();
                        InventoryComponent.CurrentActiveWeapon.ReloadClip();
                        onUpdateWeaponClipSize?.Invoke(InventoryComponent.CurrentActiveWeapon.ClipSize,
                            InventoryComponent.CurrentActiveWeapon.ClipSizeMax);

                        if (inventoryComponent.CurrentActiveWeapon.ClipComponent is FirearmWeaponClipComponent
                            || InventoryComponent.CurrentActiveWeapon.ClipSize >= InventoryComponent.CurrentActiveWeapon.ClipSizeMax)
                        {
                            InventoryComponent.CurrentActiveWeapon.SetAimStatus(true);
                            _lockAction = false;
                        }
                        else
                        {
                            Reload();
                        }
                    });
            }
        }

        private void TryMakeMeleeAttack()
        {
            
        }

        private void PaintLaserBeam()
        {
            if (CharacterInputComponent == null)
                return;
            
            if (!IsFirearmWeaponEquipped)
                return;

            if (!_lockAction && CharacterAimComponent.IsAim && CharacterAimComponent.AimProgress >= 1)
            {
                InventoryComponent.CurrentActiveWeapon.SetAimStatus(true);
                InventoryComponent.CurrentActiveWeapon.SetLaserPosition(characterAimAnimationData.BodyPistolAimPointObject.position);
            }
            else
            {
                InventoryComponent.CurrentActiveWeapon.SetAimStatus(false);
            }
        }
    }
}