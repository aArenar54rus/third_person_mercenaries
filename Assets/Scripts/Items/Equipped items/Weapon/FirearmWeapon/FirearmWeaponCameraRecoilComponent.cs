using Cinemachine;
using UnityEngine;


public class FirearmWeaponCameraRecoilComponent : MonoBehaviour
{
    [SerializeField] private CinemachineImpulseSource _impulseSurce;


    public void ApplyShootRecoil(Vector3 direction)
    {
        _impulseSurce.GenerateImpulseWithVelocity(direction);
    }
}
