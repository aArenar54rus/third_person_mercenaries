using UnityEngine;


namespace Arenar.Items
{
    public class ShotgunFirearmWeapon : FirearmWeapon
    {
        [Space(10)]
        [SerializeField] private int _bulletsPerShot = 6;
        [SerializeField] private Vector3 _shotgunBulletRandomVector;
        
        
        public override FirearmWeaponClass FirearmWeaponClass => FirearmWeaponClass.Shotgun;


        protected override void InitializeBullets(Vector3 direction)
        {
            for(int i = 0; i < _bulletsPerShot; i++)
                CreateBullet(BulletDirectionSpread(direction));
        }

        private Vector3 BulletDirectionSpread(Vector3 direction)
        {
            return new Vector3(
                direction.x + Random.Range(-_shotgunBulletRandomVector.x, _shotgunBulletRandomVector.z),
                direction.y + Random.Range(-_shotgunBulletRandomVector.y, _shotgunBulletRandomVector.y),
                direction.z + Random.Range(-_shotgunBulletRandomVector.z, _shotgunBulletRandomVector.z)
                );
        }
    }
}