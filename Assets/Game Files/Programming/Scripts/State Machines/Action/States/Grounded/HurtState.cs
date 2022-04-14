using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterState/ActionState/Grounded/Hurt")]
public class HurtState : SmartState
{
	public TangibilityFrames[] TangibilityFrames;
	public AnimationCurve HurtFriction;

	public override void OnEnter(SmartObject smartObject)
	{
		base.OnEnter(smartObject);
		smartObject.MovementVector = smartObject.KnockbackDir;
	}

	public override void BeforeCharacterUpdate(SmartObject smartObject, float deltaTime)
	{
		CombatUtilities.CreateTangibilityFrames(smartObject, TangibilityFrames);

		smartObject.KnockbackDir *= HurtFriction.Evaluate(smartObject.CurrentFrame);
	}

	public override void UpdateVelocity(SmartObject smartObject, ref Vector3 currentVelocity, float deltaTime)
	{
		currentVelocity = smartObject.KnockbackDir;
	}
	public override void UpdateRotation(SmartObject smartObject, ref Quaternion currentRotation, float deltaTime)
	{
		Vector3 smoothedLookInputDirection = Vector3.ProjectOnPlane(smartObject.KnockbackDir, smartObject.Motor.CharacterUp);

		currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, smartObject.Motor.CharacterUp);

		smartObject.LocomotionStateMachine.CurrentLocomotionState.CalculateCharacterUp(smartObject, ref currentRotation, deltaTime);
	}

	public override void AfterCharacterUpdate(SmartObject smartObject, float deltaTime)
	{
		if (smartObject.CurrentFrame > smartObject.HitStun)
			smartObject.ActionStateMachine.ChangeActionState(ActionStates.Idle);
	}
}
