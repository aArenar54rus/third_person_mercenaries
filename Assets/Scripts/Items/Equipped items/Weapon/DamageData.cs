using Arenar.Character;
using UnityEngine;


namespace Arenar
{
    public class DamageData
    {
        private ICharacterEntity _damageSetterCharacter;
        private int _damage;
        private Vector3 _bulletMight;


        public ICharacterEntity DamageSetterCharacter => _damageSetterCharacter;
        public int Damage => _damage;
        public Vector3 BulletMight => _bulletMight;


        public DamageData(ICharacterEntity damageSetterCharacter, int damage, Vector3 bulletMight)
        {
            _damageSetterCharacter = damageSetterCharacter;
            _bulletMight = bulletMight;
            _damage = damage;
        }
    }
}