using UnityEngine;

namespace Arenar.Services.DamageNumbersService
{
    public interface IDamageNumbersService
    {
        public void PlayDamageNumber(int damageNumber, Transform damageTarget, Transform attackerTarger);
    }
}