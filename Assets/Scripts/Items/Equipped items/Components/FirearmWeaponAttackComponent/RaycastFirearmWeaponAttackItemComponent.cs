using Arenar.Character;
using DG.Tweening;
using UnityEngine;


namespace Arenar
{
    public class RaycastFirearmWeaponAttackItemComponent : IFirearmWeaponAttackItemComponent
    {
        private EffectsSpawner _effectsSpawner;
        
        
        public RaycastFirearmWeaponAttackItemComponent(EffectsSpawner effectsSpawner)
        {
            _effectsSpawner = effectsSpawner;
        }
        
        
        public void MakeShoot(Transform muzzle, Vector3 direction, DamageData damageData)
        {
            PlayMuzzleFlashEffect(muzzle);
            
            Ray ray = new(muzzle.position, direction);
            if (!Physics.Raycast(ray, out RaycastHit hit))
                return;
                    
            if (hit.transform.TryGetComponent<CharacterController>(
                    out CharacterController characterController)
                && characterController.TryGetComponent<ICharacterLiveComponent>(
                    out ICharacterLiveComponent characterLiveComponent))
            {
                characterLiveComponent.SetDamage(damageData);

            }
                    
            ParticleSystem effect = _effectsSpawner.GetEffect(EffectType.BulletCollision);
            effect.transform.position = hit.transform.position;
            effect.Play();        
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