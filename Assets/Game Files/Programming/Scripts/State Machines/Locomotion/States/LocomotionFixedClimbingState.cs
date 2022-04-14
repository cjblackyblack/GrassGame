using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterState/LocomotionState/FixedClimbingState")]
public class LocomotionFixedClimbingState : LocomotionState
{
    public float ClimbMoveThreshold;
    public float AnchoringDuration;
    public float AnchTest;

	public override void OnEnter(SmartObject smartObject)
	{
        smartObject.Motor.SetMovementCollisionsSolvingActivation(false);
        smartObject.Motor.SetGroundSolvingActivation(false);

        smartObject.Motor.BaseVelocity *= 0;
        smartObject.ClimbingInfo.ClimbingState = ClimbingState.Anchoring;
        smartObject.ActiveAirTime = 0;
        smartObject.CurrentAirTime = 0;

        // Store the target position and rotation to snap to


        smartObject.ClimbingInfo.TargetLedgePos = smartObject.ClimbingInfo.ActiveLedge.ClosestPointOnLadderSegment(smartObject.Motor.TransientPosition, out smartObject.ClimbingInfo.LedgeSegmentState);
        smartObject.ClimbingInfo.NormalizedPosition = smartObject.ClimbingInfo.ActiveLedge.GetNormalizedPosition(smartObject.Motor.TransientPosition, out smartObject.ClimbingInfo.NormalizedPosition);
        smartObject.ClimbingInfo.TargetLedgePos = smartObject.ClimbingInfo.ActiveLedge.GetPositionFromFloat(smartObject.ClimbingInfo.NormalizedPosition);
        smartObject.ClimbingInfo.TargetLadderRot = smartObject.ClimbingInfo.ActiveLedge.transform.rotation; smartObject.ClimbingInfo.AnchorTime = 0f;





        smartObject.Motor.SetTransientPosition(smartObject.ClimbingInfo.TargetLedgePos + (smartObject.Motor.CharacterUp)/3);

        smartObject.ClimbingInfo.RotationBeforeClimbing = smartObject.Motor.TransientRotation;
        smartObject.ClimbingInfo.AnchorStartPos = smartObject.ClimbingInfo.TargetLedgePos;
        smartObject.ClimbingInfo.AnchorStartRot = smartObject.Motor.TransientRotation;
    }

	public override void OnExit(SmartObject smartObject)
	{
        smartObject.Motor.SetGroundSolvingActivation(true);
        smartObject.Motor.SetMovementCollisionsSolvingActivation(true);
        base.OnExit(smartObject);
	}

    public override void BeforeCharacterUpdate(SmartObject smartObject, float deltaTime)
	{

        //smartObject.ClimbingInfo._ladderTargetPosition = smartObject.ClimbingInfo._activeLadder.ClosestPointOnLadderSegment(smartObject.Motor.TransientPosition, out smartObject.ClimbingInfo._onLadderSegmentState);
        smartObject.ClimbingInfo.TargetLedgePos = smartObject.ClimbingInfo.ActiveLedge.GetPositionFromFloat(smartObject.ClimbingInfo.NormalizedPosition);
        smartObject.ClimbingInfo.TargetLadderRot = smartObject.ClimbingInfo.ActiveLedge.transform.rotation;

        smartObject.MovementVector = smartObject.InputVector;
        if (smartObject.MovementVector != Vector3.zero)
        {
            smartObject.ClimbingInfo.LedgeInput = -Mathf.RoundToInt(Vector3.Dot(smartObject.MovementVector, smartObject.Motor.CharacterRight));
        }
        else
            smartObject.ClimbingInfo.LedgeInput = 0;

        if (smartObject.Controller.Button4Buffer > 0 && (smartObject.ActionStateMachine.CurrentActionEnum == ActionStates.Idle || smartObject.ActionStateMachine.CurrentActionEnum == ActionStates.Move))
        {
            smartObject.Controller.Button4Buffer = 0;
            if ((Vector3.Dot(smartObject.MovementVector, smartObject.Motor.CharacterForward)) < 0)
            {

                smartObject.LocomotionStateMachine.ChangeLocomotionState(LocomotionStates.Aerial);
                smartObject.ActionStateMachine.ChangeActionState(ActionStates.Idle);
            }
            else
            {
                if((Vector3.Dot(smartObject.MovementVector, smartObject.Motor.CharacterForward)) > 0.75f)
                    smartObject.Motor.BaseVelocity *= 0;
                smartObject.ActionStateMachine.ChangeActionState(ActionStates.Jump);
            }
        }
    }

	public override void UpdateRotation(SmartObject smartObject, ref Quaternion currentRotation, float deltaTime)
	{
        smartObject.Motor.BaseVelocity *= 0;
        switch (smartObject.ClimbingInfo.ClimbingState)
        {
            case ClimbingState.Climbing:
                    currentRotation = smartObject.ClimbingInfo.ActiveLedge.transform.rotation;
                break;
            case ClimbingState.Anchoring: case ClimbingState.DeAnchoring:
                {
                    currentRotation = Quaternion.Slerp(smartObject.ClimbingInfo.AnchorStartRot, smartObject.ClimbingInfo.TargetLadderRot, (smartObject.ClimbingInfo.AnchorTime / AnchoringDuration));
                }
                break;
        }
    }

