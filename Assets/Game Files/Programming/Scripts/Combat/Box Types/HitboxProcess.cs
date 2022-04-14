using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HitboxProcess : ScriptableObject
{
	public abstract bool Process(Hitbox sourceObject, CombatBox hitObject); //returns true if other processes may continue afterward
}