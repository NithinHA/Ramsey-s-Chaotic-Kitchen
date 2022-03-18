using UnityEngine;
using System.Collections;

namespace SingletonBase
{
	public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
	{
        private static T mInstance = null;
        
        public bool IsDontDestroyOnLoad = false;
		
        protected Singleton() { }

		public static T Instance
		{
			get
			{
				if (mInstance == null)
					mInstance = FindObjectOfType<T>();

				return mInstance;
			}
		}

		protected virtual void Awake()
		{
			if (IsDontDestroyOnLoad)
				DontDestroyOnLoad(gameObject);
		}

		protected virtual void Start()
		{
			T[] instance = FindObjectsOfType<T>();
			if (instance.Length > 1)
			{
				Debug.Log(gameObject.name + " has been destroyed because another object already has the same component.");
				Destroy(gameObject);
			}
		}

		protected virtual void OnDestroy()
		{
			if (this == mInstance)
				mInstance = null;
		}
	}
}