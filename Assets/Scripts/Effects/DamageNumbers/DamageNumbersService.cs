using DamageNumbersPro;
using System.Collections.Generic;
using UnityEngine;


namespace Arenar.Services.UI {
    public class DamageNumbersService : IEffectMessagesService {
        private readonly Transform _damageNumbersContainer;
        private readonly DamageNumbersDataSO _damageNumbersDataSo;
        
        private Dictionary<EEffectMessage, List<DamageNumber>> _damageNumbersPrefabs;
        
        public DamageNumbersService(DamageNumbersDataSO damageNumbersDataSO) {
            _damageNumbersDataSo = damageNumbersDataSO;

            _damageNumbersContainer = new GameObject("DamageNumbersContainer").transform;
            Object.DontDestroyOnLoad(_damageNumbersContainer.gameObject);

            Initialize();
        }


        private void Initialize() {
            /*_damageNumbersPrefabs = new Dictionary<EEffectMessage, List<DamageNumber>>();

            if (_damageNumbersDataSo.IsShowDamage)
                CreatePrefabs(EEffectMessage.Damage, _damageNumbersDataSo.DamagePrefab);
            
            if (_damageNumbersDataSo.IsShowHeal)
                CreatePrefabs(EEffectMessage.Heal, _damageNumbersDataSo.HealPrefab);

            if (_damageNumbersDataSo.IsShowStun)
                CreatePrefabs(EEffectMessage.Stun, _damageNumbersDataSo.StunPrefab);
            
            if (_damageNumbersDataSo.IsShowCritical)
                CreatePrefabs(EEffectMessage.Critical, _damageNumbersDataSo.CriticalPrefab);*/
        }
        
        private void CreatePrefabs(EEffectMessage type, DamageNumber originalPrefab) {
            _damageNumbersPrefabs.Add(type, new List<DamageNumber>(_damageNumbersDataSo.PrefabsCountMin));
            for (int i = 0; i < _damageNumbersDataSo.PrefabsCountMin; i++) {
                var prefab = GameObject.Instantiate(originalPrefab, _damageNumbersContainer);
                _damageNumbersPrefabs[type].Add(prefab);
            }
        }
        
        public void SpawnDamageNumber(Transform target, int damage) {
            DamageNumber damagePrefab = _damageNumbersDataSo.DamagePrefab;
            
            /*foreach (var damageNumber in _damageNumbersPrefabs[EEffectMessage.Damage]) {
                if (damageNumber.IsAlive(Time.time))
                    continue;
                
                damagePrefab = damageNumber;
                break;
            }

            if (!damagePrefab) {
                damagePrefab = GameObject.Instantiate(_damageNumbersDataSo.DamagePrefab, _damageNumbersContainer);
                _damageNumbersPrefabs[EEffectMessage.Damage].Add(damagePrefab);
            }*/

            damagePrefab.Spawn(target.position, damage, target);
        }

        public void SpawnCritical(Transform target) {
            throw new System.NotImplementedException();
        }

        public void SpawnStun(Transform target) {
            throw new System.NotImplementedException();
        }
        
        public void SpawnHealing(Transform target, int damage) {
            throw new System.NotImplementedException();
        }
    }
}