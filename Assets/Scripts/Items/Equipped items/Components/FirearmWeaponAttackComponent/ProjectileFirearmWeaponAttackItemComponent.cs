using DG.Tweening;
using UnityEngine;

namespace Arenar
{
    public class ProjectileFirearmWeaponAttackItemComponent : IFirearmWeaponAttackItemComponent
    {
        private EffectsSpawner _effectsSpawner;
        private EffectType _effectType;
        private float _projectileSpeed;


        public ProjectileFirearmWeaponAttackItemComponent(EffectsSpawner effectsSpawner,
                                                      EffectType effectType,
                                                      float projectileSpeed)
        {
            _effectsSpawner = effectsSpawner;
            _effectType = effectType;
            _projectileSpeed = projectileSpeed;
        }
        
        
        public void MakeShoot(Transform muzzle, Vector3 direction, DamageData damageData)
        {
            PlayMuzzleFlashEffect(muzzle);
            
            ProjectileControl projectile = _effectsSpawner.GetItemProjectile(_effectType);
            projectile.Initialize(muzzle.position,
                muzzle.rotation, 
                direction,
                _projectileSpeed,
                damageData,
                _effectsSpawner);
        }
        
        private void PlayMuzzleFlashEffect(Transform muzzle)
        {
            var effect = _effectsSpawner.GetEffect(EffectType.MuzzleFlashYellow);
            effect.gameObject.SetActive(true);
            effect.transform.SetParent(muzzle);
            effect.transform.localPosition = Vector3.zero;
            effect.transform.rotation = Quaternion.Inverse(muzzle.rotation);  //gunMuzzleTransform.rotation;
            effect.Play();

            DOVirtual.DelayedCall(1.0f, () =>
            {
                _effectsSpawner.ReturnEffect(effect);
            });
        }
    }
}