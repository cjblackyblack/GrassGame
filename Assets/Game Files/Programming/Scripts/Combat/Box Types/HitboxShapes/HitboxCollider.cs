using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxCollider : Hitbox
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
}