using Arenar.PreferenceSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using TakeTop.PreferenceSystem;
using UnityEngine;
using Zenject;

namespace Arenar.Character
{
    public class PlayerCharacterSkillUpgradeService : ICharacterSkillUpgradeService
    {
        public event Action OnUpgradeScoreCountChange;
        
        
        private IPreferenceManager preferenceManager;
        
        private CharacterParametersUpgradeData characterParametersUpgradeData;
        private ICharacterEntity playerCharacterEntity;
        private Dictionary<CharacterSkillUpgradeType, int> characterUpgradeParametersLevels;
        
        private CharacterSkillUpgradePreferenceData characterSkillUpgradePreferenceData;
        
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
        public void Construct(IPreferenceManager preferenceManager, CharacterParametersUpgradeData characterParametersUpgradeData)
        {
            this.characterParametersUpgradeData = characterParametersUpgradeData;
            this.preferenceManager = preferenceManager;
        }
        
        public void InitializeCharacter(ICharacterEntity characterEntity)
        {
            playerCharacterEntity = characterEntity;
            characterSkillUpgradePreferenceData = preferenceManager.LoadValue<CharacterSkillUpgradePreferenceData>();
            if (characterSkillUpgradePreferenceData == null)
            {
                characterSkillUpgradePreferenceData = new CharacterSkillUpgradePreferenceData();
                preferenceManager.SaveValue(characterSkillUpgradePreferenceData);
            }
            
            UpgradeScore = characterSkillUpgradePreferenceData.upgradeScore;

            characterUpgradeParametersLevels = new Dictionary<CharacterSkillUpgradeType, int>();
            foreach (CharacterSkillUpgradeType skill in Enum.GetValues(typeof(CharacterSkillUpgradeType)))
            {
                if (skill == CharacterSkillUpgradeType.None)
                    continue;

                int parameterLevel = characterSkillUpgradePreferenceData.characterSkillUpgradeData[skill];
                characterUpgradeParametersLevels.Add(skill, parameterLevel);
                
                LoadCharacterParameters(skill, parameterLevel);
            }
        }
        
        public int GetUpgradeSkillLevel(CharacterSkillUpgradeType skillUpgradeType)
        {
            if (characterUpgradeParametersLevels == null || characterUpgradeParametersLevels.Count == 0)
                return -1;
            
            return characterUpgradeParametersLevels[skillUpgradeType];
        }
        
        public float GetUpgradeSkillValueByLevel(CharacterSkillUpgradeType skillUpgradeType, int level = -1)
        {
            if (level < 0)
                return 0;
            
            return characterParametersUpgradeData
                .GetParameterProgressByType(CharacterSkillUpgradeType.HealthMax)
                .ParameterByLevel.Take(level).Sum();
        }
        
        public bool CanUpgradeSkill(CharacterSkillUpgradeType skillUpgradeType, int level = - 1)
        {
            if (UpgradeScore == 0)
                return false;

            return !IsMaxLevel(skillUpgradeType, level);
        }
        
        public bool IsMaxLevel(CharacterSkillUpgradeType skillUpgradeType, int level = -1)
        {
            if (level < 0)
                return false;
            
            int levelMax = characterParametersUpgradeData
                .GetParameterProgressByType(skillUpgradeType)
                .ParameterByLevel.Length;
            
            return (level >= levelMax - 1);
        }

        public bool TryUpgradeSkill(CharacterSkillUpgradeType skillUpgradeType)
        {
            if (!CanUpgradeSkill(skillUpgradeType))
                return false;

            characterUpgradeParametersLevels[skillUpgradeType]++;
            
            float addedValue = characterParametersUpgradeData
                .GetParameterProgressByType(skillUpgradeType)
                .ParameterByLevel[characterUpgradeParametersLevels[skillUpgradeType]];

            switch (skillUpgradeType)
            {
                case CharacterSkillUpgradeType.DamageMax:
                    if (playerCharacterEntity.TryGetCharacterComponent<ICharacterAttackComponent>(out var attackComponentComponent))
                        attackComponentComponent.CharacterDamage += (int) addedValue;

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

            upgradeScore--;
            characterSkillUpgradePreferenceData.upgradeScore = upgradeScore;
            characterSkillUpgradePreferenceData.characterSkillUpgradeData[skillUpgradeType] = characterUpgradeParametersLevels[skillUpgradeType];
            
            preferenceManager.SaveValue(characterSkillUpgradePreferenceData);

            return true;
        }
        
        private void LoadCharacterParameters(CharacterSkillUpgradeType skillUpgradeType, int parameterLevel)
        {
            if (parameterLevel < 0)
                return;
            
            switch (skillUpgradeType)
            {
                case CharacterSkillUpgradeType.DamageMax:
                    if (!playerCharacterEntity.TryGetCharacterComponent<ICharacterAttackComponent>(out var attackComponentComponent))
                        return;
                    
                    int addedDamage = characterParametersUpgradeData
                        .GetParameterProgressByType(CharacterSkillUpgradeType.DamageMax)
                        .ParameterByLevel.Take(parameterLevel).Sum();
                    attackComponentComponent.CharacterDamage += (int)addedDamage;
                    return;
                
                case CharacterSkillUpgradeType.HealthMax:
                    if (!playerCharacterEntity.TryGetCharacterComponent<ICharacterLiveComponent>(out var liveComponent))
                        return;

                    int addedHealth = characterParametersUpgradeData
                        .GetParameterProgressByType(CharacterSkillUpgradeType.HealthMax)
                        .ParameterByLevel.Take(parameterLevel).Sum();
                    
                    liveComponent.HealthContainer = new HealthContainer_Decorator(liveComponent.HealthContainer, addedHealth);
                    return;
                
                case CharacterSkillUpgradeType.MovementSpeedMax:
                    if (!playerCharacterEntity.TryGetCharacterComponent<ICharacterMovementComponent>(out var movementComponent))
                        return;
                    
                    int addedSpeed = characterParametersUpgradeData
                        .GetParameterProgressByType(CharacterSkillUpgradeType.MovementSpeedMax)
                        .ParameterByLevel.Take(parameterLevel).Sum();
                    movementComponent.MovementContainer = new MovementContainer_SpeedDecorator(movementComponent.MovementContainer, addedSpeed);
                    return;
                
                default:
                    Debug.LogError("Unknown character upgrade type: " + skillUpgradeType);
                    return;
            }
        }
    }
}