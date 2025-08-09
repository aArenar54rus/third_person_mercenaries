using DamageNumbersPro;
using UnityEngine;

namespace Arenar.Services.UI {
    [CreateAssetMenu(menuName = "Effects/Damage Numbers Collection")]
    public class DamageNumbersDataSO : ScriptableObject {
        [SerializeField] private DamageNumber _damagePrefab;
        [SerializeField] private DamageNumber _healPrefab;
        [SerializeField] private DamageNumber _criticalPrefab;
        [SerializeField] private DamageNumber _stunPrefab;

        [Space(10), Header("Parameters")]
        [SerializeField] private int _prefabsCountMin = 5;
        [SerializeField] private bool _isShowDamage;
        [SerializeField] private bool _isShowHeal;
        [SerializeField] private bool _isShowCritical;
        [SerializeField] private bool _isShowStun;
        
        
        public DamageNumber DamagePrefab => _damagePrefab;
        public DamageNumber HealPrefab => _healPrefab;
        public DamageNumber CriticalPrefab => _criticalPrefab;
        public DamageNumber StunPrefab => _stunPrefab;
        
        public int PrefabsCountMin => _prefabsCountMin;
        public bool IsShowDamage => _isShowDamage;
        public bool IsShowHeal => _isShowHeal;
        public bool IsShowCritical => _isShowCritical;
        public bool IsShowStun => _isShowStun;
    }
}