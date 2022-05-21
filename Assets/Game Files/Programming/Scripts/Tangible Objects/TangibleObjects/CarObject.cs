using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarObject : TangibleObject
{
	public Animator Animator;
	public override void TakeDamage(ref DamageInstance damageInstance)
	{
		Animator.Play("Hit", 0, 0);
	}
}
