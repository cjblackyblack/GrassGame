using KinematicCharacterController;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "CharacterState/ActionState/Aerial/AerialAttack")]
public class AerialAttackState : SmartState
{
	public StateTransition[] StateTransitions;

	public MotionCurve MotionCurve;
	public bool KillYVelocityWhenZero;

	public HitboxData[] hitboxes;
	public TangibilityFrames[] TangibilityFrames;
	public GameObject[] HitParticles = new GameObject[4];// match index to PhysicalTangibility Enum for reaction none for intangible ever
	public SFX HitFX;
	public float EntryFriction;
	public float CollisionFriction;
    public float ComboAmount = 1;

	public override void OnEnter(SmartObject smartObject)
	{
		base.OnEnter(smartObject);

		smartObject.Controller.Button1Buffer = 0;

		smartObject.MovementVector = smartObject.MovementVector == Vector3.zero ? smartObject.Motor.CharacterForward : smartObject.InputVector.normalized;
		smartObject.Motor.BaseVelocity *= EntryFriction;
		smartObject.LocomotionStateMachine.ChangeLocomotionState(LocomotionStates.Aerial);
		//smartObject.Motor.SetGroundSolvingActivation(true);
	}

	public override void OnExit(SmartObject smartObject)
	{
		smartObject.GravityModifier = 1;
		CombatUtilities.ResetTangibilityFrames(smartObject, TangibilityFrames);
		for (int i = 0; i < BodyVFX.Length; i++)
			smartObject.ToggleBodyVFX(BodyVFX[i].BodyVFX, false);
	}

	public override void BeforeCharacterUpdate(SmartObject smartObject, float deltaTime)
	{
		//smartObject.MovementVector = smartObject.InputVector;
		MotionCurve.GravityMod(smartObject);
		CombatUtilities.CreateTangibilityFrames(smartObject, TangibilityFrames);
	}

	public override void UpdateRotation(SmartObject smartObject, ref Quaternion currentRotation, float deltaTime)
	{
		Vector3 smoothedLookInputDirection = Vector3.Slerp(smartObject.Motor.CharacterForward, MotionCurve.Rotation(smartObject), 1 - Mathf.Exp(-100 * deltaTime)).normalized;

		currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, smartObject.Motor.CharacterUp);

		if (smartObject.Target && Vector3.Distance( Vector3.ProjectOnPlane(smartObject.transform.position, smartObject.transform.up), Vector3.ProjectOnPlane(smartObject.Target.transform.position, smartObject.Target.transform.up)) > MotionCurve.TrackingResistance)
		{
			if (MotionCurve.RotationTrackingCurve.Evaluate(smartObject.CurrentFrame) > 0)
				currentRotation = MotionCurve.TrackingRotation(smartObject, 10000);
		}


