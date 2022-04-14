using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetableObject : MonoBehaviour
{
	public TangibleObject SourceObject;
	[SerializeField]
	private float DefaultWeight;
	public float Weight;
	[SerializeField]
	private float DefaultRadius;
	public float Radius;

	private void Start()
	{
		SourceObject = GetComponent<TangibleObject>();
		if (SourceObject == null)
			SourceObject = GetComponentInParent<TangibleObject>();
	}
	public void SetWeight(float weight)
	{
		Weight = weight;
	}

	public void ResetWeight()
	{
		Weight = DefaultWeight;
	}

	public void SetRadius(float weight)
	{
		Radius = weight;
	}

	public void ResetRadius()
	{
		Radius = DefaultRadius;
	}
}