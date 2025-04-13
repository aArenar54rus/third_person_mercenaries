namespace Arenar.Character
{
	public class MovementContainer_SpeedDecorator : MovementContainer
	{
		public readonly MovementContainer originalMovementContainer;
		private readonly float addedSpeed;

		
		public override float MovementSpeed
		{
			get => originalMovementContainer.MovementSpeed + addedSpeed;
			set => originalMovementContainer.MovementSpeed = value;
		}
		

		public MovementContainer_SpeedDecorator(MovementContainer originalContainer, float addedSpeed)
		{
			originalMovementContainer = originalContainer;
			this.addedSpeed = addedSpeed;
		}
	}
}