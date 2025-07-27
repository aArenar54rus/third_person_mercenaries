using Arenar.Character;
using UnityEngine;

namespace Arenar
{
    public struct DamageData
    {
        private ICharacterEntity _damageSetterCharacter;
        private ECharacterDamageContainerBodyType _bodyPart;
        private int _weaponDamage;
        private int _addedDamageByCharacterUpgrades;
        private int _addedStunPoint;
        private Vector3 _physicalMight;

        public ICharacterEntity DamageSetterCharacter => _damageSetterCharacter;
        public ECharacterDamageContainerBodyType BodyPart {
            get => _bodyPart;
            set => _bodyPart = value;
        }
        public int WeaponDamageWithUpgrades => _weaponDamage + _addedDamageByCharacterUpgrades;
        public int AddedDamageByCharacterUpgrades => _addedDamageByCharacterUpgrades;
        public int AddedStunPoint => _addedStunPoint;
        public Vector3 PhysicalMight => _physicalMight;


        public DamageData(ICharacterEntity damageSetterCharacter,
                          ECharacterDamageContainerBodyType bodyPart,
                          int weaponDamage,
                          int addedDamageByCharacterUpgrades,
                          int addedStunPoint,
                          Vector3 physicalMight)
        {
            _damageSetterCharacter = damageSetterCharacter;
            _bodyPart = bodyPart;
            _physicalMight = physicalMight;
            _weaponDamage = weaponDamage;
            _addedStunPoint = addedStunPoint;
            _addedDamageByCharacterUpgrades = addedDamageByCharacterUpgrades;
        }
    }
}