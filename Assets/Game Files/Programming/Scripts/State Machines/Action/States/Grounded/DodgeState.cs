using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterState/ActionState/Grounded/Dodge")]
public class DodgeState : SmartState
{
	public TangibilityFrames[] TangibilityFrames;
	public MotionCurve MotionCurve;

	public override void OnEnter(SmartObject smartObject)
	{
		base.OnEnter(smartObject);
		smartObject.Controller.Button4Buffer = 0;


		smartObject.MovementVector = smartObject.MovementVector == Vector3.zero ? smartObject.Motor.CharacterForward : smartObject.InputVector.normalized;
	}

	public override void OnExit(SmartObject smartObject)
	{

		smartObject.GravityModifier = 1;
		//CombatUtilities.ResetTangibilityFrames(smartObject, TangibilityFrames);
	}

	public override void BeforeCharacterUpdate(SmartObject smartObject, float deltaTime)
	{
		//smartObject.MovementVector = smartObject.InputVector;
		MotionCurve.GravityMod(smartObject);
		CombatUtilities.CreateTangibilityFrames(smartObject, TangibilityFrames);
	}

	public override void UpdateRotation(SmartObject smartObject, ref Quaternion currentRotation, float deltaTime)
	{

		//Vector3 smoothedLookInputDirection = Vector3.Slerp(smartObject.Motor.CharacterForward, MotionCurve.Rotation(smartObject), 1 - Mathf.Exp(-100 * deltaTime)).normalized;

		//currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, smartObject.Motor.CharacterUp);


		//smartObject.LocomotionStateMachine.CurrentLocomotionState.CalculateCharacterUp(smartObject, ref currentRotation, deltaTime);
	}

	public override void UpdateVelocity(SmartObject smartObject, ref Vector3 currentVelocity, float deltaTime)
	{
		if (smartObject.LocomotionStateMachine.CurrentLocomotionEnum == LocomotionStates.Grounded)
		{
			currentVelocity = MotionCurve.GetFixedTotalCurve(smartObject);
			float currentVelocityMagnitude = currentVelocity.magnitude;


			if (smartObject.CurrentFrame < MotionCurve.TurnAroundTime && (smartObject.InputVector != Vector3.zero) && smartObject.OrientationMethod != OrientationMethod.TowardsCamera)
			{
				smartObject.Motor.RotateCharacter(MotionCurve.TurnAroundRotation(smartObject, ref currentVelocity, true));
			}

			Vector3 effectiveGroundNormal = smartObject.Motor.GroundingStatus.GroundNormal;
			if (currentVelocityMagnitude > 0f && smartObject.Motor.GroundingStatus.SnappingPrevented)
			{
				// Take the normal from where we're coming from
				Vector3 groundPointToCharacter = smartObject.Motor.TransientPosition - smartObject.Motor.GroundingStatus.GroundPoint;
				if (Vector3.Dot(currentVelocity, groundPointToCharacter) >= 0f)
				{
					effectiveGroundNormal = smartObject.Motor.GroundingStatus.OuterGroundNormal;
				}
				else
				{
					effectiveGroundNormal = smartObject.Motor.GroundingStatus.InnerGroundNormal;
				}
			}

			// Reorient velocity on slope
			currentVelocity = smartObject.Motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocityMagnitude;

			// Calculate target velocity
			Vector3 inputRight = Vector3.Cross(smartObject.Motor.CharacterForward, smartObject.Motor.CharacterUp);
			Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * currentVelocityMagnitude; ;
			Vector3 targetMovementVelocity = reorientedInput * 1;

			// Smooth movement Velocity
			currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1f - Mathf.Exp(-1000 * deltaTime));
			//currentVelocity += smartObject.StoredVelocity * deltaTime;
			//smartObject.StoredVelocity *= friction;
		}
		else if (smartObject.LocomotionStateMachine.CurrentLocomotionEnum == LocomotionStates.Aerial)
		{
			Vector3 addedVelocity = Vector3.zero;

			Vector3 currentVelocityOnInputsPlane = Vector3.ProjectOnPlane(currentVelocity, smartObject.Motor.CharacterUp);

			// Limit air velocity from inputs
			if (currentVelocityOnInputsPlane.magnitude < 10)//locomotion aerial max move input just being lazy
			{
				// clamp addedVel to make total vel not exceed max vel on inputs plane
				Vector3 newTotal = Vector3.ClampMagnitude(currentVelocityOnInputsPlane + addedVelocity, 10);
				addedVelocity = newTotal - currentVelocityOnInputsPlane;
			}
			else
			{
				// Make sure added vel doesn't go in the direction of the already-exceeding velocity
				if (Vector3.Dot(currentVelocityOnInputsPlane, addedVelocity) > 0f)
				{
					addedVelocity = Vector3.ProjectOnPlane(addedVelocity, currentVelocityOnInputsPlane.normalized);
				}
			}

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

		//smartObject.LocomotionStateMachine.CurrentLocomotionState.CalculateStateVelocity(smartObject, ref currentVelocity, deltaTime);

	}
	public override void AfterCharacterUpdate(SmartObject smartObject, float deltaTime)
	{
		base.AfterCharacterUpdate(smartObject, deltaTime);
		if (smartObject.CurrentFrame > MaxTime)
			smartObject.ActionStateMachine.ChangeActionState(ActionStates.Idle);
	}
}
