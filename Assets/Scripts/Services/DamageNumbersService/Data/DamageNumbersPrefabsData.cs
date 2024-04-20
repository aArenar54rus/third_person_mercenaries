using DamageNumbersPro;
using UnityEngine;


namespace Arenar.Services.DamageNumbersService
{
    [CreateAssetMenu(menuName = "Damage Number Data")]
    public class DamageNumbersPrefabsData : ScriptableObject
    {
        [SerializeField] private DamageNumber _defaultDamageNumberPrefab;
        [SerializeField] private float _damageNumberMultiplier = 0.1f;


        public DamageNumber DefaultDamageNumberPrefab => _defaultDamageNumberPrefab;
        public float DamageNumberMultiplier => _damageNumberMultiplier;
    }
}