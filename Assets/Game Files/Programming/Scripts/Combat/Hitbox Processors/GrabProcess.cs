using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "HitboxProcess/GrabProcess")]
public class GrabProcess : HitboxProcess
{
	public override bool Process(Hitbox sourceObject, CombatBox hitObject)
	{
		return false;
	}
}