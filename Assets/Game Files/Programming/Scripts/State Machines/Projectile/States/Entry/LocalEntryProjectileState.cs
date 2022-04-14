using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "ProjectileState/Entry/Local")]
public class LocalEntryProjectileState : ProjectileState
{
	public ProjectileState TravelState;
	public override void OnEnter(ProjectileObject projectileObject)
	{
		
	}

	public override void OnExit(ProjectileObject projectileObject)
	{
		
	}

	public override void OnFixedUpdate(ProjectileObject projectileObject)
	{
		
	}

	public override void OnUpdate(ProjectileObject projectileObject)
	{
		
	}

	public override void HandleState(ProjectileObject projectileObject)
	{
		if (projectileObject.CurrentFrame > MaxTime)
			projectileObject.StateMachine.ChangeState(TravelState);
	}
}
