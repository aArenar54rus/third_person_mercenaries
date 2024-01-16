using UnityEngine;


namespace Arenar
{
    [CreateAssetMenu(fileName = "Item Projectiles Data", menuName = "Data/Item Projectiles", order = 1)]
    public class ItemProjectileDataSO : ScriptableObject
    {
        [SerializeField] private SerializableDictionary<EffectType, ProjectileControl> weaponProjectilePrefabs;
        [SerializeField] private SerializableDictionary<EffectType, ParticleSystem> effectPrefabs;
        [SerializeField] private int defaultSpawnCount = 20;


        public SerializableDictionary<EffectType, ProjectileControl> WeaponProjectilePrefabs =>
            weaponProjectilePrefabs;

        public SerializableDictionary<EffectType, ParticleSystem> EffectPrefabs =>
            effectPrefabs;

        public int DefaultSpawnCount => defaultSpawnCount;
    }
}