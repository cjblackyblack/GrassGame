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
	public TangibilityFrames[] TangibilityFrames;
	public MotionCurve MotionCurve;

	public override void OnEnter(SmartObject smartObject)
	{
		base.OnEnter(smartObject);
		smartObject.Controller.Button4Buffer = 0;
		smartObject.CurrentAirTime = 0;
		smartObject.CurrentFrame = 0;
		smartObject.AirJumps--;
		smartObject.MovementVector = new Vector3(Mathf.RoundToInt(smartObject.InputVector.x), Mathf.RoundToInt(smartObject.InputVector.y), Mathf.RoundToInt(smartObject.InputVector.z));

	}

	public override void OnExit(SmartObject smartObject)
	{
		base.OnExit(smartObject);
		smartObject.GravityModifier = 1;
		CombatUtilities.ResetTangibilityFrames(smartObject, TangibilityFrames);
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

		if(smartObject.CurrentFrame <= JumpFrame)
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
	}

	public void Jump(SmartObject smartObject, ref Vector3 currentVelocity, float deltaTime)
	{
		Vector3 jumpDirection = smartObject.Motor.CharacterUp;
		currentVelocity *= 0;
		if (smartObject.Motor.GroundingStatus.FoundAnyGround && !smartObject.Motor.GroundingStatus.IsStableOnGround)//&& (Vector3.Dot(Vector3.down, smartObject.Gravity.normalized) > 0.99f))
		{
			jumpDirection = smartObject.Motor.GroundingStatus.GroundNormal;
		}

		currentVelocity += (((jumpDirection.normalized * (JumpPower))) - (Vector3.Project(currentVelocity, smartObject.Motor.CharacterUp)));
		currentVelocity += (smartObject.MovementVector.normalized * JumpScalableForwardSpeed);
	}
}
