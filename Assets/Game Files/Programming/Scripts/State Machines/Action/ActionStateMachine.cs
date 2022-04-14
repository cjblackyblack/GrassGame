using KinematicCharacterController;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class ActionStateMachine : SerializedMonoBehaviour
{
	private SmartObject smartObject => GetComponent<SmartObject>();
	public Dictionary<ActionStates, SmartState> ActionDict;
	public SmartState CurrentActionState;
	public ActionStates CurrentActionEnum;
	public SmartState PreviousActionState;
	public ActionStates PreviousActionEnum;
	public void StartMachine(SmartState[] states)
	{
		UpdateActionStates(states);

		CurrentActionState = ActionDict[ActionStates.Idle];
		ChangeActionState(ActionStates.Idle);
	}

	public void UpdateActionStates(SmartState[] states) //MAKE ONE THAT ACTUALLY UPDATES THIS SHIT
	{
		ActionDict.Clear();
		for (int i = 0; i < states.Length; i++)
		{
			if (states[i] == null)
				continue;
			ActionDict.Add((ActionStates)i, states[i]);
		}
	}

	public void ChangeActionState(ActionStates actionState)
	{
		if (CurrentActionState == smartObject.LocomotionStateMachine.DeadState)
			return;
		PreviousActionState = CurrentActionState;
		PreviousActionEnum = CurrentActionEnum;
		CurrentActionState.OnExit(smartObject);
		CurrentActionState = ActionDict[actionState];
		CurrentActionEnum = actionState;
		CurrentActionState.OnEnter(smartObject);
		smartObject.OnActionChange?.Invoke(actionState);
	}

	public void ChangeActionState(SmartState actionState)
	{
		PreviousActionState = CurrentActionState;
		PreviousActionEnum = CurrentActionEnum;
		CurrentActionState.OnExit(smartObject);
		CurrentActionState = actionState;
		CurrentActionEnum = ActionStates.Other;
		CurrentActionState.OnEnter(smartObject);
		smartObject.OnActionChange?.Invoke(ActionStates.Other);
	}

	public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
	{
		CurrentActionState.UpdateRotation(smartObject, ref currentRotation, deltaTime);
	}

	public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
	{
		CurrentActionState.UpdateVelocity(smartObject, ref currentVelocity, deltaTime);
	}

	public void BeforeCharacterUpdate(float deltaTime)
	{
		CurrentActionState.BeforeCharacterUpdate(smartObject, deltaTime);
	}

	public void PostGroundingUpdate(float deltaTime)
	{
		CurrentActionState.PostGroundingUpdate(smartObject, deltaTime);
	}

	public void AfterCharacterUpdate(float deltaTime)
	{
		CurrentActionState.AfterCharacterUpdate(smartObject, deltaTime);
	}

	public bool IsColliderValidForCollisions(Collider coll)
	{
		return CurrentActionState.IsColliderValidForCollisions(smartObject, coll);
	}

	public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
	{
		CurrentActionState.OnGroundHit(smartObject, hitCollider, hitNormal, hitPoint, ref hitStabilityReport);
	}

	public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
	{
		CurrentActionState.OnMovementHit(smartObject, hitCollider, hitNormal, hitPoint, ref hitStabilityReport); ;
	}

	public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
	{
		CurrentActionState.ProcessHitStabilityReport(smartObject, hitCollider, hitNormal, hitPoint, atCharacterPosition, atCharacterRotation, ref hitStabilityReport);
	}

	public void OnDiscreteCollisionDetected(Collider hitCollider)
	{
		CurrentActionState.OnDiscreteCollisionDetected(smartObject, hitCollider);
	}
}