	public override void UpdateVelocity(SmartObject smartObject, ref Vector3 currentVelocity, float deltaTime)
	{

        Vector3 tmpPosition = Vector3.zero;
        switch (smartObject.ClimbingInfo.ClimbingState)
        {
            case ClimbingState.Climbing:
                {
                    tmpPosition = Vector3.Lerp(smartObject.Motor.TransientPosition, smartObject.ClimbingInfo.TargetLedgePos, AnchTest);
                    break;
                }
            case ClimbingState.Anchoring: case ClimbingState.DeAnchoring:
                {
                    tmpPosition = Vector3.Lerp(smartObject.ClimbingInfo.AnchorStartPos, smartObject.ClimbingInfo.TargetLedgePos, (smartObject.ClimbingInfo.AnchorTime / AnchoringDuration));
                }
                break;
        }
        currentVelocity += smartObject.Motor.GetVelocityForMovePosition(smartObject.Motor.TransientPosition, tmpPosition, deltaTime) ;
    }

	public override void AfterCharacterUpdate(SmartObject smartObject, float deltaTime) 
    {

        switch (smartObject.ClimbingInfo.ClimbingState)
        {
            case ClimbingState.Climbing:
                // Detect getting off ladder during climbing
                smartObject.ClimbingInfo.ActiveLedge.ClosestPointOnLadderSegment(smartObject.Motor.TransientPosition, out smartObject.ClimbingInfo.LedgeSegmentState);
                if (Mathf.Abs(smartObject.ClimbingInfo.LedgeSegmentState) > 0.01f)
                {
                    // If we're higher than the ladder top point
                    if (smartObject.ClimbingInfo.LedgeSegmentState > 0)
                    {
                        if (smartObject.ClimbingInfo.ActiveLedge.TopAttach)
                        {
                            smartObject.GrabLedge(smartObject.ClimbingInfo.ActiveLedge.TopAttach, true, 2f);
                        }
                        else
                        {
                            smartObject.ClimbingInfo.TargetLedgePos = smartObject.ClimbingInfo.ActiveLedge.TopReleasePoint.position;
							//_ladderTargetRotation = _activeLadder.TopReleasePoint.rotation;
                            smartObject.ClimbingInfo.ClimbingState = ClimbingState.DeAnchoring;
                            smartObject.ClimbingInfo.AnchorTime = 0f;
                            smartObject.ClimbingInfo.AnchorStartPos = smartObject.Motor.TransientPosition;
                            smartObject.ClimbingInfo.AnchorStartRot = smartObject.Motor.TransientRotation;
                        }

                    }
                    // If we're lower than the ladder bottom point
                    else if (smartObject.ClimbingInfo.LedgeSegmentState < 0)
                    {
                        if (smartObject.ClimbingInfo.ActiveLedge.BottomAttach)
                        {
                            smartObject.GrabLedge(smartObject.ClimbingInfo.ActiveLedge.BottomAttach, true, -1f);
                        }
                        else
                        {
                            smartObject.ClimbingInfo.TargetLedgePos = smartObject.ClimbingInfo.ActiveLedge.BottomReleasePoint.position;
                            //_ladderTargetRotation = _activeLadder.TopReleasePoint.rotation;
                            smartObject.ClimbingInfo.ClimbingState = ClimbingState.DeAnchoring;
                            smartObject.ClimbingInfo.AnchorTime = 0f;
                            smartObject.ClimbingInfo.AnchorStartPos = smartObject.Motor.TransientPosition;
                            smartObject.ClimbingInfo.AnchorStartRot = smartObject.Motor.TransientRotation;

                        }
                    }
                }
                break;
            case ClimbingState.Anchoring: case ClimbingState.DeAnchoring:
                // Detect transitioning out from anchoring states
                if (smartObject.ClimbingInfo.AnchorTime >= AnchoringDuration)
                {
                    if (smartObject.ClimbingInfo.ClimbingState == ClimbingState.Anchoring)
                    {
                        smartObject.ClimbingInfo.ClimbingState = ClimbingState.Climbing;
                        smartObject.ClimbingInfo.AnchorTime = 0f;
                        smartObject.ClimbingInfo.AnchorStartPos = smartObject.Motor.TransientPosition;
                        smartObject.ClimbingInfo.AnchorStartRot = smartObject.Motor.TransientRotation;
                    }
                    else if (smartObject.ClimbingInfo.ClimbingState == ClimbingState.DeAnchoring)
                    {

                        smartObject.LocomotionStateMachine.ChangeLocomotionState(LocomotionStates.Aerial);
                        smartObject.ActionStateMachine.ChangeActionState(ActionStates.Idle);
                    }
                }

                // Keep track of time since we started anchoring
                smartObject.ClimbingInfo.AnchorTime += deltaTime;
                break;
        }
    }
}