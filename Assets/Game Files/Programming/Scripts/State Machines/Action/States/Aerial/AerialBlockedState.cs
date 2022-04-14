using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterState/ActionState/Aerial/Blocked")]
public class AerialBlockedState : SmartState
{
	public AnimationCurve FrictionCurve;
	public override void OnEnter(SmartObject smartObject)
	{
		base.OnEnter(smartObject);
		smartObject.MovementVector = -smartObject.Motor.CharacterForward;
		smartObject.LocomotionStateMachine.ChangeLocomotionState(LocomotionStates.Aerial);
	}

	public override void UpdateRotation(SmartObject smartObject, ref Quaternion currentRotation, float deltaTime)
	{

		base.UpdateRotation(smartObject, ref currentRotation, deltaTime);
	}
	public override void UpdateVelocity(SmartObject smartObject, ref Vector3 currentVelocity, float deltaTime)
	{
		smartObject.MovementVector *= 0;
		currentVelocity = -(smartObject.Motor.CharacterForward * FrictionCurve.Evaluate(smartObject.CurrentFrame));
		smartObject.LocomotionStateMachine.CurrentLocomotionState.CalculateStateVelocity(smartObject, ref currentVelocity, deltaTime);
	}

	public override void OnExit(SmartObject smartObject)
	{
		base.OnExit(smartObject);
	}

	public override void AfterCharacterUpdate(SmartObject smartObject, float deltaTime)
	{
		base.AfterCharacterUpdate(smartObject, deltaTime);
		if (smartObject.CurrentFrame > MaxTime)
			smartObject.ActionStateMachine.ChangeActionState(ActionStates.Idle);
	}
}
