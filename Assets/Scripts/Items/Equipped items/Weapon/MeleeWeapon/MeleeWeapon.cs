using Arenar.Character;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Arenar.Items
{
    public abstract class MeleeWeapon : MonoBehaviour, IWeapon, ILevelItem
    {
        [SerializeField]
        protected Transform secondHandPoint;
        [SerializeField]
        protected Vector3 rotationInHands;
        
        
        public Vector3 RotationInHands => rotationInHands;
        
        public WeaponType WeaponType => WeaponType.Melee;
        
        public int ItemLevel { get; protected set; }
        
        public float TimeBetweenAttacks { get; }
        
        public MeleeWeaponInventoryItemData MeleeWeaponInventoryItemData
        {
            get;
            protected set;
        }
        
        public IMeleeWeaponAttackComponent MeleeWeaponAttackComponent
        {
            get;
            protected set;
        }
        
        public float Damage
        { 
            get
            {
                int damage = (int) MeleeWeaponInventoryItemData.MeleeWeaponData.DefaultDamage;
                for (int i = 0; i < ItemLevel; i++)
                    damage += Random.Range(
                        (int)MeleeWeaponInventoryItemData.MeleeWeaponData.BulletLevelMinDamage, (int)MeleeWeaponInventoryItemData.MeleeWeaponData.BulletLevelMaxDamage);

                return damage;
            } 
        }
        
        public ICharacterEntity ItemOwner { get; protected set; }
        
        public ItemInventoryData ItemInventoryData { get; protected set; }
        
        public bool IsAttackProcess { get; private set; }
        
        
        public void SetItemLevel(int itemLevel)
        {
            ItemLevel = itemLevel;
        }

        public void InitializeItem(ItemInventoryData itemInventoryData,
                                   Dictionary<System.Type, IEquippedItemComponent> itemComponents)
        {
            if (itemInventoryData is not MeleeWeaponInventoryItemData weaponInventoryItemData
                || itemInventoryData.ItemType != ItemType.Weapon)
            {
                Debug.LogError($"You try initialize weapon as {itemInventoryData.ItemType}. Check your code!");
                return;
            }

            MeleeWeaponInventoryItemData = weaponInventoryItemData;
            ItemInventoryData = itemInventoryData;
            
            MeleeWeaponAttackComponent = GetEquippedComponent<IMeleeWeaponAttackComponent>(itemComponents);
        }
        
        public virtual void MakeMeleeAttack()
        {
            IsAttackProcess = true;
        }

        public void PickUpItem(ICharacterEntity characterOwner)
        {
            ItemOwner = characterOwner;
            if (ItemOwner.TryGetCharacterComponent<ICharacterAnimationComponent>(out ICharacterAnimationComponent animComponent))
            {
                animComponent.onAnimationEvent += AnimationEventHandler;
            }
            
            MeleeWeaponAttackComponent.SetOwner(ItemOwner);
            IsAttackProcess = false;
        }
        
        public void DropItem()
        {
            if (ItemOwner.TryGetCharacterComponent<ICharacterAnimationComponent>(out ICharacterAnimationComponent animComponent))
            {
                animComponent.onAnimationEvent -= AnimationEventHandler;
            }
            ItemOwner = null;
        }
        
        protected T GetEquippedComponent<T>(Dictionary<System.Type, IEquippedItemComponent>  components)
            where T : IEquippedItemComponent
        {
            if (components.ContainsKey(typeof(T)))
                return (T)components[typeof(T)];
            
            Debug.LogError($"No component of type {typeof(T)}");
            return default;
        }
        
        private void AnimationEventHandler(string animationEventKey)
        {
            if (animationEventKey == AnimationEventKeys.BREAK_ANIM_TRIGGER)
            {
                IsAttackProcess = false;
                return;
            }

            if (animationEventKey == AnimationEventKeys.MAKE_HIT_ANIM_TRIGGER)
            {
                if (IsAttackProcess == false)
                    return;

                DamageData damageData = new DamageData(
                    ItemOwner,
                    (int)Damage,
                    0,
                    MeleeWeaponInventoryItemData.MeleeWeaponData.GetStunPoints(),
                    ItemOwner.CharacterTransform.forward * MeleeWeaponInventoryItemData.MeleeWeaponData.PhysicalMight
                );
                MeleeWeaponAttackComponent.MakeMeleeAttack(damageData);
            }
            
            if (animationEventKey == AnimationEventKeys.COMPLETE_MELEE_ATTACK_ANIM_TRIGGER)
            {
                IsAttackProcess = false;
                return;
            }
        }
    }
}