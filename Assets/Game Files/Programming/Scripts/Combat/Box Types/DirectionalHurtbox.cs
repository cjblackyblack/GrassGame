using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalHurtbox : Hurtbox
{
	public float DirectionalRequirement;
	public PhysicalObjectTangibility FrontTangibility;
	public PhysicalObjectTangibility BackTangibility;

	public override PhysicalObjectTangibility ProcessHitAction(ref DamageInstance damageInstance)//GOT HIT AND NEED TO DECIDE TO TELL PARENT OBJ OR HAVE BEHAVIOUR BECAUSE THIS SUPPORTS THAT TOO
	{
		if (Vector3.Dot(damageInstance.origin.transform.forward, SourceObject.transform.forward) > DirectionalRequirement)
		{
			CurrentBoxTangibility = BackTangibility;
			SourceObject.TakeDamage(ref damageInstance);//for now just always sending upward
			return BackTangibility;
		}
		if(FrontTangibility == PhysicalObjectTangibility.Guard)
			if(FlagsExtensions.HasFlag(damageInstance.breakthroughType, BreakthroughType.GuardPierce))
				SourceObject.TakeDamage(ref damageInstance);//for now just always sending upward
		CurrentBoxTangibility = FrontTangibility;
		return FrontTangibility;
	}
}