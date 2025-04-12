using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Arenar.Character
{
    public class PlayerCharacterSkillUpgradeService : ICharacterSkillUpgradeService
    {
        private const string PLAYER_NAME = "Player";
        
        
        public event Action OnUpgradeScoreCountChange;
        
        
        private CharacterParametersUpgradeData characterParametersUpgradeData;
        private ICharacterEntity playerCharacterEntity;
        private Dictionary<CharacterSkillUpgradeType, int> characterUpgradeParametersLevels;
        
        private int upgradeScore;
        
        
        public int UpgradeScore
        {
            get => upgradeScore;
            set
            {
                upgradeScore = Mathf.Clamp(value, 0, int.MaxValue);
                OnUpgradeScoreCountChange?.Invoke();
            }
        }


        [Inject]
        public void Construct(CharacterParametersUpgradeData characterParametersUpgradeData)
        {
            this.characterParametersUpgradeData = characterParametersUpgradeData;
        }
        
        public void InitializeCharacter(ICharacterEntity characterEntity)
        {
            UpgradeScore = 0;

            characterUpgradeParametersLevels = new Dictionary<CharacterSkillUpgradeType, int>();
            foreach (CharacterSkillUpgradeType skill in Enum.GetValues(typeof(CharacterSkillUpgradeType)))
            {
                if (skill == CharacterSkillUpgradeType.None)
                    continue;

                string parameterKey = PLAYER_NAME + "_" + skill.ToString();
                int parameterLevel =  PlayerPrefs.GetInt(parameterKey, 0);
                characterUpgradeParametersLevels.Add(skill, parameterLevel);
                
                LoadCharacterParameters(skill, parameterLevel);
            }
        }
        
        public int GetUpgradeSkillLevel(CharacterSkillUpgradeType skillUpgradeType)
        {
            return characterUpgradeParametersLevels[skillUpgradeType];
        }
        
        public float GetUpgradeSkillValueByLevel(CharacterSkillUpgradeType skillUpgradeType, int level = -1)
        {
            if (level < 0)
                level = characterUpgradeParametersLevels[skillUpgradeType];
            
            return characterParametersUpgradeData
                .GetParameterProgressByType(CharacterSkillUpgradeType.HealthMax)
                .ParameterByLevel.Take(level - 1).Sum();
        }
        
        public bool CanUpgradeSkill(CharacterSkillUpgradeType skillUpgradeType, int level = - 1)
        {
            if (UpgradeScore == 0)
                return false;

            return !IsMaxLevel(skillUpgradeType, level);
        }
        
        public bool IsMaxLevel(CharacterSkillUpgradeType skillUpgradeType, int level = -1)
        {
            int levelMax = characterParametersUpgradeData
                .GetParameterProgressByType(CharacterSkillUpgradeType.HealthMax)
                .ParameterByLevel.Length;

            if (level < 0)
                level = characterUpgradeParametersLevels[skillUpgradeType];
            
            return (level >= levelMax);
        }

        public bool TryUpgradeSkill(CharacterSkillUpgradeType skillUpgradeType)
        {
            if (!CanUpgradeSkill(skillUpgradeType))
                return false;
            
            string parameterKey = PLAYER_NAME + "_" + skillUpgradeType.ToString();
            int parameterLevel =  PlayerPrefs.GetInt(parameterKey, 0);
            float addedValue = characterParametersUpgradeData
                .GetParameterProgressByType(skillUpgradeType)
                .ParameterByLevel[parameterLevel];

            switch (skillUpgradeType)
            {
                case CharacterSkillUpgradeType.DamageMax:
                    if (playerCharacterEntity.TryGetCharacterComponent<ICharacterAttackComponent>(out var attackComponentComponent))
                        attackComponentComponent.CharacterDamage += (int)addedValue;
                    break;
                
                case CharacterSkillUpgradeType.HealthMax:
                    if (playerCharacterEntity.TryGetCharacterComponent<ICharacterLiveComponent>(out var liveComponent))
                        liveComponent.HealthContainer = new HealthContainer_Decorator(liveComponent.HealthContainer, (int)addedValue);
                    break;
                
                case CharacterSkillUpgradeType.MovementSpeedMax:
                    if (playerCharacterEntity.TryGetCharacterComponent<ICharacterMovementComponent>(out var movementComponent))
                        movementComponent.MovementContainer = new MovementContainer_SpeedDecorator(movementComponent.MovementContainer, addedValue);
                    break;
                
                default:
                    Debug.LogError("Unknown character upgrade type: " + skillUpgradeType);
                    return false;
            }
            
            PlayerPrefs.SetInt(parameterKey, ++parameterLevel);
            return true;
        }
        
        private void LoadCharacterParameters(CharacterSkillUpgradeType skillUpgradeType, int parameterLevel)
        {
            if (parameterLevel == 0)
                return;
            
            switch (skillUpgradeType)
            {
                case CharacterSkillUpgradeType.DamageMax:
                    if (!playerCharacterEntity.TryGetCharacterComponent<ICharacterAttackComponent>(out var attackComponentComponent))
                        return;
                    
                    int addedDamage = characterParametersUpgradeData
                        .GetParameterProgressByType(CharacterSkillUpgradeType.DamageMax)
                        .ParameterByLevel.Take(parameterLevel - 1).Sum();
                    attackComponentComponent.CharacterDamage += (int)addedDamage;
                    return;
                
                case CharacterSkillUpgradeType.HealthMax:
                    if (!playerCharacterEntity.TryGetCharacterComponent<ICharacterLiveComponent>(out var liveComponent))
                        return;

                    int addedHealth = characterParametersUpgradeData
                        .GetParameterProgressByType(CharacterSkillUpgradeType.HealthMax)
                        .ParameterByLevel.Take(parameterLevel - 1).Sum();
                    
                    liveComponent.HealthContainer = new HealthContainer_Decorator(liveComponent.HealthContainer, addedHealth);
                    return;
                
                case CharacterSkillUpgradeType.MovementSpeedMax:
                    if (!playerCharacterEntity.TryGetCharacterComponent<ICharacterMovementComponent>(out var movementComponent))
                        return;
                    
                    int addedSpeed = characterParametersUpgradeData
                        .GetParameterProgressByType(CharacterSkillUpgradeType.MovementSpeedMax)
                        .ParameterByLevel.Take(parameterLevel - 1).Sum();
                    movementComponent.MovementContainer = new MovementContainer_SpeedDecorator(movementComponent.MovementContainer, addedSpeed);
                    return;
                
                default:
                    Debug.LogError("Unknown character upgrade type: " + skillUpgradeType);
                    return;
            }
        }
    }
}