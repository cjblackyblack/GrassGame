using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "CharacterState/ActionState/Aerial/Hurt")]
public class AerialHurtState : SmartState
{
	public TangibilityFrames[] TangibilityFrames;
	public AnimationCurve HurtFriction;
	public override void OnEnter(SmartObject smartObject)
	{
		base.OnEnter(smartObject);

		smartObject.LocomotionStateMachine.ChangeLocomotionState(LocomotionStates.Aerial);
		smartObject.MovementVector = smartObject.KnockbackDir;
	}

	public override void BeforeCharacterUpdate(SmartObject smartObject, float deltaTime)
	{
		CombatUtilities.CreateTangibilityFrames(smartObject, TangibilityFrames);
		//if(smartObject.Motor.GroundingStatus.IsStableOnGround)
		//{
		//	smartObject.LocomotionStateMachine.ChangeLocomotionState(LocomotionStates.Grounded);
		//	smartObject.ActionStateMachine.ChangeActionState(ActionStates.Hurt);
		//}
		smartObject.KnockbackDir *= HurtFriction.Evaluate(smartObject.CurrentFrame);
	}

	public override void UpdateVelocity(SmartObject smartObject, ref Vector3 currentVelocity, float deltaTime)
	{
		Vector3 storedVerticalVelocity = Vector3.ProjectOnPlane(currentVelocity, smartObject.Motor.CharacterForward);
		Vector3 calculatedVelocity = smartObject.KnockbackDir;

		currentVelocity = calculatedVelocity + storedVerticalVelocity;

		Vector3 addedVelocity = Vector3.zero;

		// Prevent air-climbing sloped walls
		if (smartObject.Motor.GroundingStatus.FoundAnyGround)
		{
			if (Vector3.Dot(currentVelocity + addedVelocity, addedVelocity) > 0f)
			{
				Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(smartObject.Motor.CharacterUp, smartObject.Motor.GroundingStatus.GroundNormal), smartObject.Motor.CharacterUp).normalized;
				addedVelocity = Vector3.ProjectOnPlane(addedVelocity, perpenticularObstructionNormal);
			}
		}

		// Apply added velocity
		currentVelocity += addedVelocity;
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
