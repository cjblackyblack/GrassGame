using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactBox : Hitbox //THIS IS A HITBOX THAT PASSES ITS DAMAGE RECEIEVED UPWARDS 
{
	public override CombatBox CheckOverlap(HitboxData hitboxData)
	{
        gameObject.layer = LayerMask.NameToLayer("Hidden");
        PhysicsExtensions.OverlapColliderNonAlloc(Collider, HitColliders, EntityManager.Instance.Hittable);
        gameObject.layer = LayerMask.NameToLayer("Hitbox");
        return ProcessValidHitboxes(hitboxData);
    }

    public override void CheckOverlapGeo()
    {
        PhysicsExtensions.OverlapColliderNonAlloc(Collider, HitColliders, EntityManager.Instance.GeoLayers);
    }

    public override PhysicalObjectTangibility ProcessHitAction(ref DamageInstance damageInstance)
	{
		switch (CurrentBoxTangibility)
        {
            case PhysicalObjectTangibility.Normal:
                {

                }
                break;
            case PhysicalObjectTangibility.Armor:
                {

                }
                break;
            case PhysicalObjectTangibility.Guard:
                {

                }
                break;
            case PhysicalObjectTangibility.Invincible:
                {

                }
                break;
            case PhysicalObjectTangibility.Intangible:
                {

                }
                break;
        }

		SourceObject.TakeDamage(ref damageInstance);//for now just always sending upward, there may be future instances where the hurtbox has its own behaviour before sending information upward / poise or some shit too 
        return CurrentBoxTangibility;
	}

    public override bool ProcessHitReaction(PhysicalObjectTangibility hitTangibility, ref DamageInstance damageInstance)
    {
        switch (hitTangibility)
        {
            case PhysicalObjectTangibility.Normal:
                {

                }
                break;
            case PhysicalObjectTangibility.Armor:
                {

                }
                break;
            case PhysicalObjectTangibility.Guard:
                {

                }
                break;
            case PhysicalObjectTangibility.Invincible:
                {

                }
                break;
            case PhysicalObjectTangibility.Intangible:
                {

                }
                break;

        }
        return false;
    }
}