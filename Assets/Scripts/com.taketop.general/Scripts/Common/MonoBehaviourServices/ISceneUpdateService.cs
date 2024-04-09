using System;


namespace Module.General
{
	public interface ISceneUpdateService
	{
		event Action OnPhysicsUpdateEvent;
		event Action OnUpdateEvent;
		event Action OnLateUpdateEvent;

		void AddUpdatableObject(IUpdatable value);
		void RemoveUpdatableObject(IUpdatable value);
	}
}
