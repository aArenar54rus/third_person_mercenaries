namespace Arenar.Character
{
	public class HealthContainer_Decorator : HealthContainer
	{
		public readonly HealthContainer healthContainer;
		private readonly int addedHealth;


		public override int HealthMax
		{
			get => healthContainer.HealthMax + addedHealth;
			set => healthContainer.HealthMax = value;
		}

		public override int Health
		{
			get => healthContainer.Health;
			set => healthContainer.Health = value;
		}
		
		
		public HealthContainer_Decorator(HealthContainer healthContainer, int addedHealth)
		{
			this.healthContainer = healthContainer;
			this.addedHealth = addedHealth;
			
			float percent = (float)healthContainer.Health / healthContainer.HealthMax;
			Health = (int)(HealthMax * percent);
		}
	}
}