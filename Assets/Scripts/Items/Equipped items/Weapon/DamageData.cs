using Arenar.Character;


namespace Arenar
{
    public class DamageData
    {
        private ICharacterEntity _damageSetterCharacter;
        private int _damage;


        public ICharacterEntity DamageSetterCharacter => _damageSetterCharacter;
        public int Damage => _damage;


        public DamageData(ICharacterEntity damageSetterCharacter, int damage)
        {
            _damageSetterCharacter = damageSetterCharacter;
            _damage = damage;
        }
    }
}