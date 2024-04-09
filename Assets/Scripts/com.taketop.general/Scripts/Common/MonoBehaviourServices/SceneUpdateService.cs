using System;
using System.Collections.Generic;
using UnityEngine;


namespace Module.General
{
	public class SceneUpdateService : MonoBehaviour, ISceneUpdateService
	{
		public event Action OnUpdateEvent;
		public event Action OnPhysicsUpdateEvent;
		public event Action OnLateUpdateEvent;


		private readonly List<IUpdatable> allUpdatableObjects = new List<IUpdatable>();
		private readonly Queue<IUpdatable> queueToAddUpdate = new Queue<IUpdatable>();
		private readonly Queue<IUpdatable> queueToRemoveUpdate = new Queue<IUpdatable>();


		private int updatableObjectCount = 0;


		public void AddUpdatableObject(IUpdatable value)
		{
			if (allUpdatableObjects.Contains(value)
				|| queueToAddUpdate.Contains(value))
			{
				return;
			}

			queueToAddUpdate.Enqueue(value);
		}

		public void RemoveUpdatableObject(IUpdatable value)
		{
			if (!allUpdatableObjects.Contains(value)
				|| queueToRemoveUpdate.Contains(value)
				|| !queueToAddUpdate.Contains(value))
			{
				return;
			}

			queueToRemoveUpdate.Enqueue(value);
		}

		private void AddUpdateObjectsProcessing()
		{
			allUpdatableObjects.AddRange(queueToAddUpdate);
			queueToAddUpdate.Clear();
			updatableObjectCount = allUpdatableObjects.Count;
		}

		private void RemoveUpdateObjectProcessing()
		{
			foreach (IUpdatable removeObject in queueToRemoveUpdate)
			{
				if (allUpdatableObjects.Contains(removeObject))
				{
					allUpdatableObjects.Remove(removeObject);
				}
				else if (queueToAddUpdate.Contains(removeObject))
				{
					var temp = new List<IUpdatable>(queueToAddUpdate.ToArray());
					queueToAddUpdate.Clear();
					temp.Remove(removeObject);
					int queueCount = temp.Count;
					for (int i = 0; i < queueCount; i++)
					{
						queueToAddUpdate.Enqueue(temp[i]);
					}
				}
			}
		}

		private void FixedUpdate()
		{
			for (var i = 0; i < updatableObjectCount; i++)
			{
				IUpdatable updateableObject = allUpdatableObjects[i];
				if (updateableObject.IsActive)
				{
					updateableObject.OnPhysicsUpdate();
				}
			}

			OnPhysicsUpdateEvent?.Invoke();
		}

		private void Update()
		{
			for (var i = 0; i < updatableObjectCount; i++)
			{
				IUpdatable updateableObject = allUpdatableObjects[i];
				if (updateableObject.IsActive)
				{
					updateableObject.OnUpdate();
				}
			}

			OnUpdateEvent?.Invoke();
		}

		private void LateUpdate()
		{
			for (var i = 0; i < updatableObjectCount; i++)
			{
				IUpdatable updateableObject = allUpdatableObjects[i];
				if (updateableObject.IsActive)
				{
					updateableObject.OnLateUpdate();
				}
			}

			OnLateUpdateEvent?.Invoke();
			RemoveUpdateObjectProcessing();
			AddUpdateObjectsProcessing();
		}

	}
}
