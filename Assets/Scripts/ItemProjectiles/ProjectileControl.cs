using UnityEngine;


namespace Arenar
{
    public abstract class ProjectileControl : MonoBehaviour
    {
        [SerializeField] protected Transform bulletTransform;

        protected Vector3 _movementVector;
        protected float _bulletDamage;
        protected float _speed;

        public bool IsActive { get; private set; } = false;


        public void Initialize(Vector3 startPoint, Vector3 movementVector, float speed, float bulletDamage)
        {
            bulletTransform.position = startPoint;
            bulletTransform.localRotation = Quaternion.Euler(movementVector);
            
            _bulletDamage = bulletDamage;
            _speed = speed;
            _movementVector = movementVector;
            
            gameObject.SetActive(true);

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
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.LogError("fly end");
            DeInitialize();
        }
    }
}