using UnityEngine;


namespace Arenar
{
    public abstract class ProjectileControl : MonoBehaviour
    {
        [SerializeField] protected Transform bulletTransform;
        [SerializeField] protected Rigidbody bulletRb;

        private float bulletDamage;

        public bool IsActive { get; private set; } = false;


        public void Initialize(Vector3 startPoint, Vector3 movementVector, float speed, float bulletDamage)
        {
            bulletTransform.position = startPoint;
            bulletTransform.localRotation = Quaternion.Euler(movementVector);
            this.bulletDamage = bulletDamage;
            
            gameObject.SetActive(true);
            bulletRb.velocity = movementVector * speed;

            IsActive = true;
        }

        public void DeInitialize()
        {
            bulletRb.velocity = Vector3.zero;
            gameObject.SetActive(false);

            IsActive = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            DeInitialize();
        }
    }
}