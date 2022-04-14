using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "CharacterState/ActionState/AerialShoot/Fall")]
public class FallShootState : SmartState
{
    public int CoyoteTime;
    public int LedgeGrabTime;
    public float ledgeBoost;

    public override void OnEnter(SmartObject smartObject)
    {
        if (smartObject.LocomotionStateMachine.PreviousLocomotionEnum == LocomotionStates.Aerial && smartObject.ActionStateMachine.PreviousActionEnum == ActionStates.Idle)
        {

            smartObject.LocomotionStateMachine.ChangeLocomotionState(LocomotionStates.AerialShoot);
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


    }

    public override void OnExit(SmartObject smartObject)
    {
        base.OnExit(smartObject);
        smartObject.ClimbingInfo.CanGrab = false;
    }

    public override void BeforeCharacterUpdate(SmartObject smartObject, float deltaTime)
    {
        smartObject.MovementVector = smartObject.InputVector;
        if (smartObject.InputVector != Vector3.zero)
            smartObject.ActionStateMachine.ChangeActionState(ActionStates.Move);

        if (smartObject.CurrentAirTime <= CoyoteTime && smartObject.Controller.Button4Buffer > 0)
        {
            smartObject.LocomotionStateMachine.ChangeLocomotionState(LocomotionStates.Grounded);
            smartObject.ActionStateMachine.ChangeActionState(ActionStates.Jump);
        }

        //if (smartObject.CurrentFrame > LedgeGrabTime)
        //    smartObject.ClimbingInfo.CanGrab = true;



    }

    public override void UpdateVelocity(SmartObject smartObject, ref Vector3 currentVelocity, float deltaTime)
    {

        base.UpdateVelocity(smartObject, ref currentVelocity, deltaTime);
        if (smartObject.Motor.LastGroundingStatus.IsStableOnGround)
        {

            currentVelocity += smartObject.Motor.CharacterForward * ledgeBoost;
        }
    }

    public override void AfterCharacterUpdate(SmartObject smartObject, float deltaTime)
    {
        //if (smartObject.Controller.Button1Buffer > 0 && smartObject.Cooldown <= 0)
        //    smartObject.ActionStateMachine.ChangeActionState(ActionStates.Attack);

        if ((smartObject.Controller.Button3ReleaseBuffer > 0 || !smartObject.Controller.Button3Hold))
        {
            smartObject.LocomotionStateMachine.ChangeLocomotionState(LocomotionStates.Aerial);
            smartObject.ActionStateMachine.ChangeActionState(ActionStates.Idle);
        }

        if (smartObject.Motor.GroundingStatus.IsStableOnGround)
            smartObject.ActionStateMachine.ChangeActionState(ActionStates.Idle);

        if (smartObject.Controller.Button4Buffer > 0 && smartObject.CurrentAirTime > CoyoteTime && smartObject.AirJumps > 0)
            smartObject.ActionStateMachine.ChangeActionState(ActionStates.Jump);

        //if (smartObject.Controller.Button2Buffer > 0)
        //    smartObject.ActionStateMachine.ChangeActionState(ActionStates.Boost);

    }

    public override void PostGroundingUpdate(SmartObject smartObject, float deltaTime)
    {

    }
}
