using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffect : ScriptableObject
{
	public int maxTime;
	public int tickRate;
	public SmartState overrideState;
	public virtual void OnEnter(SmartObject smartObject, TangibleObject origin)
	{

	}

	public virtual void OnTakeDamage(SmartObject smartObject, TangibleObject origin)
	{

	}

	public virtual void OnTick(SmartObject smartObject, TangibleObject origin)
	{

	}

	public virtual void OnExit(SmartObject smartObject, TangibleObject origin)
	{

	}
}
