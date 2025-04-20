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
        
        public MeleeWeaponItemData MeleeWeaponItemData
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
                int damage = (int) MeleeWeaponItemData.MeleeWeaponData.DefaultDamage;
                for (int i = 0; i < ItemLevel; i++)
                    damage += Random.Range(
                        (int)MeleeWeaponItemData.MeleeWeaponData.BulletLevelMinDamage, (int)MeleeWeaponItemData.MeleeWeaponData.BulletLevelMaxDamage);

                return damage;
            } 
        }
        
        public ICharacterEntity ItemOwner { get; protected set; }
        
        public ItemData ItemData { get; protected set; }
        
        public bool IsAttackProcess { get; private set; }
        
        
        public void SetItemLevel(int itemLevel)
        {
            ItemLevel = itemLevel;
        }

        public void InitializeItem(ItemData itemData,
                                   Dictionary<System.Type, IEquippedItemComponent> itemComponents)
        {
            if (itemData is not MeleeWeaponItemData weaponInventoryItemData
                || itemData.ItemType != ItemType.FirearmWeapon)
            {
                Debug.LogError($"You try initialize weapon as {itemData.ItemType}. Check your code!");
                return;
            }

            MeleeWeaponItemData = weaponInventoryItemData;
            ItemData = itemData;
            
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
                    MeleeWeaponItemData.MeleeWeaponData.GetStunPoints(),
                    ItemOwner.CharacterTransform.forward * MeleeWeaponItemData.MeleeWeaponData.PhysicalMight
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