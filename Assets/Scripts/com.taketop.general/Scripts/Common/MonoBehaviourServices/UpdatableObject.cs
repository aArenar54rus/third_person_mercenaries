using UnityEngine;


namespace Module.General
{
	public class UpdatableObject : MonoBehaviour, IUpdatable
	{
		public bool IsActive { get;  set; } = true;


		public virtual void OnUpdate() {}


		public virtual void OnPhysicsUpdate() {}


		public virtual void OnLateUpdate() {}
	}
}
