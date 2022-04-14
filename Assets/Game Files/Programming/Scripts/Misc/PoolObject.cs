using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour
{
	public string Key;
	bool resetting;
	float resetTimer;
	public virtual void Enable()
	{
		//Debug.Log("PoolObject Awake");
	}

	private void Update()
	{
		if (resetting)
		{
			resetTimer -= Time.deltaTime;
			if (resetTimer <= 0)
				ReturnObject();
		}

	}
	protected void ReturnObjectInSeconds(float time)
	{
		
	}

	protected void ReturnObject()
	{
		PoolManager.Instance.ReturnObjectToQueue(gameObject);
	}
}