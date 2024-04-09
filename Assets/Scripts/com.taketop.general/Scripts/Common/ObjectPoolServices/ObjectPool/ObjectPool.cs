using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;


namespace Module.General
{
	public class ObjectPool : MonoSingleton<ObjectPool>, IObjectPool
	{
		private class PoolCollection
		{
			public readonly Transform ParentContainer;
			public readonly List<PoolContainer> PassiveList = new List<PoolContainer>();
			public readonly List<PoolContainer> ActiveList = new List<PoolContainer>();

			public PoolCollection(Transform parentContainer)
			{
				ParentContainer = parentContainer;
			}
		}


		private class PoolContainer
		{
			private readonly GameObject gameObject;
			private readonly Dictionary<Type, Object> entityDictionary =
				new Dictionary<Type, Object>();
			private readonly IResettable[] resettables;

			public GameObject GameObject => gameObject;

			public PoolContainer(GameObject gameObject)
			{
				this.gameObject = gameObject;
				resettables = gameObject.GetComponentsInChildren<IResettable>();
			}


			public void ResetAll()
			{
				for (int i = 0; i < resettables.Length; i++)
				{
					resettables[i].ResetSettings();
				}
			}


			public bool TryGetEntity<T>(out T entity)
			{
				Type type = typeof(T);
				if (!entityDictionary.TryGetValue(type, out Object entityEssence))
				{
					entity = gameObject.GetComponent<T>();
					entityDictionary.Add(type, entity);
					return true;
				}

				if (entityEssence != null)
				{
					entity = (T)entityEssence;
					return true;
				}

				entity = default;
				return false;
			}
		}


		private readonly Dictionary<GameObject, int> identifiers = new Dictionary<GameObject, int>();
		private readonly Dictionary<int, PoolCollection> clones = new Dictionary<int, PoolCollection>();
		private readonly Queue<PoolCollection> endedCollections = new Queue<PoolCollection>();


		private int idCounter = -1;


		public T GetFromPool<T>(T prototype, Transform parent = null) where T : Component
		{
			return GetFromPool(prototype, Vector3.zero, Quaternion.identity, parent);
		}

		public T GetFromPool<T>(T prototype, Vector3 position, Quaternion rotation,
								Transform parent = null) where T : Component
		{
			GameObject gameObjectPrototype = prototype.gameObject;
			GetFromPool(ref gameObjectPrototype, out PoolContainer container);
			if (container.TryGetEntity(out T result))
			{
				Transform resultTransform = result.transform;
				resultTransform.SetParent(parent);
				resultTransform.position = position;
				resultTransform.rotation = rotation;
				resultTransform.localScale = Vector3.one;
				container.GameObject.SetActive(true);
				return result;
			}

			return null;
		}

		public GameObject GetFromPool(GameObject prototype, Transform parent = null)
		{
			GetFromPool(ref prototype, out PoolContainer container);
			Transform resultTransform = container.GameObject.transform;
			resultTransform.SetParent(parent);
			container.GameObject.SetActive(true);
			return container.GameObject;
		}

		public GameObject GetFromPool(GameObject prototype, Vector3 position, Quaternion rotation,
									  Transform parent = null)
		{
			GetFromPool(ref prototype, out PoolContainer container);
			Transform resultTransform = container.GameObject.transform;
			resultTransform.SetParent(parent);
			resultTransform.position = position;
			resultTransform.rotation = rotation;
			container.GameObject.SetActive(true);
			return container.GameObject;
		}

		public void ReturnToPool<T>(T clone) where T : Component
		{
			if (ReferenceEquals(clone, null))
			{
				return;
			}

			ReturnToPool(clone.gameObject);
		}

		public void ReturnToPool(GameObject clone)
		{
			clone.SetActive(false);
			if (!TryRemoveActiveContainer(ref clone, out PoolCollection poolCollection,
				out PoolContainer activeContainer))
			{
				return;
			}

			poolCollection.PassiveList.Add(activeContainer);
			activeContainer.GameObject.transform.SetParent(poolCollection.ParentContainer);
		}

		private void GetFromPool(ref GameObject prototype, out PoolContainer container)
		{
			PoolCollection poolCollection = GetCollection(ref prototype);

			if (!TryRemovePassiveContainer(ref poolCollection, out container))
			{
				InstantiateCloneContainer(ref prototype, out container);
			}

			container.ResetAll();
			poolCollection.ActiveList.Add(container);
			container.GameObject.transform.SetParent(null);
		}

		private PoolCollection GetCollection(ref GameObject prototype)
		{
			int id = GetIdentifier(prototype);
			return GetCollection(ref id);
		}

		private int GetIdentifier(GameObject prototype)
		{
			if (!identifiers.TryGetValue(prototype, out int id))
			{
				id = GenerateId();
				identifiers.Add(prototype, id);
			}

			return id;
		}

		private PoolCollection GetCollection(ref int id)
		{
			if (!clones.TryGetValue(id, out PoolCollection poolCollection))
			{
				Transform parentContainer = CreateParentContainer(id);
				poolCollection = new PoolCollection(parentContainer);
				clones.Add(id, poolCollection);
			}

			return poolCollection;
		}

		private bool TryRemovePassiveContainer(ref PoolCollection collection,
											   out PoolContainer cloneContainer)
		{
			if (collection.PassiveList.Count > 0)
			{
				cloneContainer = collection.PassiveList[0];
				collection.PassiveList.RemoveAt(0);

				if (collection.PassiveList.Count == 0 && !endedCollections.Contains(collection))
				{
					endedCollections.Enqueue(collection);
				}

				return true;
			}

			cloneContainer = null;
			return false;
		}

		private bool TryRemoveActiveContainer(ref GameObject clone, out PoolCollection collection,
											  out PoolContainer activeContainer)
		{
			activeContainer = null;
			collection = GetCollection(ref clone);
			for (int i = 0; i < collection.ActiveList.Count; i++)
			{
				PoolContainer activeClone = collection.ActiveList[i];
				if (activeClone.GameObject == clone)
				{
					activeContainer = activeClone;
					collection.ActiveList.RemoveAt(i);
					break;
				}
			}

			return activeContainer != null;
		}

		private void InstantiateCloneContainer(ref GameObject prototype, out PoolContainer cloneContainer)
		{
			GameObject clone = Instantiate(prototype);
			clone.name = prototype.name;
			clone.SetActive(false);
			int id = GetIdentifier(prototype);
			cloneContainer = new PoolContainer(clone);
			identifiers.Add(clone, id);
		}

		private Transform CreateParentContainer(int id)
		{
			GameObject containerGameObject = new GameObject("Container " + id);
			Transform containerTransform = containerGameObject.transform;
			containerTransform.SetParent(transform);
			containerTransform.localPosition = Vector3.zero;
			containerTransform.localRotation = Quaternion.identity;
			containerTransform.localScale = Vector3.one;
			return containerTransform;
		}

		private int GenerateId()
		{
			idCounter++;
			return idCounter;
		}
	}
}
