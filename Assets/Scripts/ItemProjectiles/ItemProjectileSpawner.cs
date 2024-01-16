using System.Collections.Generic;
using UnityEngine;
using Zenject;


namespace Arenar
{
    public class ItemProjectileSpawner
    {
        private ItemProjectileDataSO itemProjectileData;
        
        private Dictionary<EffectType, List<ProjectileControl>> projectiles = default;
        private Transform projectilesSpawnParent;
        
        private Dictionary<EffectType, List<ParticleSystem>> effects = default;
        private Transform effectsSpawnParent;

        
        [Inject]
        public void Construct(ItemProjectileDataSO itemProjectileData)
        {
            this.itemProjectileData = itemProjectileData;
            Initialize();
        }

        public ProjectileControl GetItemProjectile(EffectType type)
        {
            foreach (var projectile in projectiles[type])
            {
                if (projectile.IsActive)
                    continue;

                return projectile;
            }

            return SpawnProjectile(itemProjectileData.WeaponProjectilePrefabs[type], type);
        }

        public ParticleSystem GetEffect(EffectType type)
        {
            foreach (var effect in effects[type])
            {
                if (effect.gameObject.activeSelf)
                    continue;

                return effect;
            }

            return SpawnEffect(itemProjectileData.EffectPrefabs[type], type);
        }

        public void ReturnEffect(ParticleSystem effect)
        {
            effect.gameObject.SetActive(false);
            effect.transform.SetParent(effectsSpawnParent);
        }

        private void Initialize()
        {
            projectiles = new Dictionary<EffectType, List<ProjectileControl>>();
            projectilesSpawnParent = GameObject.Instantiate(new GameObject("Projectiles"), null).transform;

            foreach (var prefab in itemProjectileData.WeaponProjectilePrefabs)
            {
                projectiles.Add(prefab.Key, new List<ProjectileControl>());
                for (int i = 0; i < itemProjectileData.DefaultSpawnCount; i++)
                    SpawnProjectile(prefab.Value, prefab.Key);
            }

            effects = new Dictionary<EffectType, List<ParticleSystem>>();
            effectsSpawnParent = GameObject.Instantiate(new GameObject("Effects"), null).transform;
            foreach (var prefab in itemProjectileData.EffectPrefabs)
            {
                effects.Add(prefab.Key, new List<ParticleSystem>());
                for (int i = 0; i < itemProjectileData.DefaultSpawnCount; i++)
                    SpawnEffect(prefab.Value, prefab.Key);
            }
        }
        
        private ProjectileControl SpawnProjectile(ProjectileControl bulletPrefab, EffectType type)
        {
            ProjectileControl projectile = GameObject.Instantiate(bulletPrefab, projectilesSpawnParent);
            projectile.DeInitialize();
            projectiles[type].Add(projectile);
            return projectile;
        }

        private ParticleSystem SpawnEffect(ParticleSystem particle, EffectType type)
        {
            ParticleSystem effect = GameObject.Instantiate(particle, effectsSpawnParent);
            effect.gameObject.SetActive(false);
            effects[type].Add(effect);
            return effect;
        }
    }
}