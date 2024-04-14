using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMA.Core
{
    /// <summary>
    /// An abstract class that provides base functionalities of a singleton for its derived classes
    /// </summary>
    /// <typeparam name="T">The type of singleton instance</typeparam>
    public abstract class AbstractSingleton<T> : MonoBehaviour where T : Component
    {
        static T s_Instance;

		bool initialized;

		protected virtual bool dontDestroy => false;

        /// <summary>
        /// static Singleton instance
        /// </summary>
        public static T Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = FindObjectOfType<T>();
                    if (s_Instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = typeof(T).Name;
                        s_Instance = obj.AddComponent<T>();
					}

					if (s_Instance is AbstractSingleton<T> instance)
					{
						instance.Initialize();
						instance.initialized = true;
					}

					//Debug.LogError("Access " + (typeof(T)));
				}

                return s_Instance;
            }
        }

        protected void Awake()
        {
			//Debug.LogError("Awake " + this + " " + initialized + " " + (s_Instance == null) + " " + (s_Instance == this), this);

            if (s_Instance == null)
            {
                s_Instance = this as T;
				if (!initialized) Initialize();
				initialized = true;
            }
            else if (s_Instance != this)
            {
                Destroy(gameObject);
				return;
            }

			if (dontDestroy) DontDestroyOnLoad(gameObject);
        }

		protected virtual void Initialize ()
		{

		}
    }
}