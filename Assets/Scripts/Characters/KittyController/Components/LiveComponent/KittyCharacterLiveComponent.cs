using System;
using DG.Tweening;
using UnityEngine;
using Zenject;


namespace CatSimulator.Character
{
    public class KittyCharacterLiveComponent : ICharacterLiveComponent
    {
        public event Action OnKittyDie;
        
        
        private Transform kittyObject;
        private int healthMax;
        private int health;

        
        public bool IsAlive =>
            health <= 0;

        public int Health =>
            health;
        
        public int HealthMax =>
            healthMax;


        [Inject]
        public void Construct(ICharacterDataStorage<CharacterPhysicsDataStorage> characterPhysicsDataStorage)
        {
            kittyObject = characterPhysicsDataStorage.Data.CharacterTransform;
        }
        
        public void SetDamage(int damageCount)
        {
            if (!IsAlive)
                return;
            
            health -= damageCount;
            if (health <= 0)
                SetDeath();
        }

        public void SetAlive()
        {
            health = healthMax;
        }

        public void SetDeath()
        {
            DOVirtual.DelayedCall(1.0f, () => kittyObject.gameObject.SetActive(false));
            health = 0;

            OnKittyDie?.Invoke();
        }
        
        public void Initialize() { }

        public void DeInitialize() { }

        public void OnStart()
        {
            SetAlive();
        }
    }
}