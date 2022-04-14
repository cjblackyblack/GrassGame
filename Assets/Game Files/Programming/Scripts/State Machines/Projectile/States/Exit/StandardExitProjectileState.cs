using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "ProjectileState/Exit/Standard")]
public class StandardExitProjectileState : ProjectileState
{
	public override void OnEnter(ProjectileObject projectileObject)
	{
		projectileObject.CurrentFrame = -1;
		projectileObject.CurrentTime = -1;
		if (projectileObject.HurtFX != null)
			Instantiate(projectileObject.HurtFX, projectileObject.transform.position, Quaternion.identity);
	}

	public override void OnExit(ProjectileObject projectileObject)
	{

	}

	public override void OnFixedUpdate(ProjectileObject projectileObject)
	{
		if (projectileObject.CurrentFrame > MaxTime)
			Destroy(projectileObject.gameObject);
	}

	public override void OnUpdate(ProjectileObject projectileObject)
	{
		
	}

	public override void HandleState(ProjectileObject projectileObject)
	{

	}
}
