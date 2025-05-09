using Arenar.Character;
using UnityEngine;


namespace Arenar
{
    public abstract class ProjectileControl : MonoBehaviour
    {
        [SerializeField] protected Transform bulletTransform;
        [SerializeField] private int _liveTimeMax = 5;
        
        protected DamageData _damageData;
        
        protected Vector3 _movementVector;
        protected float _speed;

        private float _liveTime;
        private EffectsSpawner _effectsSpawner;
        

        public bool IsActive { get; private set; } = false;


        public void Initialize(Vector3 startPoint,
            Quaternion rotation,
            Vector3 movementVector,
            float speed,
            DamageData damageData,
            EffectsSpawner effectsSpawner)
        {
            bulletTransform.position = startPoint;
            bulletTransform.rotation = rotation;
            
            _damageData = damageData;
            _speed = speed;
            _movementVector = movementVector;
            
            gameObject.SetActive(true);
            _liveTime = 0;

            _effectsSpawner = effectsSpawner;
            
            IsActive = true;
        }

        public void DeInitialize()
        {
            gameObject.SetActive(false);
            IsActive = false;
        }

        private void Update()
        {
            bulletTransform.position += _movementVector * (_speed * Time.deltaTime);
            _liveTime += Time.deltaTime;
            if (_liveTime >= _liveTimeMax)
                DeInitialize();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent<ComponentCharacterController>(out ComponentCharacterController characterController)
                && characterController.TryGetCharacterComponent<ICharacterLiveComponent>(out ICharacterLiveComponent characterLiveComponent))
            {
                characterLiveComponent.SetDamage(_damageData);
            }
            
            ParticleSystem effect = _effectsSpawner.GetEffect(EffectType.BulletCollision);
            effect.transform.position = this.transform.position;
            effect.gameObject.SetActive(true);
            effect.Play();

            DeInitialize();
        }
    }
}