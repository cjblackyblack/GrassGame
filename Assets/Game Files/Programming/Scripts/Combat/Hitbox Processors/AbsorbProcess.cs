using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "HitboxProcess/AbsorbProcess")]
public class AbsorbProcess : HitboxProcess
{
	public override bool Process(Hitbox sourceObject, CombatBox hitObject)
	{
		return false;
	}
}