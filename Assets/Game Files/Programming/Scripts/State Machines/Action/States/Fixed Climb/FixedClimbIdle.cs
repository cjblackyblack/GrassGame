using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "CharacterState/ActionState/FixedClimb/Idle")]
public class FixedClimbIdle : SmartState
{
	public override void BeforeCharacterUpdate(SmartObject smartObject, float deltaTime)
	{
		base.BeforeCharacterUpdate(smartObject, deltaTime);
		if (smartObject.ClimbingInfo.LedgeInput != 0 && smartObject.ClimbingInfo.ActiveLedge.CanClimbMove)
			smartObject.ActionStateMachine.ChangeActionState(ActionStates.Move);
	}

	public override void UpdateVelocity(SmartObject smartObject, ref Vector3 currentVelocity, float deltaTime)
	{
		currentVelocity = Vector3.zero;
	}
}
