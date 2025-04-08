namespace Arenar.Character
{
	public interface ICharacterParametersUpgradeService
	{
		public int UpgradeScore { get; set; }


		public void InitializeCharacter(ICharacterEntity characterEntity);
        
		public bool CanUpgrade(CharacterParameterUpgradeType parameterUpgradeType);
		
		public bool TryUpgrade(CharacterParameterUpgradeType parameterUpgradeType);
	}
}