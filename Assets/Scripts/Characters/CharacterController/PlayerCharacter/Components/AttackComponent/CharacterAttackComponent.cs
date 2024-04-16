using System;
using Arenar.Services.InventoryService;
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
        private EffectsSpawner _effectsSpawner;
        private IInventoryService inventoryService;
        
        private ICharacterInputComponent characterInputComponent;
        private ICharacterAimComponent characterAimComponent;
        private FirearmWeaponFactory firearmWeaponFactory;
        private ICharacterAnimationComponent<CharacterAnimationComponent.Animation, CharacterAnimationComponent.AnimationValue> characterAnimationComponent;
        
        private CharacterPhysicsDataStorage characterPhysicsData;
        private CharacterAimAnimationDataStorage characterAimAnimationData;
        private CharacterAnimatorDataStorage characterAnimatorData;
        
        private FirearmWeapon firearmWeapon;

        private int _equippedWeaponIndex = 0;
        private bool _lockAction = false;

        private Tween _progressActionTween;
        
        
        public bool IsFirearmWeaponEquipped => firearmWeapon != null;


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
        
        private bool CanMakeDistanceAttack =>
            IsFirearmWeaponEquipped && CharacterInputComponent.AttackAction && !firearmWeapon.IsShootLock;

        private bool IsReload =>
            CharacterInputComponent.ReloadAction;
        
        
        [Inject]
        public void Construct(ICharacterEntity character,
                              TickableManager tickableManager,
                              IInventoryService inventoryService,
                              FirearmWeaponFactory firearmWeaponFactory,
                              ICharacterDataStorage<CharacterPhysicsDataStorage> characterPhysicsDataStorage,
                              ICharacterDataStorage<CharacterAimAnimationDataStorage> characterAimAnimationDataStorage,
                              ICharacterDataStorage<CharacterAnimatorDataStorage> characterAnimatorDataStorage)
        {
            this.character = character;
            this.tickableManager = tickableManager;
            this.inventoryService = inventoryService;
            this.firearmWeaponFactory = firearmWeaponFactory;
            
            characterPhysicsData = characterPhysicsDataStorage.Data;
            characterAimAnimationData = characterAimAnimationDataStorage.Data;
            characterAnimatorData = characterAnimatorDataStorage.Data;
        }
        
        public void Initialize()
        {
            bool success = false;
            character.TryGetCharacterComponent<ICharacterInputComponent>(out characterInputComponent);

            if (character.TryGetCharacterComponent<ICharacterAnimationComponent>(out ICharacterAnimationComponent animationComponent))
            {
                if (animationComponent is CharacterAnimationComponent neededAnimationComponent)
                    characterAnimationComponent = neededAnimationComponent;
            }

            OnUpdateEquippedWeaponItem();
            
            inventoryService.OnUpdateEquippedWeaponItem += OnUpdateEquippedWeaponItem;
            characterAnimatorData.AnimationReactionsTriggerController.onCompleteAction += CompleteAction;
            tickableManager.Add(this);
        }

        public void DeInitialize()
        {
            inventoryService.OnUpdateEquippedWeaponItem -= OnUpdateEquippedWeaponItem;
            characterAnimatorData.AnimationReactionsTriggerController.onCompleteAction -= CompleteAction;
            tickableManager.Remove(this);
        }

        public void OnStart()
        {
            _lockAction = false;
        }

        public void Tick()
        {
            PaintLaserBeam();
            
            if (CharacterInputComponent == null || _lockAction)
                return;

            if (IsReload && firearmWeapon.ClipSize < firearmWeapon.ClipSizeMax)
            {
                Reload();
                return;
            }

            if (!CharacterInputComponent.AimAction)
            {
                TryMakeMeleeAttack();
            }
            else
            {
                if (!CanMakeDistanceAttack)
                    return;
                
                if (firearmWeapon.ClipSize == 0)
                {
                    Reload();
                    
                    return;
                }
                    
                Vector3 direction = characterAimAnimationData.BodyPistolAimPointObject.position - firearmWeapon.GunMuzzleTransform.position;
                direction = direction.normalized;
                
                firearmWeapon.MakeShot(direction, false);
                onUpdateWeaponClipSize?.Invoke(firearmWeapon.ClipSize, firearmWeapon.ClipSizeMax);
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

            firearmWeapon.SetLaserStatus(false);
            if (characterAnimatorData.IsReloadByAnimation)
            {
                        
            }
            else
            {
                onReloadStart?.Invoke();
                onReloadProgress?.Invoke(0, 2.0f);
                float progress = 0;

                _progressActionTween = DOTween.To(() => progress, x => progress = x, firearmWeapon.ReloadSpeed, firearmWeapon.ReloadSpeed)
                    .OnUpdate(() =>
                    {
                        onReloadProgress?.Invoke(progress, firearmWeapon.ReloadSpeed);
                    }).OnComplete(() =>
                    {
                        onReloadEnd?.Invoke();
                        firearmWeapon.ReloadClip(firearmWeapon.IsFullClipReload);
                        onUpdateWeaponClipSize?.Invoke(firearmWeapon.ClipSize, firearmWeapon.ClipSizeMax);

                        if (firearmWeapon.IsFullClipReload || firearmWeapon.ClipSize >= firearmWeapon.ClipSizeMax)
                        {
                            firearmWeapon.SetLaserStatus(true);
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
            
            if (CharacterInputComponent.AimAction && !_lockAction && CharacterAimComponent.IsAim && CharacterAimComponent.AimProgress >= 1)
            {
                firearmWeapon.SetLaserStatus(true);
                firearmWeapon.SetLaserPosition(characterAimAnimationData.BodyPistolAimPointObject.position);
            }
            else
            {
                firearmWeapon.SetLaserStatus(false);
            }
        }
        
        private void OnUpdateEquippedWeaponItem()
        {
            var equippedWeapon = inventoryService.GetEquippedWeapons()[_equippedWeaponIndex];
            if (equippedWeapon == null || equippedWeapon.itemInventoryData == null)
            {
                if (firearmWeapon != null)
                {
                    GameObject.Destroy(firearmWeapon);
                    firearmWeapon = null;
                }
                
                characterAnimationComponent.SetAnimationValue(CharacterAnimationComponent.AnimationValue.HandPistol, 0);
            }
            else
            {
                firearmWeapon = firearmWeaponFactory.Create(equippedWeapon, characterPhysicsData.RightHandPoint);
                characterAnimationComponent.SetAnimationValue(CharacterAnimationComponent.AnimationValue.HandPistol, 1);
                
                onUpdateWeaponClipSize?.Invoke(firearmWeapon.ClipSize, firearmWeapon.ClipSizeMax);
            }
        }
    }
}