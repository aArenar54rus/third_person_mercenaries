using UnityEngine;


namespace Arenar.Character
{
    [CreateAssetMenu(menuName = "Characters/Shooting Gallery Target")]
    public class ShootingGalleryTargetParameters : ScriptableObject
    {
        [SerializeField] private int _baseHealth;
        [SerializeField] private int _addedHealthByLvl;

        [Space(10), Header("Movement")]
        [SerializeField] private SerializableDictionary<LevelDifficult, float> _baseSpeed;
        [SerializeField] private SerializableDictionary<LevelDifficult, float>  _baseAccelerationSpeedMultiply;
        [SerializeField] private SerializableDictionary<LevelDifficult, float>  _baseRotationSpeed;

        [Space(10), Header("Attack")]
        [SerializeField] private int _baseDamage;
        [SerializeField] private int _addedDamageByLvl;


        public int BaseHealth => _baseHealth;
        public int AddedHealthByLvl => _addedHealthByLvl;
        public SerializableDictionary<LevelDifficult, float> BaseSpeed => _baseSpeed;
        public SerializableDictionary<LevelDifficult, float> BaseAccelerationSpeedMultiply => _baseAccelerationSpeedMultiply;
        public SerializableDictionary<LevelDifficult, float> BaseRotationSpeed => _baseRotationSpeed;
        public int BaseDamage => _baseDamage;
        public int AddedDamageByLvl => _addedDamageByLvl;
    }
}