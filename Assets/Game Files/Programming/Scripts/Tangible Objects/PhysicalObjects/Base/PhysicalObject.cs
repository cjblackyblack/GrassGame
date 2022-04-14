using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PhysicalObject : TangibleObject
{
	[PropertyOrder(-100)]
	public TargetableObject Target;
	public Rigidbody RBody => GetComponent<Rigidbody>();
	[FoldoutGroup("Variables/Time")]
	public float CurrentTime;
	[FoldoutGroup("Variables/Time")]
	public int CurrentFrame;
	[FoldoutGroup("Variables/Velocity")]
	public Vector3 Velocity;
	[FoldoutGroup("Variables/Velocity")]
	public float Friction;
	[FoldoutGroup("Variables/Velocity")]
	public float FrictionModifier;
	[FoldoutGroup("Variables/Velocity")]
	public Vector3 Gravity;

	public Vector3 CollisionNormal;
	public bool BounceBuffered;
	public bool CanBounce;
	private void OnCollisionEnter(Collision collision)
	{
		if (CanBounce)
		{
			//Debug.Log(collision.gameObject);
			BounceBuffered = true;
			CollisionNormal = collision.contacts[0].normal;
		}
	}
}