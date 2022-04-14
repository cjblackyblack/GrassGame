using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(-50)]
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour 
{
	public static T Instance { get; private set; }
	public virtual void Awake()
	{
		if (Instance != null && Instance != this as T)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this as T;
	}

	public virtual void Start()
	{

		DontDestroyOnLoad(gameObject);
	}

	public virtual void OnApplicationQuit()
	{
		Instance = null;
		Destroy(gameObject);
	}
}
//They're public so they're easy to set...
//Don't be stupid