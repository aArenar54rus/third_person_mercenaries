using Arenar.Character;
using System;
using UnityEngine;

namespace Arenar.Services.LevelsService
{
	[CreateAssetMenu(menuName = "LevelsData/SurvivalLevelInfoCollection")]
	public class SurvivalLevelInfoCollection : ScriptableObject
	{
		[SerializeField]
		private float minSpawnInterval;
		[SerializeField]
		private float maxSpawnInterval;
		[SerializeField]
		private int maxEnemyCountOnScene;
		[SerializeField]
		private SpawnPattern[] spawnPatterns;


		public float MinSpawnInterval => minSpawnInterval;
		public float MaxSpawnInterval => maxSpawnInterval;
		public int MaxEnemyCountOnScene => maxEnemyCountOnScene;
		public SpawnPattern[] SpawnPatterns => spawnPatterns;
		


		[Serializable]
		public class SpawnPattern
		{
			[SerializeField]
			private CharacterTypeKeys[] spawnNpcs;


			public CharacterTypeKeys[] SpawnNpcs => spawnNpcs;
		}
	}
}