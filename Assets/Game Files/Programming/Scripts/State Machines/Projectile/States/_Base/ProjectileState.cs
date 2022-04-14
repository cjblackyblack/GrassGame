using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileState : ScriptableObject
{
	public int MaxTime;

	public abstract void OnEnter(ProjectileObject projectileObject);

	public abstract void OnUpdate(ProjectileObject projectileObject);

	public abstract void OnFixedUpdate(ProjectileObject projectileObject);

	public abstract void OnExit(ProjectileObject projectileObject);

	public abstract void HandleState(ProjectileObject projectileObject);
}
