using System.Numerics;
using Arenar.CameraService;
using DamageNumbersPro;
using UnityEngine;
using System.Collections;
using Vector3 = UnityEngine.Vector3;


namespace Arenar.Services.DamageNumbersService
{
    public class DamageNumbersService : IDamageNumbersService
    {
        private DamageNumbersPrefabsData _damageNumbersPrefabsData;


        public DamageNumbersService(DamageNumbersPrefabsData damageNumbersPrefabsData)
        {
            _damageNumbersPrefabsData = damageNumbersPrefabsData;
        }
        
        
        public void PlayDamageNumber(int damageNumber, Transform damageTarget, Transform attackerTarget)
        {
            DamageNumber newPopup = _damageNumbersPrefabsData.DefaultDamageNumberPrefab.Spawn(damageTarget.position, damageNumber);
            
            newPopup.enableFollowing = true;
            newPopup.SetFollowedTarget(damageTarget);

            float distance = Vector3.Distance(damageTarget.position, attackerTarget.position);
            newPopup.SetScale(distance * _damageNumbersPrefabsData.DamageNumberMultiplier);
        }
    }
}