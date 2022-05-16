using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Masocube : PhysicalObject
{
	public int massCount;

	private void FixedUpdate()
	{
		RBody.mass = massCount > 0 ? 2 : 10;

		if (massCount > 0)
			massCount--;
	}
	public override void TakeDamage(ref DamageInstance damageInstance)
	{
		massCount = damageInstance.hitStun;
		StartCoroutine(TakeKnockback(((damageInstance.knockbackDirection.x * damageInstance.origin.transform.right) + (damageInstance.knockbackDirection.y * damageInstance.origin.transform.up) + (damageInstance.knockbackDirection.z * damageInstance.origin.transform.forward)) * damageInstance.knockbackStrength * RBody.mass));
		//Debug.Log(((damageInstance.knockbackDirection.x * damageInstance.origin.transform.right) + (damageInstance.knockbackDirection.y * damageInstance.origin.transform.up) + (damageInstance.knockbackDirection.z * damageInstance.origin.transform.forward)) * damageInstance.knockbackStrength);
	}

	IEnumerator TakeKnockback(Vector3 knockback) 
	{
		RBody.velocity *= 0;
		Vector3 startPos = transform.position;
		transform.position = startPos + (Random.insideUnitSphere * 0.1f);
		yield return new WaitForSeconds(0.01f);
		transform.position = startPos + (Random.insideUnitSphere * 0.1f);
		yield return new WaitForSeconds(0.01f);
		transform.position = startPos;
		yield return new WaitForSeconds(0.01f);
		RBody.AddForce(knockback, ForceMode.Impulse);
	}
}