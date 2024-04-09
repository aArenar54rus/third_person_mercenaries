namespace Module.General
{
	public interface IUpdatable
	{
		bool IsActive { get; set; }


		void OnPhysicsUpdate();
		void OnUpdate();
		void OnLateUpdate();
	}
}
