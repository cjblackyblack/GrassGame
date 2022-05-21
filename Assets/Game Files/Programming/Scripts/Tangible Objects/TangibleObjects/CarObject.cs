using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarObject : PhysicalObject
{
	public Animator Animator;
	public float ResetDistance;
	public float ResetTime;
	float resetTimer;
	public float forceMult;
	Vector3 StartPos;
	Quaternion StartRot;

	new private void Start()
	{

		StartPos = transform.position;
		StartRot = transform.rotation;
	}
	private void FixedUpdate()
	{
		//RBody.useGravity
		//if (RBody.velocity.y < terminalVel)
		//	RBody.velocity = new Vector3(RBody.velocity.x, terminalVel, RBody.velocity.z);
		//Debug.Log(GravityCurve.Evaluate(RBody.velocity.y) * Gravity);

		if (Mathf.Abs((transform.position - StartPos).sqrMagnitude) > ResetDistance)
		{
			resetTimer += Time.deltaTime;
		}

		if (resetTimer > ResetTime)
		{
			transform.position = StartPos;
			transform.rotation = StartRot;
		}
	}

	public override void TakeDamage(ref DamageInstance damageInstance)
	{
		resetTimer = 0;
		Animator.Play("Hit", 0, 0);
		RBody.velocity *= 0;
		RBody.AddForce(forceMult  * ((damageInstance.knockbackDirection.x * damageInstance.origin.transform.right) + (damageInstance.knockbackDirection.y * damageInstance.origin.transform.up) + (damageInstance.knockbackDirection.z * damageInstance.origin.transform.forward).normalized * damageInstance.knockbackStrength), ForceMode.Impulse);
	}
}
