using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Arenar.Character
{
    public class PlayerCharacterParametersUpgradeService : ICharacterParametersUpgradeService
    {
        private const string PLAYER_NAME = "Player";
        
        
        private CharacterParametersUpgradeData characterParametersUpgradeData;
        private ICharacterEntity playerCharacterEntity;
        private Dictionary<CharacterParameterUpgradeType, int> characterUpgradeParametersLevels;
        
        
        public int UpgradeScore { get; set; }


        [Inject]
        public void Construct(CharacterParametersUpgradeData characterParametersUpgradeData)
        {
            this.characterParametersUpgradeData = characterParametersUpgradeData;
        }
        
        public void InitializeCharacter(ICharacterEntity characterEntity)
        {
            UpgradeScore = 0;

            characterUpgradeParametersLevels = new Dictionary<CharacterParameterUpgradeType, int>();
            foreach (CharacterParameterUpgradeType parameter in Enum.GetValues(typeof(CharacterParameterUpgradeType)))
            {
                if (parameter == CharacterParameterUpgradeType.None)
                    continue;

                string parameterKey = PLAYER_NAME + "_" + parameter.ToString();
                int parameterLevel =  PlayerPrefs.GetInt(parameterKey, 0);
                characterUpgradeParametersLevels.Add(parameter, parameterLevel);
                
                LoadCharacterParameters(parameter, parameterLevel);
            }
        }
        
        public bool CanUpgrade(CharacterParameterUpgradeType parameterUpgradeType)
        {
            int levelCountMax = characterParametersUpgradeData
                .GetParameterProgressByType(CharacterParameterUpgradeType.HealthMax)
                .ParameterByLevel.Length;

            return (characterUpgradeParametersLevels[parameterUpgradeType] < levelCountMax);
        }

        public bool TryUpgrade(CharacterParameterUpgradeType parameterUpgradeType)
        {
            if (!CanUpgrade(parameterUpgradeType))
                return false;
            
            string parameterKey = PLAYER_NAME + "_" + parameterUpgradeType.ToString();
            int parameterLevel =  PlayerPrefs.GetInt(parameterKey, 0);
            float addedValue = characterParametersUpgradeData
                .GetParameterProgressByType(parameterUpgradeType)
                .ParameterByLevel[parameterLevel];

            switch (parameterUpgradeType)
            {
                case CharacterParameterUpgradeType.DamageMax:
                    
                    break;
                
                case CharacterParameterUpgradeType.HealthMax:
                    if (playerCharacterEntity.TryGetCharacterComponent<ICharacterLiveComponent>(out var liveComponent))
                        liveComponent.HealthContainer = new HealthContainer_Decorator(liveComponent.HealthContainer, (int)addedValue);
                    break;
                
                case CharacterParameterUpgradeType.MovementSpeedMax:
                    if (playerCharacterEntity.TryGetCharacterComponent<ICharacterMovementComponent>(out var movementComponent))
                        movementComponent.MovementContainer = new MovementContainer_SpeedDecorator(movementComponent.MovementContainer, addedValue);
                    break;
                
                default:
                    Debug.LogError("Unknown character upgrade type: " + parameterUpgradeType);
                    return false;
            }
            
            PlayerPrefs.SetInt(parameterKey, ++parameterLevel);
            return true;
        }
        
        private void LoadCharacterParameters(CharacterParameterUpgradeType parameterUpgradeType, int parameterLevel)
        {
            if (parameterLevel == 0)
                return;
            
            switch (parameterUpgradeType)
            {
                case CharacterParameterUpgradeType.DamageMax:
                    
                    return;
                
                case CharacterParameterUpgradeType.HealthMax:
                    if (!playerCharacterEntity.TryGetCharacterComponent<ICharacterLiveComponent>(out var liveComponent))
                        return;

                    int addedHealth = characterParametersUpgradeData
                        .GetParameterProgressByType(CharacterParameterUpgradeType.HealthMax)
                        .ParameterByLevel.Take(parameterLevel - 1).Sum();
                    
                    liveComponent.HealthContainer = new HealthContainer_Decorator(liveComponent.HealthContainer, addedHealth);
                    return;
                
                case CharacterParameterUpgradeType.MovementSpeedMax:

                    return;
            }
        }
    }
}