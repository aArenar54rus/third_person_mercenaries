using System;
using DG.Tweening;
using UnityEngine;
using Zenject;


namespace Arenar.Character
{
    public class CharacterLiveComponent : ICharacterLiveComponent
    {
        public event Action OnCharacterDie;
        public event Action<int, int> OnCharacterChangeHealthValue;


        private PlayerCharacterParametersData playerCharacterParametersData;
        private Transform kittyObject;
        private int healthMax;
        private int health;

        
        public bool IsAlive =>
            health > 0;

        public int Health =>
            health;
        
        public int HealthMax =>
            healthMax;


        [Inject]
        public void Construct(ICharacterDataStorage<CharacterPhysicsDataStorage> characterPhysicsDataStorage,
            PlayerCharacterParametersData playerCharacterParametersData)
        {
            kittyObject = characterPhysicsDataStorage.Data.CharacterTransform;
            this.playerCharacterParametersData = playerCharacterParametersData;
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

            OnCharacterDie?.Invoke();
        }

        public void Initialize()
        {
            healthMax = playerCharacterParametersData.DefaultHealthMax;
            health = healthMax;
        }

        public void DeInitialize() { }

        public void OnStart()
        {
            SetAlive();
        }
    }
}