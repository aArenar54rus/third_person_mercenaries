using UnityEngine;


namespace Module.General
{
	public interface IObjectPool
	{
		T GetFromPool<T>(T prototype, Transform parent = null) where T : Component;
		T GetFromPool<T>(T prototype, Vector3 position, Quaternion rotation, Transform parent = null)
			where T : Component;
		GameObject GetFromPool(GameObject prototype, Transform parent = null);
		GameObject GetFromPool(GameObject prototype, Vector3 position,
							   Quaternion rotation, Transform parent = null);
		void ReturnToPool<T>(T clone) where T : Component;
		void ReturnToPool(GameObject clone);
	}
}
