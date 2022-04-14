using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "CharacterState/ActionState/FixedClimb/Move")]
public class FixedClimbMove : SmartState
{
    public float ClimbingSpeed;
    public LayerMask climbFilter;
    public float climbShell;

	public override void BeforeCharacterUpdate(SmartObject smartObject, float deltaTime)
	{
        if (smartObject.ClimbingInfo.LedgeInput == 0)
            smartObject.ActionStateMachine.ChangeActionState(ActionStates.Idle);
    }

	public override void UpdateVelocity(SmartObject smartObject, ref Vector3 currentVelocity, float deltaTime)
	{
        Ray rayRight = new Ray(smartObject.transform.position + smartObject.transform.up, smartObject.Motor.CharacterRight);
        Ray rayLeft = new Ray(smartObject.transform.position = smartObject.transform.up, -smartObject.Motor.CharacterRight);
        if (smartObject.ClimbingInfo.LedgeInput > 0)
        {
            if (!Physics.Raycast(rayLeft, out RaycastHit hitInfoRight, smartObject.Motor.Capsule.radius + climbShell, climbFilter))
                currentVelocity = (smartObject.ClimbingInfo.LedgeInput *  (smartObject.ClimbingInfo.ActiveLedge.BottomAnchorPoint - smartObject.ClimbingInfo.ActiveLedge.TopAnchorPoint).normalized).normalized * ClimbingSpeed;
                //smartObject.ClimbingInfo.NormalizedPosition = smartObject.ClimbingInfo._activeLadder.GetNormalizedPosition(smartObject.Motor.TransientPosition + -smartObject.Motor.CharacterRight * ClimbingSpeed, out smartObject.ClimbingInfo.NormalizedPosition);
        }
        else if (smartObject.ClimbingInfo.LedgeInput < 0)
        {
            if (!Physics.Raycast(rayRight, out RaycastHit hitInfoLeft, smartObject.Motor.Capsule.radius + climbShell, climbFilter))
                currentVelocity = (smartObject.ClimbingInfo.LedgeInput * (smartObject.ClimbingInfo.ActiveLedge.BottomAnchorPoint - smartObject.ClimbingInfo.ActiveLedge.TopAnchorPoint).normalized).normalized * ClimbingSpeed;
                //smartObject.ClimbingInfo.NormalizedPosition = smartObject.ClimbingInfo._activeLadder.GetNormalizedPosition(smartObject.Motor.TransientPosition + smartObject.Motor.CharacterRight * ClimbingSpeed, out smartObject.ClimbingInfo.NormalizedPosition);
        }
        if(smartObject.MovementVector != Vector3.zero)
            smartObject.StoredMovementVector = smartObject.MovementVector;
    }

	public override void AfterCharacterUpdate(SmartObject smartObject, float deltaTime)
	{
		base.AfterCharacterUpdate(smartObject, deltaTime);
        smartObject.ClimbingInfo.NormalizedPosition = smartObject.ClimbingInfo.ActiveLedge.GetNormalizedPosition(smartObject.Motor.TransientPosition, out smartObject.ClimbingInfo.NormalizedPosition);
    }
}
