using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Masocube : PhysicalObject
{
	public int massCount;
	public AnimationCurve GravityCurve;
	public float terminalVel;

	private Vector3 StartPos;
	private Quaternion StartRot;

	float resetTimer;

	new private void Start()
	{
		StartPos = transform.position;
		StartRot = transform.rotation;
	}
	private void FixedUpdate()
	{
		RBody.mass = massCount > 0 ? 2 : 10;

		if (massCount > 0f)
			massCount--;

		RBody.AddForce(GravityCurve.Evaluate(RBody.velocity.y) * Gravity);
		if(Mathf.Abs((transform.position - StartPos).sqrMagnitude) > 7.5f)
		{
			resetTimer += Time.deltaTime;
		}

		if(resetTimer > 15f)
		{
			transform.position = StartPos;
			transform.rotation = StartRot;
		}
		//RBody.useGravity
		//if (RBody.velocity.y < terminalVel)
		//RBody.velocity = new Vector3(RBody.velocity.x, terminalVel, RBody.velocity.z);
		//Debug.Log(GravityCurve.Evaluate(RBody.velocity.y) * Gravity);
	}

	public override void TakeDamage(ref DamageInstance damageInstance)
	{
		resetTimer = 0f;
		massCount = damageInstance.hitStun;
		RBody.mass = 2;
		StartCoroutine(TakeKnockback(((damageInstance.knockbackDirection.x * damageInstance.origin.transform.right) + (damageInstance.knockbackDirection.y * damageInstance.origin.transform.up) + (damageInstance.knockbackDirection.z * damageInstance.origin.transform.forward)).normalized * damageInstance.knockbackStrength * RBody.mass, damageInstance));
		//Debug.Log(((damageInstance.knockbackDirection.x * damageInstance.origin.transform.right) + (damageInstance.knockbackDirection.y * damageInstance.origin.transform.up) + (damageInstance.knockbackDirection.z * damageInstance.origin.transform.forward)) * damageInstance.knockbackStrength);
	}

	IEnumerator TakeKnockback(Vector3 knockback ,DamageInstance damageInstance) 
	{
		RBody.velocity *= 0;
		Vector3 startPos = transform.position;
		transform.position = Vector3.Lerp(startPos, damageInstance.origin.transform.position + damageInstance.origin.transform.forward, 0.0f) + (Random.insideUnitSphere * 0.1f);
		yield return new WaitForSeconds(0.01f);
		RBody.velocity *= 0;
		transform.position = Vector3.Lerp(startPos, damageInstance.origin.transform.position + damageInstance.origin.transform.forward, 0.0f) + (Random.insideUnitSphere * 0.1f);
		yield return new WaitForSeconds(0.01f);
		RBody.velocity *= 0;
		transform.position = Vector3.Lerp(startPos, damageInstance.origin.transform.position + Vector3.Lerp(damageInstance.origin.transform.forward, knockback, 0.25f), 0.1f) + (Random.insideUnitSphere * 0.1f);
		yield return new WaitForSeconds(0.01f);
		RBody.velocity *= 0;
		RBody.AddForce(knockback, ForceMode.Impulse);
	}
}