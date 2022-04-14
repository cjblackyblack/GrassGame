using KinematicCharacterController;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Locomotion State does the heavy physics lifting to make scripting for behaviours (ActionStates) easier.
//This could be redundant, but its my first time doing this so we learning over here
public class LocomotionStateMachine : SerializedMonoBehaviour
{
	private SmartObject smartObject => GetComponent<SmartObject>();

	public Dictionary<LocomotionStates, LocomotionState> LocomotionDict;
	public LocomotionState[] LocomotionStateArray;
	public LocomotionState CurrentLocomotionState;
	public LocomotionStates CurrentLocomotionEnum;
	public LocomotionState PreviousLocomotionState;
	public LocomotionStates PreviousLocomotionEnum;
	public LocomotionStates NextLocomotionState;
	public VehicleState CurrentVehicleState;
	public SmartState LandState;
	public SmartState DefaultLandState;
	public SmartState DeadState;

	public void Start()
	{
		StartMachine(LocomotionStateArray);
		smartObject.ActionStateMachine.StartMachine(CurrentLocomotionState.SmartStates);
	}

	public void StartMachine(LocomotionState[] states)
	{
		LocomotionDict.Clear();
		for (int i = 0; i < states.Length; i++)
		{
			if (states[i] == null)
				continue;
			LocomotionDict.Add((LocomotionStates)i, states[i]);
		}

		CurrentLocomotionState = LocomotionDict[LocomotionStates.Aerial];
		ChangeLocomotionState(LocomotionStates.Aerial);
	}

	public void ChangeLocomotionState(LocomotionStates locomotionState)
	{
		if (smartObject.ActionStateMachine.CurrentActionState == DeadState)
			return;

		NextLocomotionState = locomotionState;
		PreviousLocomotionState = CurrentLocomotionState;
		PreviousLocomotionEnum = CurrentLocomotionEnum;
		CurrentLocomotionState.OnExit(smartObject);
		CurrentLocomotionState = LocomotionDict[locomotionState];
		CurrentLocomotionEnum = locomotionState;
		CurrentLocomotionState.OnEnter(smartObject);
		smartObject.ActionStateMachine.UpdateActionStates(CurrentLocomotionState.SmartStates);
		smartObject.OnLocomotionChange?.Invoke(locomotionState);
	}

	public void ChangeLocomotionState(VehicleState vehicleState)
	{
		NextLocomotionState = LocomotionStates.Vehicle;
		PreviousLocomotionState = CurrentLocomotionState;
		PreviousLocomotionEnum = CurrentLocomotionEnum;
		CurrentLocomotionState.OnExit(smartObject);
		CurrentLocomotionState = vehicleState;
		CurrentLocomotionEnum = LocomotionStates.Vehicle;
		CurrentLocomotionState.OnEnter(smartObject);
		smartObject.ActionStateMachine.UpdateActionStates(CurrentLocomotionState.SmartStates);
		smartObject.OnLocomotionChange?.Invoke(LocomotionStates.Vehicle);
	}

	public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
	{
		CurrentLocomotionState.UpdateRotation(smartObject, ref currentRotation, deltaTime);
	}

	public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
	{
		CurrentLocomotionState.UpdateVelocity(smartObject, ref currentVelocity, deltaTime);
	}

	public void BeforeCharacterUpdate(float deltaTime)
	{
		CurrentLocomotionState.BeforeCharacterUpdate(smartObject, deltaTime);
	}

	public void PostGroundingUpdate(float deltaTime)
	{
		CurrentLocomotionState.PostGroundingUpdate(smartObject, deltaTime);
	}

	public void AfterCharacterUpdate(float deltaTime)
	{
		CurrentLocomotionState.AfterCharacterUpdate(smartObject, deltaTime);
	}

	public bool IsColliderValidForCollisions(Collider coll)
	{
		return CurrentLocomotionState.IsColliderValidForCollisions(smartObject, coll);
	}

	public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
	{
		CurrentLocomotionState.OnGroundHit(smartObject, hitCollider, hitNormal, hitPoint, ref hitStabilityReport);
	}

	public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
	{
		CurrentLocomotionState.OnMovementHit(smartObject, hitCollider, hitNormal, hitPoint, ref hitStabilityReport); ;
	}

	public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
	{
		CurrentLocomotionState.ProcessHitStabilityReport(smartObject, hitCollider, hitNormal, hitPoint, atCharacterPosition, atCharacterRotation, ref hitStabilityReport);
	}

	public void OnDiscreteCollisionDetected(Collider hitCollider)
	{
		CurrentLocomotionState.OnDiscreteCollisionDetected(smartObject, hitCollider);
	}
}