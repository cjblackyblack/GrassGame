using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraWorldUpOverride : MonoBehaviour
{
	public Transform Target;
	[Range(0, 1)]
	public float LerpAmount;

	private void Update()
	{
		if(Target)
			transform.rotation = Quaternion.Lerp(transform.rotation, Target.rotation, LerpAmount);	
	}
}
