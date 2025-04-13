using Arenar.Character;
using UnityEngine;


namespace Arenar
{
	public interface IMeleeWeaponAttackComponent : IEquippedItemComponent
	{
		void SetOwner(ICharacterEntity entityOwner);
		
		void MakeMeleeAttack(DamageData damageData);
	}
}