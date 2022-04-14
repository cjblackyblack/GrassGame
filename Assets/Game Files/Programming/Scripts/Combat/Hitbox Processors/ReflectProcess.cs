using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "HitboxProcess/ReflectProcess")]
public class ReflectProcess : HitboxProcess
{
	public override bool Process(Hitbox sourceObject, CombatBox hitObject)
	{
		return false;
	}
}