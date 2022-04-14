using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;

public abstract class LocomotionState : ScriptableObject
{

	public SmartState[] SmartStates;
	[SerializeField]
	private BonusOrientationMethod bonusOrientationMethod = BonusOrientationMethod.TowardsGravity;
	public float OrientationSharpness;
	public float BonusOrientationSharpness;
	public virtual void OnEnter(SmartObject smartObject)
	{
	}

	public virtual void OnExit(SmartObject smartObject)
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

	public virtual void CalculateStateVelocity(SmartObject smartObject, ref Vector3 currentVelocity, float deltaTime)
	{

	}

	public virtual void CalculateCharacterUp(SmartObject smartObject,ref Quaternion currentRotation, float deltaTime)
	{
		Vector3 currentUp = (currentRotation * Vector3.up);
		if (bonusOrientationMethod == BonusOrientationMethod.TowardsGravity)
		{
			// Rotate from current up to invert gravity
			Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, -smartObject.Gravity.normalized, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
			currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
		}
		else if (bonusOrientationMethod == BonusOrientationMethod.TowardsGroundSlopeAndGravity)
		{
			if (smartObject.Motor.GroundingStatus.IsStableOnGround)
			{
				Vector3 initialCharacterBottomHemiCenter = smartObject.Motor.TransientPosition + (currentUp * smartObject.Motor.Capsule.radius);

				Vector3 smoothedGroundNormal = Vector3.Slerp(smartObject.Motor.CharacterUp, smartObject.Motor.GroundingStatus.GroundNormal, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
				currentRotation = Quaternion.FromToRotation(currentUp, smoothedGroundNormal) * currentRotation;

				// Move the position to create a rotation around the bottom hemi center instead of around the pivot
				smartObject.Motor.SetTransientPosition(initialCharacterBottomHemiCenter + (currentRotation * Vector3.down * smartObject.Motor.Capsule.radius));
			}
			else
			{
				Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, -smartObject.Gravity.normalized, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
				currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
			}
		}
		else
		{
			Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, Vector3.up, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
			currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
		}
	}
}
