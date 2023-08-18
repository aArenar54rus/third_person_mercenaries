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
        private ICharacterAnimationComponent<CharacterAnimationComponent.KittyAnimation, CharacterAnimationComponent.KittyAnimationValue> characterAnimationComponent;
        
        private CharacterPhysicsDataStorage characterPhysicsData;
        private CharacterAimAnimationDataStorage characterAimAnimationData;
        
        private FirearmWeapon firearmWeapon;
        
        
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
                              ICharacterDataStorage<CharacterPhysicsDataStorage> characterPhysicsDataStorage,
                              ICharacterDataStorage<CharacterAimAnimationDataStorage> characterAimAnimationDataStorage)
        {
            this.character = character;
            this.tickableManager = tickableManager;
            this.inventoryService = inventoryService;
            
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
            var equippedWeapon = inventoryService.GetEquippedWeapon();
            if (equippedWeapon == null || equippedWeapon.itemData == null)
            {
                if (firearmWeapon != null)
                {
                    GameObject.Destroy(firearmWeapon);
                    firearmWeapon = null;
                }
                
                characterAnimationComponent.SetAnimationValue(CharacterAnimationComponent.KittyAnimationValue.HandPistol, 0);
            }
            else
            {
                var weaponObject = GameObject.Instantiate(
                    Resources.Load<GameObject>("Prefabs/Items/" + equippedWeapon.itemData.Id),
                    characterPhysicsData.RightHandPoint);

                firearmWeapon = weaponObject.GetComponent<FirearmWeapon>();
                firearmWeapon.InitializeWeapon(equippedWeapon.itemData);
                
                weaponObject.transform.localPosition = Vector3.zero;
                weaponObject.transform.localRotation = Quaternion.Euler(firearmWeapon.LocalRotation);
                
                characterAnimationComponent.SetAnimationValue(CharacterAnimationComponent.KittyAnimationValue.HandPistol, 1);
            }
        }
    }
}