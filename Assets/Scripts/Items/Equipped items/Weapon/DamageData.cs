using Arenar.Character;
using UnityEngine;

namespace Arenar
{
    public class DamageData
    {
        private ICharacterEntity damageSetterCharacter;
        private int weaponDamage;
        private int addedDamageByCharacterUpgrades;
        private Vector3 bulletPhysicalMight;


        public ICharacterEntity DamageSetterCharacter => damageSetterCharacter;
        public int WeaponDamageWithUpgrades => weaponDamage + addedDamageByCharacterUpgrades;
        public int AddedDamageByCharacterUpgrades => addedDamageByCharacterUpgrades;
        public Vector3 BulletPhysicalMight => bulletPhysicalMight;


        public DamageData(ICharacterEntity damageSetterCharacter, int weaponDamage, int addedDamageByCharacterUpgrades, Vector3 bulletPhysicalMight)
        {
            this.damageSetterCharacter = damageSetterCharacter;
            this.bulletPhysicalMight = bulletPhysicalMight;
            this.weaponDamage = weaponDamage;
            this.addedDamageByCharacterUpgrades = addedDamageByCharacterUpgrades;
        }
    }
}