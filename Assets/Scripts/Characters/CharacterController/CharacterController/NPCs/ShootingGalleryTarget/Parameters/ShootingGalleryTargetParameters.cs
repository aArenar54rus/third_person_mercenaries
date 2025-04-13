using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


namespace Arenar.Character
{
    [CreateAssetMenu(menuName = "Characters/Enemy Character Parameters")]
    public class EnemyCharacterParameters : ScriptableObject
    {
        [SerializeField]
        private int baseMinHealth;
        [SerializeField]
        private int baseMaxHealth;
        [SerializeField]
        private int addedHealthByLvl;

        [Space(10), Header("Movement")]
        [SerializeField]
        private SerializableDictionary<LevelDifficult, float> _baseSpeed;
        [SerializeField]
        private SerializableDictionary<LevelDifficult, float>  _baseAccelerationSpeedMultiply;
        [SerializeField]
        private SerializableDictionary<LevelDifficult, float>  _baseRotationSpeed;

        [Space(10), Header("Attack")]
        [SerializeField]
        private int baseDamage;
        [SerializeField]
        private int addedDamageByLvl;
        [SerializeField]
        private StunPartData[] stunPartDatas;


        public int BaseHealth => Random.Range(baseMinHealth, baseMaxHealth);
        public int AddedHealthByLvl => addedHealthByLvl;
        public SerializableDictionary<LevelDifficult, float> BaseSpeed => _baseSpeed;
        public SerializableDictionary<LevelDifficult, float> BaseAccelerationSpeedMultiply => _baseAccelerationSpeedMultiply;
        public SerializableDictionary<LevelDifficult, float> BaseRotationSpeed => _baseRotationSpeed;
        public int BaseDamage => baseDamage;
        public int AddedDamageByLvl => addedDamageByLvl;



        [Serializable]
        public class StunPartData
        {
            private CharacterDamageContainerBodyType bodyType;
            private int neededScoreForStun;
            
            
            public CharacterDamageContainerBodyType BodyType => bodyType;
            public int NeededScoreForStun => neededScoreForStun;
        }
    }
}