using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PoolManager : Singleton<PoolManager>
{

	Dictionary<string, GameObject> poolPrefabs = new Dictionary<string, GameObject>();
	Dictionary<string, GameObject> poolParents = new Dictionary<string, GameObject>();
	Dictionary<string, Queue<ObjectInstance>> poolFreeDictionary = new Dictionary<string, Queue<ObjectInstance>>();
	Dictionary<string, Dictionary<int, ObjectInstance>> poolUsedDictionary = new Dictionary<string, Dictionary<int, ObjectInstance>>();
	public GameObject testObj;

	public override void Start()
	{
		base.Start();
		CreatePool("test", testObj, 10);
	}
	public void CreatePool(string poolKey, GameObject prefab, int poolSeedSize)
	{
		if (!poolPrefabs.ContainsKey(poolKey))
		{
			poolPrefabs.Add(poolKey, prefab);
			if (!prefab.GetComponent<PoolObject>())
			{
				prefab.AddComponent<PoolObject>();
			}
			prefab.GetComponent<PoolObject>().Key = poolKey;

			poolFreeDictionary.Add(poolKey, new Queue<ObjectInstance>());
			poolUsedDictionary.Add(poolKey, new Dictionary<int, ObjectInstance>());

			GameObject poolParent;
			if (poolParents.ContainsKey(poolKey))
			{
				poolParent = poolParents[poolKey];
			}
			else
			{
				poolParent = new GameObject(poolKey + " pool");
				poolParent.transform.parent = transform;
				poolParents.Add(poolKey, poolParent);
			}

			for (int i = 0; i < poolSeedSize; i++)
			{
				ObjectInstance newObject = new ObjectInstance(Instantiate(prefab) as GameObject);
				poolFreeDictionary[poolKey].Enqueue(newObject);
				newObject.SetParent(poolParent.transform);
			}
		}
	}

	public GameObject GetObject(string poolKey, Vector3 position, Quaternion rotation)
	{
		if (poolPrefabs.ContainsKey(poolKey))
		{
			if (poolFreeDictionary[poolKey].Count > 0)
			{
				ObjectInstance obj = poolFreeDictionary[poolKey].Dequeue();
				poolUsedDictionary[poolKey].Add(obj.gameObject.GetInstanceID(), obj);
				obj.GetObject(position, rotation);
				return obj.gameObject;
			}
			else
			{
				ObjectInstance newObject = new ObjectInstance(Instantiate(poolPrefabs[poolKey]) as GameObject);
				poolUsedDictionary[poolKey].Add(newObject.gameObject.GetInstanceID(), newObject);
				newObject.SetParent(poolParents[poolKey].transform);
				newObject.GetObject(position, rotation);
				return newObject.gameObject;
			}
		}
		return null;
	}

	public void ReturnObjectToQueue(GameObject gameObject)
	{
		if (gameObject.GetComponent<PoolObject>())
		{
			string poolKey = gameObject.GetComponent<PoolObject>().Key;
			gameObject.SetActive(false);
			ObjectInstance obj = poolUsedDictionary[poolKey][gameObject.GetInstanceID()];
			poolUsedDictionary[poolKey].Remove(gameObject.GetInstanceID());
			poolFreeDictionary[poolKey].Enqueue(obj);
		}
	}

	private void Update()
	{
		if (Keyboard.current.tabKey.wasPressedThisFrame)
			GetObject("test", PlayerManager.Instance.PlayerObject.transform.position + Random.insideUnitSphere + Vector3.up, Random.rotation);
	}

	private class ObjectInstance
	{
		public GameObject gameObject;
		Transform transform;
		PoolObject poolObjectScript;

		public ObjectInstance(GameObject objectInstance)
		{
			gameObject = objectInstance;
			transform = gameObject.transform;
			gameObject.SetActive(false);
			poolObjectScript = gameObject.GetComponent<PoolObject>();
		}

		public void GetObject(Vector3 position, Quaternion rotation)
		{
			gameObject.SetActive(true);
			transform.position = position;
			transform.rotation = rotation;
			poolObjectScript.Enable();
		}

		public void SetParent(Transform parent)
		{
			transform.parent = parent;
		}
	}
}