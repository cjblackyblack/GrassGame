using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "StatusEffect/Poison Effect")]
public class PoisonEffect : StatusEffect
{
	public int damage;

	public override void OnEnter(SmartObject smartObject, TangibleObject origin)
	{
		OnTick(smartObject, origin);
	}

	public override void OnTick(SmartObject smartObject, TangibleObject origin)
	{
		DamageInstance damageInstance = new DamageInstance(origin, Random.Range(-1000000, 1000000), null, true, 0, 0, 0,HitstopType.ReceiverOnly ,BreakthroughType.None, KnockbackType.Knockback, 0, Vector3.zero, false, false, true, true);
		smartObject.TakeDamage(ref damageInstance);
	}
}