		smartObject.LocomotionStateMachine.CurrentLocomotionState.CalculateCharacterUp(smartObject, ref currentRotation, deltaTime);
	}

	public override void UpdateVelocity(SmartObject smartObject, ref Vector3 currentVelocity, float deltaTime)
	{
		Vector3 storedVerticalVelocity = Vector3.ProjectOnPlane(Vector3.ProjectOnPlane(currentVelocity, smartObject.Motor.CharacterForward), smartObject.Motor.CharacterRight);
		Vector3 calculatedVelocity = Vector3.zero;
		//Debug.Log(storedVerticalVelocity);
		calculatedVelocity = MotionCurve.GetFixedTotalCurve(smartObject);

		if (smartObject.Target != null)
		{
			if (smartObject.TrackingTime > 0)
			{
				storedVerticalVelocity -= MotionCurve.TrackingVelocity(smartObject, smartObject.CurrentFrame - 1) * 0.8f;
				smartObject.TrackingTime = 0;
			}
			if (MotionCurve.TrackingVelocity(smartObject) != Vector3.zero)
			{
				calculatedVelocity += MotionCurve.TrackingVelocity(smartObject);
				smartObject.TrackingTime++;
			}
		}


		currentVelocity = calculatedVelocity + storedVerticalVelocity;
		smartObject.CachedAerialVelocity = currentVelocity;

		if (smartObject.CurrentFrame < MotionCurve.TurnAroundTime && (smartObject.InputVector != Vector3.zero) && !smartObject.Target && smartObject.OrientationMethod != OrientationMethod.TowardsCamera)
		{
			smartObject.Motor.RotateCharacter(MotionCurve.TurnAroundRotation(smartObject, ref currentVelocity, true));
		}

		Vector3 addedVelocity = Vector3.zero;

		Vector3 currentVelocityOnInputsPlane = Vector3.ProjectOnPlane(currentVelocity, smartObject.Motor.CharacterUp);

		// Limit air velocity from inputs
		if (currentVelocityOnInputsPlane.magnitude < 10)//locomotion aerial max move input just being lazy
		{
			// clamp addedVel to make total vel not exceed max vel on inputs plane
			Vector3 newTotal = Vector3.ClampMagnitude(currentVelocityOnInputsPlane + addedVelocity, 10);
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
			//Debug.Log("hitting something");
			if (Vector3.Dot(currentVelocity + addedVelocity, addedVelocity) > 0f)
			{
				Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(smartObject.Motor.CharacterUp, smartObject.Motor.GroundingStatus.GroundNormal), smartObject.Motor.CharacterUp).normalized;
				addedVelocity = Vector3.ProjectOnPlane(addedVelocity, perpenticularObstructionNormal);
				//Debug.Log("preventing air climbing in attack");
			}
		}

		// Apply added velocity
		currentVelocity += addedVelocity;

		if (KillYVelocityWhenZero)
		{
			if (MotionCurve.VerticalCurve.Evaluate(smartObject.CurrentFrame) == 0 && MotionCurve.GravityModCurve.Evaluate(smartObject.CurrentFrame) == 0 && MotionCurve.TrackingVelocity(smartObject) == Vector3.zero)
			{
				Debug.Log("killingYVelocity");
				currentVelocity = Vector3.ProjectOnPlane(currentVelocity, smartObject.Motor.CharacterUp);
			}
		}


	}

	public override void OnMovementHit(SmartObject smartObject, Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
	{
		if (!smartObject.Motor.GroundingStatus.IsStableOnGround && !hitStabilityReport.IsStable && Vector3.Dot(smartObject.Motor.CharacterUp, smartObject.Motor.BaseVelocity) > 0.25f)
			//&& Vector3.Project(smartObject.Motor.BaseVelocity, smartObject.Motor.CharacterUp).y > MotionCurve.VerticalCurve.Evaluate(smartObject.CurrentFrame))
		{
			smartObject.Motor.BaseVelocity.y *= CollisionFriction;

		}
		if (Vector3.Dot(smartObject.Motor.CharacterForward.normalized, hitNormal) < -0.5f)
		{
			smartObject.Motor.BaseVelocity.x *= CollisionFriction;
			smartObject.Motor.BaseVelocity.z *= CollisionFriction;
		}
	}


	public override void AfterCharacterUpdate(SmartObject smartObject, float deltaTime)
	{
		base.AfterCharacterUpdate(smartObject, deltaTime);
		CreateHitboxes(smartObject);
		CreateVFX(smartObject);
		CreateBodyVFX(smartObject);
		CreateSFX(smartObject);


		if (smartObject.CurrentFrame > MaxTime)
		{
			smartObject.PreviousAttackBuffer = smartObject.PreviousAttackTime;
			smartObject.PreviousAttack = this;
			smartObject.ActionStateMachine.ChangeActionState(ActionStates.Idle);
		}

		if (StateTransitions != null)
			for (int i = 0; i < StateTransitions.Length; i++)
				if (StateTransitions[i].CanTransition(smartObject))
					smartObject.ActionStateMachine.ChangeActionState(StateTransitions[i].TransitionState);


	}

	protected void CreateHitboxes(SmartObject smartObject) //CHECK OVERLAPS SHOULD HAPPEN IN THE HITBOX CLASS TO SUPPORT CYLINDERS OR OTHER STRANGE MULTIOBJECT HITBOXES THIS CLASS NEEDS TO ONLY ACTIVATE THE ONES ACCORDING TO HITBOX DATA
	{
		for (int i = 0; i < hitboxes.Length; i++)
			if (smartObject.CurrentFrame >= hitboxes[i].ActiveFrames.x && smartObject.CurrentFrame <= hitboxes[i].ActiveFrames.y)
			{
				Hitbox activeHitbox = smartObject.Hitboxes[hitboxes[i].Hitbox].GetComponent<Hitbox>();
				activeHitbox.Active = true;

				if (smartObject.CurrentFrame == hitboxes[i].ActiveFrames.x)
				{
					activeHitbox.SetHitboxData(hitboxes[i]);

					for (int j = 0; j < hitboxes.Length; j++) //catch any late blooming hitboxes that got unlucky with no box ID set
					{
						if (!hitboxes[j].ShareID && !hitboxes[j].RefreshID)
							if (CombatUtilities.BoxGroupMatch(hitboxes[j].HitboxGroup, hitboxes[i].HitboxGroup))
							{
								smartObject.Hitboxes[hitboxes[j].Hitbox].GetComponent<Hitbox>().AttackID = activeHitbox.AttackID;
								smartObject.Hitboxes[hitboxes[j].Hitbox].GetComponent<Hitbox>().CombatBoxGroup = hitboxes[i].HitboxGroup;
							}
					}
				}//share shit with other hitboxes

				CombatBox hitBox = activeHitbox.CheckOverlap(hitboxes[i]); //Check Overlap, Filter Hitboxes, Apply Processes to hurtboxes, should only return combat boxes that were processed
				if (hitBox != null)
					if (hitBox.CurrentBoxTangibility != PhysicalObjectTangibility.Intangible)
						OnHitReaction(smartObject, activeHitbox, hitBox);
			}
			else
			{
				smartObject.Hitboxes[hitboxes[i].Hitbox].GetComponent<Hitbox>().Active = false;
			}
	}

	public void OnHitReaction(SmartObject smartObject, Hitbox hitbox, CombatBox hurtBox)// do FX stuff and change staes if needed
	{
		switch (hurtBox.CurrentBoxTangibility)
		{
			case PhysicalObjectTangibility.Normal:
				{ 
                    ComboManager.Instance.AddCombo(ComboAmount * hurtBox.SourceObject.comboMultiplier);

                    if (!hurtBox.SourceObject.IgnoreHitFX)
						CreateHitFX(0, hitbox);
				}
				break;
			case PhysicalObjectTangibility.Armor:
				{
					if (FlagsExtensions.HasFlag(hitbox.DamageInstance.breakthroughType, BreakthroughType.ArmorPierce))
					{
						if (!hurtBox.SourceObject.IgnoreHitFX)
							CreateHitFX(0, hitbox);
					}
					else
					{
						if (!hurtBox.SourceObject.IgnoreHitFX)
							CreateHitFX(1, hitbox);
					}
				}
				break;
			case PhysicalObjectTangibility.Guard:
				{
					if (FlagsExtensions.HasFlag(hitbox.DamageInstance.breakthroughType, BreakthroughType.GuardPierce) || hitbox.DamageInstance.unstoppable)
					{
						if (!hurtBox.SourceObject.IgnoreHitFX)
							CreateHitFX(0, hitbox);
					}
					else
					{
						smartObject.ActionStateMachine.ChangeActionState(ActionStates.Blocked);
					}

				}
				break;
			case PhysicalObjectTangibility.Invincible:
				{

				}
				break;
		}
		//Instantiate(HitParticles[(int)hitBox.CurrentBoxTangibility], hitBox.transform.position, Quaternion.identity);
	}

	void CreateHitFX(int index, CombatBox hitbox)
	{
		Instantiate(HitParticles[index], hitbox.transform.position, Quaternion.identity);
		HitFX.PlaySFX(hitbox.SourceObject);
	}

	private void OnValidate()
	{
		if (hitboxes.Length > 0)
		{
			for (int i = 0; i < hitboxes.Length; i++)
				hitboxes[i].MaxTime = MaxTime;

			hitboxes[0].RefreshID = true;
		}

		if (StateTransitions.Length > 0)
		{
			for (int i = 0; i < StateTransitions.Length; i++)
				StateTransitions[i].MaxTime = MaxTime;
		}
	}
}