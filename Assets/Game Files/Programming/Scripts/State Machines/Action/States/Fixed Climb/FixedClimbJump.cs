using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "CharacterState/ActionState/FixedClimb/Jump")]
public class FixedClimbJump : SmartState
{
    public int JumpFrame;
    public float JumpPower = 10;
    public float JumpScalableForwardSpeed = 1;
    public float FallVelocity;
    public int HorizontalLockoutTime;

	public override void OnEnter(SmartObject smartObject)
	{
		base.OnEnter(smartObject);
        smartObject.ClimbingInfo.CanGrab = false;
        smartObject.Motor.SetGroundSolvingActivation(false);
    }

    public override void OnExit(SmartObject smartObject)
    {
        base.OnExit(smartObject);
        smartObject.GravityModifier = 1;

        smartObject.Motor.SetGroundSolvingActivation(true);
    }

    public override void BeforeCharacterUpdate(SmartObject smartObject, float deltaTime)
    {
        smartObject.ClimbingInfo.CanGrab = false;
        if (smartObject.CurrentFrame > JumpFrame && smartObject.InputVector.sqrMagnitude > 0f)
            smartObject.MovementVector = smartObject.InputVector;
    }

    public override void UpdateVelocity(SmartObject smartObject, ref Vector3 currentVelocity, float deltaTime)
    {
        if (smartObject.CurrentFrame >= JumpFrame && smartObject.CurrentAirTime == 0)
        {
            Jump(smartObject, ref currentVelocity, deltaTime);
        }
        else
        {
            smartObject.Motor.ForceUnground(0.02f);

            if (smartObject.CurrentFrame > HorizontalLockoutTime)
                smartObject.LocomotionStateMachine.CurrentLocomotionState.CalculateStateVelocity(smartObject, ref currentVelocity, deltaTime);
            //Test Hold to Increase Height
            //smartObject.GravityModifier = smartObject.Controller.Button4Hold ? 0.5f : 1;
        }
        smartObject.GravityModifier = (smartObject.Controller.Button4Hold && smartObject.Controller.Button4Buffer == 0 && smartObject.Controller.Button4ReleaseBuffer == 0) ? 0.9f : 1 / smartObject.LocalTimeScale;


    }

    public override void AfterCharacterUpdate(SmartObject smartObject, float deltaTime)
    {


        float yVel = Vector3.Project(smartObject.Motor.BaseVelocity, smartObject.Motor.CharacterUp).y;

        if (yVel < FallVelocity && smartObject.CurrentFrame > JumpFrame + 2)
        {
            smartObject.ActionStateMachine.ChangeActionState(ActionStates.Idle);
            smartObject.Motor.SetGroundSolvingActivation(true);
        }
    }

    public void Jump(SmartObject smartObject, ref Vector3 currentVelocity, float deltaTime)
    {
        Vector3 jumpDirection = smartObject.Motor.CharacterUp;
        if (smartObject.Motor.GroundingStatus.FoundAnyGround && !smartObject.Motor.GroundingStatus.IsStableOnGround)
        {
            jumpDirection = smartObject.Motor.GroundingStatus.GroundNormal;
        }
        smartObject.Motor.ForceUnground(0.02f);
        currentVelocity += (jumpDirection * JumpPower) - Vector3.Project(currentVelocity, smartObject.Motor.CharacterUp);
        currentVelocity += (smartObject.MovementVector * JumpScalableForwardSpeed);
        smartObject.LocomotionStateMachine.ChangeLocomotionState(LocomotionStates.Aerial); //this is here to catch Coyote Time edge cases where we just changed from ground to aerial
        smartObject.ActiveAirTime = 1;
        smartObject.CurrentAirTime = 1;
    }
}
