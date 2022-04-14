using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "CharacterState/LocomotionState/GroundedShoot")]
public class LocomotionGroundedShootState : LocomotionState
{
    public float MaxStableMoveSpeed;
    public float StableMovementSharpness;
    public int FinishRotationTime;
    public float friction;
    public override void OnEnter(SmartObject smartObject)
    {
        smartObject.AirJumps = smartObject.MaxAirJumps;
        smartObject.ToggleGuns(true, 0);
        smartObject.Controller.Button3Buffer = 0;
        if (smartObject.ActionStateMachine.CurrentActionEnum != ActionStates.Jump)
        {
            smartObject.ActiveAirTime = 0;
            smartObject.CurrentAirTime = 0;
        }
    }

    public override void OnExit(SmartObject smartObject)
    {
        smartObject.ToggleGuns(false, 0);
        //smartObject.Cooldown = 70;
    }

    public override void UpdateRotation(SmartObject smartObject, ref Quaternion currentRotation, float deltaTime)
    {
        //WE NEED THIS TO FIRE WHEN THE PLAYER TAPS A TO ROTATE
        if (smartObject.OrientationMethod == OrientationMethod.TowardsCamera)
        {
            Vector3 smoothedLookInputDirection = Vector3.Slerp(smartObject.Motor.CharacterForward, smartObject.LookInputVector, 1 - Mathf.Exp(-OrientationSharpness * deltaTime)).normalized;

            currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, smartObject.Motor.CharacterUp);
        }
        //WE NEED THIS TO FIRE WHEN THE PLAYER TAPS A TO ROTATE
        else if ((smartObject.MovementVector.sqrMagnitude > 0f || (smartObject.CurrentFrame < FinishRotationTime && smartObject.ActionStateMachine.CurrentActionEnum == ActionStates.Idle)) && OrientationSharpness > 0f && smartObject.ActionStateMachine.CurrentActionEnum != ActionStates.Blocked)
        {
            // Smoothly interpolate from current to target look direction

            Vector3 smoothedLookInputDirection = Vector3.Slerp(smartObject.Motor.CharacterForward, smartObject.StoredMovementVector, 1 - Mathf.Exp(-OrientationSharpness * deltaTime)).normalized;

            // Set the current rotation (which will be used by the KinematicCharacterMotor)
            currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, smartObject.Motor.CharacterUp);
        }

        CalculateCharacterUp(smartObject, ref currentRotation, deltaTime);
    }

    public override void UpdateVelocity(SmartObject smartObject, ref Vector3 currentVelocity, float deltaTime)
    {

    }

    public override void BeforeCharacterUpdate(SmartObject smartObject, float deltaTime)
    {

    }

    public override void AfterCharacterUpdate(SmartObject smartObject, float deltaTime)
    {

    }

    public override void PostGroundingUpdate(SmartObject smartObject, float deltaTime)
    {
        if (!smartObject.Motor.GroundingStatus.IsStableOnGround && smartObject.Motor.LastGroundingStatus.IsStableOnGround)
        {
            smartObject.LocomotionStateMachine.ChangeLocomotionState(LocomotionStates.Aerial);
            if (smartObject.ActionStateMachine.CurrentActionEnum != ActionStates.Boost && smartObject.ActionStateMachine.CurrentActionEnum != ActionStates.Jump)
            {
                //Debug.Log("force fall");
                smartObject.ActionStateMachine.ChangeActionState(ActionStates.Idle);
            }
        }
    }

    public override void CalculateStateVelocity(SmartObject smartObject, ref Vector3 currentVelocity, float deltaTime)
    {
        float currentVelocityMagnitude = currentVelocity.magnitude;

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
        Vector3 inputRight = Vector3.Cross(smartObject.MovementVector, smartObject.Motor.CharacterUp);
        Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * smartObject.MovementVector.magnitude;
        Vector3 targetMovementVelocity = reorientedInput * MaxStableMoveSpeed;

        // Smooth movement Velocity
        currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1f - Mathf.Exp(-StableMovementSharpness * deltaTime));
        //currentVelocity += smartObject.StoredVelocity * deltaTime;
        //smartObject.StoredVelocity *= friction;
    }
}