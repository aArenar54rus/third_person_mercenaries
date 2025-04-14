using Arenar.Character;
using UnityEngine;

namespace Arenar
{
    public class DamageData
    {
        private ICharacterEntity damageSetterCharacter;
        private int weaponDamage;
        private int addedDamageByCharacterUpgrades;
        private int addedStunPoint;
        private Vector3 physicalMight;
        public bool isCritical;


        public ICharacterEntity DamageSetterCharacter => damageSetterCharacter;
        public int WeaponDamageWithUpgrades => weaponDamage + addedDamageByCharacterUpgrades;
        public int AddedDamageByCharacterUpgrades => addedDamageByCharacterUpgrades;
        public int AddedStunPoint => addedStunPoint;
        public Vector3 PhysicalMight => physicalMight;


        public DamageData(ICharacterEntity damageSetterCharacter, int weaponDamage,
                          int addedDamageByCharacterUpgrades, int addedStunPoint, Vector3 physicalMight)
        {
            this.damageSetterCharacter = damageSetterCharacter;
            this.physicalMight = physicalMight;
            this.weaponDamage = weaponDamage;
            this.addedStunPoint = addedStunPoint;
            this.addedDamageByCharacterUpgrades = addedDamageByCharacterUpgrades;
            isCritical = false;
        }
    }
}