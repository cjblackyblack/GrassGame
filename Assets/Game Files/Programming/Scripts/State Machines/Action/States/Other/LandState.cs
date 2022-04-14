using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterState/ActionState/Other/Land")]
public class LandState : SmartState
{
	public int LandingLag;
	public float LandingFriction;

	public override void OnEnter(SmartObject smartObject)
	{

		base.OnEnter(smartObject);
		if(smartObject.InputVector == Vector3.zero)
			smartObject.MovementVector *= 0;
		//smartObject.LocomotionStateMachine.ChangeLocomotionState(LocomotionStates.Grounded);
	}

	public override void UpdateVelocity(SmartObject smartObject, ref Vector3 currentVelocity, float deltaTime)
	{
		smartObject.MovementVector *= LandingFriction;
		base.UpdateVelocity(smartObject, ref currentVelocity, deltaTime);
		smartObject.LocomotionStateMachine.CurrentLocomotionState.CalculateStateVelocity(smartObject, ref currentVelocity, deltaTime);

	}

	public override void BeforeCharacterUpdate(SmartObject smartObject, float deltaTime)
	{
		CreateVFX(smartObject);
		CreateBodyVFX(smartObject);
		CreateSFX(smartObject);
	}

	public override void AfterCharacterUpdate(SmartObject smartObject, float deltaTime)
	{
		if (smartObject.CurrentFrame > LandingLag)
			smartObject.ActionStateMachine.ChangeActionState(ActionStates.Idle);
	}
}