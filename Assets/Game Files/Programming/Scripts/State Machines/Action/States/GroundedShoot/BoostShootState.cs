using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterState/ActionState/GroundedShoot/Boost")]
public class BoostShootState : SmartState
{
	public int MinTime;
	public TangibilityFrames[] TangibilityFrames;
	public MotionCurve MotionCurve;
	public float DirectionControl;
	public int CoyoteTime;



	public override void OnEnter(SmartObject smartObject)
	{
		if (smartObject.LocomotionStateMachine.PreviousLocomotionEnum == LocomotionStates.Grounded && smartObject.ActionStateMachine.PreviousActionEnum == ActionStates.Boost)
		{

		}
		else
		{
			smartObject.CurrentTime = -1;
			smartObject.CurrentFrame = -1;
		}
		if (AnimationTransitionTime != 0)
		{
			smartObject.Animator.CrossFadeInFixedTime(AnimationState, AnimationTransitionTime, 0, AnimationTransitionOffset);

		}
		else
		{
			smartObject.Animator.Play(AnimationState, 0, 0);

		}
		smartObject.Controller.Button4Buffer = 0;

		smartObject.LocomotionStateMachine.ChangeLocomotionState(LocomotionStates.GroundedShoot);
		smartObject.MovementVector = smartObject.MovementVector == Vector3.zero ? smartObject.Motor.CharacterForward : smartObject.InputVector.normalized;
		smartObject.ToggleBodyVFX(BodyVFX[0].BodyVFX, true);
	}

	public override void OnExit(SmartObject smartObject)
	{

		smartObject.GravityModifier = 1;
		//CombatUtilities.ResetTangibilityFrames(smartObject, TangibilityFrames);
		for (int i = 0; i < BodyVFX.Length; i++)
			smartObject.ToggleBodyVFX(BodyVFX[i].BodyVFX, false);
	}

	public override void BeforeCharacterUpdate(SmartObject smartObject, float deltaTime)
	{
		//smartObject.MovementVector = smartObject.InputVector;
		if (smartObject.CurrentAirTime <= CoyoteTime && smartObject.Controller.Button4Buffer > 0)
		{
			smartObject.LocomotionStateMachine.ChangeLocomotionState(LocomotionStates.GroundedShoot);
			smartObject.ActionStateMachine.ChangeActionState(ActionStates.Jump);
		}

		smartObject.MovementVector = Vector3.Slerp(smartObject.MovementVector, smartObject.InputVector == Vector3.zero ? smartObject.StoredMovementVector : smartObject.InputVector.normalized, DirectionControl);
		MotionCurve.GravityMod(smartObject);
		if (TangibilityFrames.Length > 0)
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
		if (smartObject.LocomotionStateMachine.CurrentLocomotionEnum == LocomotionStates.GroundedShoot)
		{
			currentVelocity = MotionCurve.GetFixedTotalCurve(smartObject, true);
			float currentVelocityMagnitude = currentVelocity.magnitude;


			//if (smartObject.CurrentFrame < MotionCurve.TurnAroundTime && (smartObject.InputVector != Vector3.zero) && smartObject.OrientationMethod != OrientationMethod.TowardsCamera)
			//{
			//		smartObject.Motor.RotateCharacter(MotionCurve.TurnAroundRotation(smartObject, ref currentVelocity, true));
			//}

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
			Vector3 inputRight = Vector3.Cross(currentVelocity, smartObject.Motor.CharacterUp);
			Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * currentVelocityMagnitude; ;
			Vector3 targetMovementVelocity = reorientedInput * 1;

			// Smooth movement Velocity
			currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1f - Mathf.Exp(-1000 * deltaTime));
			//currentVelocity += smartObject.StoredVelocity * deltaTime;
			//smartObject.StoredVelocity *= friction;
		}
		else if (smartObject.LocomotionStateMachine.CurrentLocomotionEnum == LocomotionStates.AerialShoot)
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
		CreateVFX(smartObject);
		CreateBodyVFX(smartObject);
		CreateSFX(smartObject);

		base.AfterCharacterUpdate(smartObject, deltaTime);

		//if (smartObject.CurrentFrame > MaxTime)
		//	smartObject.ActionStateMachine.ChangeActionState(ActionStates.Idle);

		if (smartObject.CurrentFrame > MinTime)
		{
			if (smartObject.Controller.Button2ReleaseBuffer > 0 || smartObject.Controller.Button2Hold == false)
				smartObject.ActionStateMachine.ChangeActionState(ActionStates.Idle);

			//if (smartObject.Controller.Button1Buffer > 0 && smartObject.Cooldown <= 0)
			//	smartObject.ActionStateMachine.ChangeActionState(ActionStates.Attack);
		}

		if ((smartObject.Controller.Button3ReleaseBuffer > 0 || !smartObject.Controller.Button3Hold))
		{
			smartObject.LocomotionStateMachine.ChangeLocomotionState(LocomotionStates.Grounded);
			smartObject.ActionStateMachine.ChangeActionState(ActionStates.Boost);
		}

		if (smartObject.Controller.Button4Buffer > 0 && ((smartObject.LocomotionStateMachine.CurrentLocomotionEnum == LocomotionStates.Grounded) || (smartObject.CurrentAirTime > CoyoteTime && smartObject.AirJumps > 0 && smartObject.LocomotionStateMachine.CurrentLocomotionEnum == LocomotionStates.Aerial)))
			smartObject.ActionStateMachine.ChangeActionState(ActionStates.Jump);
	}
}
