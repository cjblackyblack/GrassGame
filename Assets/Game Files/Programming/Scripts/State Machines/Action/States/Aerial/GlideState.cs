using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "CharacterState/ActionState/Aerial/Glide")]
public class GlideState : SmartState
{
    public float GravityMod;
    public int MinTime;
    public SmartState AttackState;


    public override void OnEnter(SmartObject smartObject)
    {
        if (smartObject.LocomotionStateMachine.PreviousLocomotionEnum == LocomotionStates.AerialShoot && smartObject.ActionStateMachine.PreviousActionEnum == ActionStates.Jump)
        {
            smartObject.LocomotionStateMachine.ChangeLocomotionState(LocomotionStates.Aerial);
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

        smartObject.AirJumps--;
        smartObject.GravityModifier = GravityMod;
        smartObject.ToggleBodyVFX(BodyVFX[0].BodyVFX, true);
    }

    public override void OnExit(SmartObject smartObject)
    {
        base.OnExit(smartObject);
        smartObject.ClimbingInfo.CanGrab = false;
        smartObject.GravityModifier = 1;
        for (int i = 0; i < BodyVFX.Length; i++)
            smartObject.ToggleBodyVFX(BodyVFX[i].BodyVFX, false);
    }
    public override void BeforeCharacterUpdate(SmartObject smartObject, float deltaTime)
    {
        smartObject.MovementVector = smartObject.InputVector;
        smartObject.StoredMovementVector = smartObject.MovementVector;
    }

    public override void UpdateVelocity(SmartObject smartObject, ref Vector3 currentVelocity, float deltaTime)
    {
        if(smartObject.CurrentFrame == 0)
		{
            currentVelocity = Vector3.ProjectOnPlane(currentVelocity, smartObject.Motor.CharacterUp);
        }
        // Add move input
        smartObject.LocomotionStateMachine.CurrentLocomotionState.CalculateStateVelocity(smartObject, ref currentVelocity, deltaTime);
        currentVelocity = Vector3.ProjectOnPlane(currentVelocity, smartObject.Motor.CharacterUp);

    }


    public override void AfterCharacterUpdate(SmartObject smartObject, float deltaTime)
    {
        CreateVFX(smartObject);
        CreateBodyVFX(smartObject);
        CreateSFX(smartObject);


    if ((smartObject.Controller.Button1Buffer > 0 || smartObject.Controller.Button2Buffer > 0))
      if (AttackState != null)
                smartObject.ActionStateMachine.ChangeActionState(AttackState);
            else
                smartObject.ActionStateMachine.ChangeActionState(ActionStates.Attack);

        //if ((smartObject.Controller.Button3Buffer > 0 || smartObject.Controller.Button3Hold))
        //{
        //    smartObject.ActionStateMachine.ChangeActionState(ActionStates.Dodge);
        //}

        if (smartObject.CurrentFrame > MinTime)
            if (!smartObject.Controller.Button4Hold || smartObject.Controller.Button4ReleaseBuffer > 0 || smartObject.CurrentFrame > MaxTime)
                smartObject.ActionStateMachine.ChangeActionState(ActionStates.Idle);

    }
}