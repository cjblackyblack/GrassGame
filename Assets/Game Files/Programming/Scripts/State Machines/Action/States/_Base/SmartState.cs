using KinematicCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartState : ScriptableObject
{
	public string AnimationState;
	public float AnimationTransitionTime;
	public float AnimationTransitionOffset;
	public int MaxTime;

	public VFXContainer[] VFX;
	public BodyVFXContainer[] BodyVFX;
	public SFXContainer[] SFX;

	public virtual void OnEnter(SmartObject smartObject)
	{
		smartObject.CurrentTime = -1;
		smartObject.CurrentFrame = -1;
		if (AnimationTransitionTime != 0)
		{
			smartObject.Animator.CrossFadeInFixedTime(AnimationState, AnimationTransitionTime, 0, AnimationTransitionOffset);
		
		}
		else
		{
			smartObject.Animator.Play(AnimationState, 0, 0);
		}
	}

	public void CreateVFX(SmartObject smartObject)
	{
		if (VFX == null || VFX.Length == 0)
			return;

		for (int i = 0; i < VFX.Length; i++)
			if (VFX[i].Time == smartObject.CurrentFrame)
			{
				GameObject obj = Instantiate(VFX[i].VFX, smartObject.transform.position, smartObject.transform.rotation, smartObject.transform);
				
					obj.transform.localPosition = VFX[i].Position;
					obj.transform.localEulerAngles = VFX[i].Rotation;
				if (!VFX[i].Local)
					obj.transform.parent = null;

			}
	}

	public void CreateBodyVFX(SmartObject smartObject)
	{
		if (BodyVFX == null || BodyVFX.Length == 0)
			return;

		for (int i = 0; i < BodyVFX.Length; i++)
			if (BodyVFX[i].Time == smartObject.CurrentFrame)
				smartObject.ToggleBodyVFX(BodyVFX[i].BodyVFX, BodyVFX[i].Toggle);
	}

	public void CreateSFX(SmartObject smartObject)
	{
		if (SFX == null || SFX.Length == 0)
			return;

		for (int i = 0; i < SFX.Length; i++)
			if (SFX[i].Time == smartObject.CurrentFrame)
				SFX[i].SFX.PlaySFX(smartObject);
	}

	public virtual void OnExit(SmartObject smartObject)
	{

	}

	public virtual void HandleState(SmartObject smartObject)
	{

	}

	public virtual void UpdateRotation(SmartObject smartObject, ref Quaternion currentRotation, float deltaTime)
	{

	}

	public virtual void UpdateVelocity(SmartObject smartObject, ref Vector3 currentVelocity, float deltaTime)
	{

	}

	public virtual void BeforeCharacterUpdate(SmartObject smartObject, float deltaTime)
	{

	}

	public virtual void PostGroundingUpdate(SmartObject smartObject, float deltaTime)
	{

	}

	public virtual void AfterCharacterUpdate(SmartObject smartObject, float deltaTime)
	{

	}

	public virtual bool IsColliderValidForCollisions(SmartObject smartObject, Collider coll)
	{
		return true;
	}

	public virtual void OnGroundHit(SmartObject smartObject, Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
	{

	}

	public virtual void OnMovementHit(SmartObject smartObject, Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
	{

	}

	public virtual void ProcessHitStabilityReport(SmartObject smartObject, Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
	{

	}

	public virtual void OnDiscreteCollisionDetected(SmartObject smartObject, Collider hitCollider)
	{

	}
}