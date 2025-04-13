namespace Arenar.Character
{
	public interface ICharacterAimComponent : ICharacterComponent
	{
		bool IsAim { get; set; }
       
		float AimProgress { get; }
	}
}