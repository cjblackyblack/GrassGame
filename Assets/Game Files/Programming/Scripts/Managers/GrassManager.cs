using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassManager : Singleton<GrassManager>
{
	public static List<GrassObject> grassObjects = new List<GrassObject>();


	private void Start()
	{
		//grassObjects = new List<GrassObject>();
	}
	private void FixedUpdate()
	{
		for (int i = 0; i < grassObjects.Count; i++)
			grassObjects[i].OnFixedUpdate();
	}
}
