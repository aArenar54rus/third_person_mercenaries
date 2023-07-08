using System.Collections.Generic;
using UnityEngine;
using Zenject;


namespace Arenar
{
    public class ItemProjectileSpawner
    {
        private Transform spawnParent;
        private ItemProjectileDataSO itemProjectileData;
        private Dictionary<ItemProjectileType, List<ProjectileControl>> projectiles = default;


        [Inject]
        public void Construct(ItemProjectileDataSO itemProjectileData)
        {
            this.itemProjectileData = itemProjectileData;
            Initialize();
        }

        public ProjectileControl GetItemProjectile(ItemProjectileType type)
        {
            foreach (var projectile in projectiles[type])
            {
                if (projectile.IsActive)
                    continue;

                return projectile;
            }

            return SpawnProjectile(itemProjectileData.ItemProjectilePrefabs[type], type);
        }

        private void Initialize()
        {
            projectiles = new Dictionary<ItemProjectileType, List<ProjectileControl>>();
            spawnParent = GameObject.Instantiate(new GameObject("Projectiles"), null).transform;

            foreach (var prefab in itemProjectileData.ItemProjectilePrefabs)
            {
                projectiles.Add(prefab.Key, new List<ProjectileControl>());
                for (int i = 0; i < itemProjectileData.DefaultSpawnCount; i++)
                    SpawnProjectile(prefab.Value, prefab.Key);
            }
        }
        
        private ProjectileControl SpawnProjectile(ProjectileControl bulletPrefab, ItemProjectileType type)
        {
            ProjectileControl projectile = GameObject.Instantiate(bulletPrefab, spawnParent);
            projectile.DeInitialize();
            projectiles[type].Add(projectile);
            return projectile;
        }
    }
}