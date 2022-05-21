using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "CharacterState/ActionState/Aerial/AerialDodge")]
public class AerialDodgeState : SmartState
{
	public int JumpFrame;
	public float JumpPower;
	public float JumpScalableForwardSpeed;
	public float UnlockTime;
	public TangibilityFrames[] TangibilityFrames;
	public MotionCurve MotionCurve;
	public StateTransition[] StateTransitions;

	public override void OnEnter(SmartObject smartObject)
	{
		base.OnEnter(smartObject);
		smartObject.Controller.Button4Buffer = 0;
		smartObject.CurrentAirTime = 0;
		smartObject.CurrentFrame = 0;
		smartObject.AirJumps--;
		smartObject.MovementVector = smartObject.InputVector;

		if (smartObject.LocomotionStateMachine.CurrentLocomotionEnum != LocomotionStates.Aerial) //we came from a state like an attack
		{
			smartObject.Motor.ForceUnground(0.02f);
			smartObject.Motor.SetGroundSolvingActivation(false);
			smartObject.LocomotionStateMachine.ChangeLocomotionState(LocomotionStates.Aerial);
		}

	}

	public override void OnExit(SmartObject smartObject)
	{
		base.OnExit(smartObject);
		smartObject.GravityModifier = 1;
		CombatUtilities.ResetTangibilityFrames(smartObject, TangibilityFrames);
		smartObject.Motor.SetGroundSolvingActivation(true);
	}

	public override void BeforeCharacterUpdate(SmartObject smartObject, float deltaTime)
	{
		base.BeforeCharacterUpdate(smartObject, deltaTime);
	
		MotionCurve.GravityMod(smartObject);
		CombatUtilities.CreateTangibilityFrames(smartObject, TangibilityFrames);
	}

	public override void UpdateRotation(SmartObject smartObject, ref Quaternion currentRotation, float deltaTime)
	{
		base.UpdateRotation(smartObject, ref currentRotation, deltaTime);



		smartObject.LocomotionStateMachine.CurrentLocomotionState.CalculateCharacterUp(smartObject, ref currentRotation, deltaTime);
	}

	public override void UpdateVelocity(SmartObject smartObject, ref Vector3 currentVelocity, float deltaTime)
	{


		if (smartObject.CurrentFrame < MotionCurve.TurnAroundTime && (smartObject.InputVector != Vector3.zero) && smartObject.OrientationMethod != OrientationMethod.TowardsCamera)
		{
			smartObject.MovementVector = smartObject.InputVector;
			smartObject.Motor.RotateCharacter(MotionCurve.TurnAroundRotation(smartObject, ref currentVelocity, true));
		}

		if (smartObject.CurrentFrame > UnlockTime)
		{
			smartObject.MovementVector = smartObject.InputVector;
		}

		if (smartObject.CurrentFrame <= JumpFrame)
		{
			smartObject.CurrentAirTime = 0;
			//smartObject.CurrentFrame = 0;
		}

		if ((smartObject.CurrentFrame >= JumpFrame) && smartObject.CurrentAirTime == 0)
		{
			Jump(smartObject, ref currentVelocity, deltaTime);
		}

	
		smartObject.LocomotionStateMachine.CurrentLocomotionState.CalculateStateVelocity(smartObject, ref currentVelocity, deltaTime);

	}

	public override void AfterCharacterUpdate(SmartObject smartObject, float deltaTime)
	{
		base.AfterCharacterUpdate(smartObject, deltaTime);
		if (smartObject.CurrentFrame > MaxTime)
			smartObject.ActionStateMachine.ChangeActionState(ActionStates.Idle);

		if (StateTransitions != null)
			for (int i = 0; i < StateTransitions.Length; i++)
				if (StateTransitions[i].CanTransition(smartObject))
					smartObject.ActionStateMachine.ChangeActionState(StateTransitions[i].TransitionState);

	}

	public void Jump(SmartObject smartObject, ref Vector3 currentVelocity, float deltaTime)
	{
		Vector3 jumpDirection = smartObject.Motor.CharacterUp;
		currentVelocity *= 0;
		//if (smartObject.Motor.GroundingStatus.FoundAnyGround && !smartObject.Motor.GroundingStatus.IsStableOnGround)//&& (Vector3.Dot(Vector3.down, smartObject.Gravity.normalized) > 0.99f))
		//{
		//	jumpDirection = smartObject.Motor.GroundingStatus.GroundNormal;
		//}

		currentVelocity += (((jumpDirection.normalized * (JumpPower))) - (Vector3.Project(currentVelocity, smartObject.Motor.CharacterUp)));
		currentVelocity += (smartObject.MovementVector * JumpScalableForwardSpeed);
	}

	private void OnValidate()
	{

		if (StateTransitions.Length > 0)
		{
			for (int i = 0; i < StateTransitions.Length; i++)
				StateTransitions[i].MaxTime = MaxTime;
		}
	}
}
