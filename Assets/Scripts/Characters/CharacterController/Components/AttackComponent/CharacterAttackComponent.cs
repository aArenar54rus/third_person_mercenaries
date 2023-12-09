using Arenar.Services.InventoryService;
using UnityEngine;
using Zenject;


namespace Arenar.Character
{
    public class CharacterAttackComponent : ICharacterAttackComponent, ITickable
    {
        private ICharacterEntity character;
        private TickableManager tickableManager;
        private ItemProjectileSpawner itemProjectileSpawner;
        private IInventoryService inventoryService;
        
        private ICharacterInputComponent characterInputComponent;
        private ICharacterRayCastComponent characterRayCastComponent;
        private FirearmWeaponFactory firearmWeaponFactory;
        private ICharacterAnimationComponent<CharacterAnimationComponent.Animation, CharacterAnimationComponent.AnimationValue> characterAnimationComponent;
        
        private CharacterPhysicsDataStorage characterPhysicsData;
        private CharacterAimAnimationDataStorage characterAimAnimationData;
        
        private FirearmWeapon firearmWeapon;

        private int _equippedWeaponIndex = 0;
        
        
        public bool IsFirearmWeaponEquipped => firearmWeapon != null;


        public ICharacterInputComponent CharacterInputComponent
        {
            get
            {
                if (characterInputComponent == null)
                    character.TryGetCharacterComponent<ICharacterInputComponent>(out characterInputComponent);
                return characterInputComponent;
            }
        }
        
        
        [Inject]
        public void Construct(ICharacterEntity character,
                              TickableManager tickableManager,
                              IInventoryService inventoryService,
                              FirearmWeaponFactory firearmWeaponFactory,
                              ICharacterDataStorage<CharacterPhysicsDataStorage> characterPhysicsDataStorage,
                              ICharacterDataStorage<CharacterAimAnimationDataStorage> characterAimAnimationDataStorage)
        {
            this.character = character;
            this.tickableManager = tickableManager;
            this.inventoryService = inventoryService;
            this.firearmWeaponFactory = firearmWeaponFactory;
            
            characterPhysicsData = characterPhysicsDataStorage.Data;
            characterAimAnimationData = characterAimAnimationDataStorage.Data;
        }
        
        public void Initialize()
        {
            bool success = false;
            character.TryGetCharacterComponent<ICharacterInputComponent>(out characterInputComponent);
            character.TryGetCharacterComponent<ICharacterRayCastComponent>(out characterRayCastComponent);
            
            if (character.TryGetCharacterComponent<ICharacterAnimationComponent>(out ICharacterAnimationComponent animationComponent))
            {
                if (animationComponent is CharacterAnimationComponent neededAnimationComponent)
                    characterAnimationComponent = neededAnimationComponent;
            }

            OnUpdateEquippedWeaponItem();
            
            inventoryService.OnUpdateEquippedWeaponItem += OnUpdateEquippedWeaponItem;
            tickableManager.Add(this);
        }

        public void DeInitialize()
        {
            inventoryService.OnUpdateEquippedWeaponItem -= OnUpdateEquippedWeaponItem;
            tickableManager.Remove(this);
        }

        public void OnStart()
        {

        }

        public void Tick()
        {
            PaintLaserBeam();
            
            if (CharacterInputComponent == null)
                return;

            if (!CharacterInputComponent.AimAction)
            {
                CheckMeleeAttack();
            }
            else
            {
                CheckDistanceAttack();
            }
        }

        private void CheckMeleeAttack()
        {
            
        }

        private void CheckDistanceAttack()
        {
            if (!IsFirearmWeaponEquipped)
                return;

            if (!CharacterInputComponent.AttackAction)
                return;
            
            if (firearmWeapon.ClipSize == 0)
            {
                firearmWeapon.ReloadClip(true);
            }
            else
            {
                Vector3 direction = characterAimAnimationData.BodyAimPointObject.position - firearmWeapon.GunMuzzleTransform.position;
                direction = direction.normalized;
                firearmWeapon.MakeShot(direction, false);
                characterAnimationComponent.PlayAnimation(CharacterAnimationComponent.Animation.Shoot);
            }
        }

        private void PaintLaserBeam()
        {
            if (CharacterInputComponent == null)
                return;
            
            if (!IsFirearmWeaponEquipped)
                return;
            
            if (CharacterInputComponent.AimAction)
            {
                firearmWeapon.LineRendererEffect.gameObject.SetActive(true);
                firearmWeapon.LineRendererEffect.SetPosition(1,
                    characterAimAnimationData.BodyAimPointObject.position);
            }
            else
            {
                firearmWeapon.LineRendererEffect.gameObject.SetActive(false);
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
            }
        }
    }
}