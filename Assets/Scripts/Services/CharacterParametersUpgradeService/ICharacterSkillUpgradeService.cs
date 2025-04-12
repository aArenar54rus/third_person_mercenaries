using System;


namespace Arenar.Character
{
	public interface ICharacterSkillUpgradeService
	{
		event Action OnUpgradeScoreCountChange;
		
		
		int UpgradeScore { get; set; }


		void InitializeCharacter(ICharacterEntity characterEntity);

		int GetUpgradeSkillLevel(CharacterSkillUpgradeType skillUpgradeType);

		bool IsMaxLevel(CharacterSkillUpgradeType skillUpgradeType, int level = -1);
		
		float GetUpgradeSkillValueByLevel(CharacterSkillUpgradeType skillUpgradeType, int level = - 1);
        
		bool CanUpgradeSkill(CharacterSkillUpgradeType skillUpgradeType, int level = - 1);
		
		bool TryUpgradeSkill(CharacterSkillUpgradeType skillUpgradeType);
	}
}