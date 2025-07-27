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
        private SerializableDictionary<ECharacterDamageContainerBodyType, CharacterPartData> _partDatas;
        
        [Space(10), Header("Health")]
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
        
        [Space(10), Header("Attack")]
        [SerializeField]
        private float stunTime;


        public int BaseHealth => Random.Range(baseMinHealth, baseMaxHealth);
        public int AddedHealthByLvl => addedHealthByLvl;
        public SerializableDictionary<LevelDifficult, float> BaseSpeed => _baseSpeed;
        public SerializableDictionary<LevelDifficult, float> BaseAccelerationSpeedMultiply => _baseAccelerationSpeedMultiply;
        public SerializableDictionary<LevelDifficult, float> BaseRotationSpeed => _baseRotationSpeed;
        public int BaseDamage => baseDamage;
        public int AddedDamageByLvl => addedDamageByLvl;
        public float StunTime => stunTime;
        public SerializableDictionary<ECharacterDamageContainerBodyType, CharacterPartData> PartDatas => _partDatas;



        [Serializable]
        public class CharacterPartData
        {
            [SerializeField, Range(0.1f, 1.0f)] private float _stunScorePercent;
            [SerializeField, Range(0.1f, 1.0f)] private float _criticalChance;
            
            
            public float StunScorePercent => _stunScorePercent;
            public float CriticalChance => _criticalChance;
        }
    }
}