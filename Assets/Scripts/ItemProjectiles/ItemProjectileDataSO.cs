using UnityEngine;


namespace Arenar
{
    [CreateAssetMenu(fileName = "Item Projectiles Data", menuName = "Data/Item Projectiles", order = 1)]
    public class ItemProjectileDataSO : ScriptableObject
    {
        [SerializeField] private SerializableDictionary<ItemFirearmAttackType, ProjectileControl> itemProjectilePrefabs;
        [SerializeField] private int defaultSpawnCount = 20;


        public SerializableDictionary<ItemFirearmAttackType, ProjectileControl> ItemProjectilePrefabs =>
            itemProjectilePrefabs;

        public int DefaultSpawnCount => defaultSpawnCount;
    }
}