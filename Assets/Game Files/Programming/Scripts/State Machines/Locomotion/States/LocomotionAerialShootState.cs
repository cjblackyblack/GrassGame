using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterState/LocomotionState/AerialShoot")]
public class LocomotionAerialShootState : LocomotionState
{
    public float MaxAirMoveSpeed;
    public float AirAccelerationSpeed;
    public float AirFriction;

    public float Drag;
    public float NoLandAnimationTime;
    public float RotationInfluence;
    public AnimationCurve CollisionHeight;
    public AnimationCurve CollisionYOffset;

    public override void OnEnter(SmartObject smartObject)
    {
        smartObject.Motor.StepHandling = KinematicCharacterController.StepHandlingMethod.None;
        smartObject.ToggleGuns(true, 0);
        smartObject.Controller.Button3Buffer = 0;
    } 

    public override void OnExit(SmartObject smartObject)
    {
        smartObject.ToggleGuns(false, 0);
        smartObject.Motor.SetCapsuleDimensions(smartObject.CharacterRadius, smartObject.CharacterHeight, smartObject.CharacterCenter.y);
        smartObject.Motor.StepHandling = KinematicCharacterController.StepHandlingMethod.Extra;
        //smartObject.Cooldown = 70;
    }

    public override void UpdateRotation(SmartObject smartObject, ref Quaternion currentRotation, float deltaTime)
    {
        if (smartObject.OrientationMethod == OrientationMethod.TowardsCamera)
        {
            Vector3 smoothedLookInputDirection = Vector3.Slerp(smartObject.Motor.CharacterForward, smartObject.LookInputVector, 1 - Mathf.Exp(-OrientationSharpness * deltaTime)).normalized;

            currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, smartObject.Motor.CharacterUp);
        }
        else if (Vector3.ProjectOnPlane(smartObject.Motor.BaseVelocity, smartObject.Motor.CharacterUp).sqrMagnitude > 0f && OrientationSharpness > 0f)// && (Vector3.Dot(Vector3.down, smartObject.Gravity.normalized) > 0.999f))
        {
            // Smoothly interpolate from current to target look direction    // (Vector3.ProjectOnPlane(smartObject.Motor.BaseVelocity, smartObject.Motor.CharacterUp) + smartObject.movementVector * RotationInfluence).normalized               
            Vector3 smoothedLookInputDirection = Vector3.Slerp(smartObject.Motor.CharacterForward, (Vector3.ProjectOnPlane(smartObject.Motor.BaseVelocity, smartObject.Motor.CharacterUp) + smartObject.MovementVector * RotationInfluence).normalized, 1 - Mathf.Exp(-OrientationSharpness * deltaTime)).normalized;

            // Set the current rotation (which will be used by the KinematicCharacterMotor)
            currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, smartObject.Motor.CharacterUp);
        }
        //else if (smartObject.MovementVector.sqrMagnitude > 0f && OrientationSharpness > 0f)
        //{
        //    // Smoothly interpolate from current to target look direction
        //    Vector3 smoothedLookInputDirection = Vector3.Slerp(smartObject.Motor.CharacterForward, smartObject.StoredMovementVector, 1 - Mathf.Exp(-OrientationSharpness * deltaTime)).normalized;
        //
        //    // Set the current rotation (which will be used by the KinematicCharacterMotor)
        //    currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, smartObject.Motor.CharacterUp);
        //}

        CalculateCharacterUp(smartObject, ref currentRotation, deltaTime);
    }

    public override void BeforeCharacterUpdate(SmartObject smartObject, float deltaTime)
    {

    }

    public override void UpdateVelocity(SmartObject smartObject, ref Vector3 currentVelocity, float deltaTime)
    {
        float yVel = Vector3.Project(currentVelocity, smartObject.Motor.CharacterUp).y;

        smartObject.Motor.SetCapsuleDimensions(smartObject.CharacterRadius, CollisionHeight.Evaluate(yVel), CollisionYOffset.Evaluate(yVel));
        // Gravity
        currentVelocity += ((smartObject.Gravity * smartObject.GravityModifier)) * deltaTime;

        // Drag
        currentVelocity *= (1f / (1f + (Drag * smartObject.GravityModifier * deltaTime)));


        //Vector3 adjustedStoredVelocity = Vector3.Project(smartObject.StoredVelocity, smartObject.Motor.CharacterUp);
        //  adjustedStoredVelocity.y = 0;

        //}
        ////Debug.Log(currentVelocity.z);
        //currentVelocity += (adjustedStoredVelocity * deltaTime);
        //smartObject.StoredVelocity *= AirFriction;



    }



    public override void AfterCharacterUpdate(SmartObject smartObject, float deltaTime)
    {


        smartObject.ActiveAirTime += smartObject.LocalTimeScale;
        if (smartObject.ActiveAirTime - smartObject.CurrentAirTime >= 1)
        {
            smartObject.CurrentAirTime = (int)smartObject.ActiveAirTime;
        }

        if (smartObject.ClimbingInfo.CanGrab)
            smartObject.PollLedge();
    }

    public override void PostGroundingUpdate(SmartObject smartObject, float deltaTime)
    {
        if (smartObject.Motor.GroundingStatus.IsStableOnGround)
        {
            //Debug.Log(smartObject.AirTime);
            if (smartObject.CurrentAirTime > NoLandAnimationTime && smartObject.ActionStateMachine.CurrentActionEnum != ActionStates.Boost)
                smartObject.ActionStateMachine.ChangeActionState(smartObject.LocomotionStateMachine.LandState);

            if (smartObject.CurrentAirTime > 6 && (smartObject.ActionStateMachine.CurrentActionEnum != ActionStates.Jump))
            {
                smartObject.LocomotionStateMachine.ChangeLocomotionState(LocomotionStates.Grounded);
            }
            else
            {
                smartObject.Motor.ForceUnground(0.02f);
            }
        }

    }

    public override void CalculateStateVelocity(SmartObject smartObject, ref Vector3 currentVelocity, float deltaTime)
    {
        //if (smartObject.MovementVector.sqrMagnitude > 0f)
        //{

        Vector3 addedVelocity = smartObject.MovementVector * AirAccelerationSpeed * deltaTime;

        Vector3 currentVelocityOnInputsPlane = Vector3.ProjectOnPlane(currentVelocity, smartObject.Motor.CharacterUp);

        // Limit air velocity from inputs
        if (currentVelocityOnInputsPlane.magnitude < MaxAirMoveSpeed)
        {
            // clamp addedVel to make total vel not exceed max vel on inputs plane
            Vector3 newTotal = Vector3.ClampMagnitude(currentVelocityOnInputsPlane + addedVelocity, MaxAirMoveSpeed);
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
        //}
        //else
        //{
    }
}