using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookTarget : MonoBehaviour
{
	public Transform target;
	[Range(0,1)]
	public float lerpAmount;

	private void Update()
	{
		if(target)
			transform.position = Vector3.Slerp(transform.position, target.position, lerpAmount);
	}

	private void OnValidate()
	{
		Update();
	}
}
