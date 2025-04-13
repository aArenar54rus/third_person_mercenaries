using UnityEngine;

namespace Arenar.Character
{
	public class EnemyCharacterController : PhysicalHumanoidComponentCharacterController,
											ICharacterDataStorage<EnemyCharacterDataStorage>
	{
		[SerializeField]
		private EnemyCharacterDataStorage enemyCharacterDataStorage;
		
		
		EnemyCharacterDataStorage ICharacterDataStorage<EnemyCharacterDataStorage>.Data =>
			enemyCharacterDataStorage;
	}
}