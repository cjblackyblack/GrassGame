using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterState/ActionState/Grounded/Attack")]
public class AttackState : SmartState
{
	public StateTransition[] StateTransitions;

	public MotionCurve MotionCurve;
	public HitboxData[] hitboxes;
	public TangibilityFrames[] TangibilityFrames;
	public GameObject[] HitParticles = new GameObject[4];// match index to PhysicalTangibility Enum for reaction none for intangible ever
	public SFX HitFX;
	public ProjectileContainer[] Projectiles;

	public override void OnEnter(SmartObject smartObject)
	{
		base.OnEnter(smartObject);

		smartObject.Controller.Button1Buffer = 0;

		smartObject.MovementVector = smartObject.MovementVector == Vector3.zero ? smartObject.Motor.CharacterForward : smartObject.InputVector.normalized;
		smartObject.LocomotionStateMachine.ChangeLocomotionState(LocomotionStates.Grounded);
	}

	public override void OnExit(SmartObject smartObject)
	{
		smartObject.GravityModifier = 1;
		CombatUtilities.ResetTangibilityFrames(smartObject, TangibilityFrames);
		for (int i = 0; i < BodyVFX.Length; i++)
				smartObject.ToggleBodyVFX(BodyVFX[i].BodyVFX, false);

		for (int i = 0; i < Projectiles.Length; i++)
				smartObject.Guns[Projectiles[i].Transform].Active = false;
	}

	public override void BeforeCharacterUpdate(SmartObject smartObject, float deltaTime)
	{
		MotionCurve.GravityMod(smartObject);
		CombatUtilities.CreateTangibilityFrames(smartObject, TangibilityFrames);
	}

	public override void UpdateRotation(SmartObject smartObject, ref Quaternion currentRotation, float deltaTime)
    {
		Vector3 smoothedLookInputDirection = Vector3.Slerp(smartObject.Motor.CharacterForward, MotionCurve.Rotation(smartObject), 1 - Mathf.Exp(-100 * deltaTime)).normalized;

		currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, smartObject.Motor.CharacterUp);

		if (smartObject.Target && Vector3.Distance(smartObject.transform.position, smartObject.Target.transform.position) > 0.8f)
		{
			if (MotionCurve.RotationTrackingCurve.Evaluate(smartObject.CurrentFrame) > 0)
				currentRotation = MotionCurve.TrackingRotation(smartObject, 35);
		}

		smartObject.LocomotionStateMachine.CurrentLocomotionState.CalculateCharacterUp(smartObject, ref currentRotation, deltaTime);
	}

    public override void UpdateVelocity(SmartObject smartObject, ref Vector3 currentVelocity, float deltaTime)
    {
			currentVelocity = MotionCurve.GetFixedTotalCurve(smartObject);
			if (smartObject.Target != null)
				currentVelocity += MotionCurve.TrackingVelocity(smartObject);


			if (smartObject.CurrentFrame < MotionCurve.TurnAroundTime && (smartObject.InputVector != Vector3.zero) && !smartObject.Target && smartObject.OrientationMethod != OrientationMethod.TowardsCamera)
			{
					smartObject.Motor.RotateCharacter(MotionCurve.TurnAroundRotation(smartObject, ref currentVelocity, true));
			}

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
			Vector3 inputRight = Vector3.Cross(currentVelocity, smartObject.Motor.CharacterUp);
			Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * currentVelocityMagnitude; ;
			Vector3 targetMovementVelocity = reorientedInput * 1;

			// Smooth movement Velocity
			currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1f - Mathf.Exp(-1000 * deltaTime));	
	}

    public override void AfterCharacterUpdate(SmartObject smartObject, float deltaTime)
	{
		base.AfterCharacterUpdate(smartObject, deltaTime);
        CreateHitboxes(smartObject);
		CreateProjectiles(smartObject);
		CreateVFX(smartObject);
		CreateBodyVFX(smartObject);
		CreateSFX(smartObject);

        if (smartObject.CurrentFrame > MaxTime)
            smartObject.ActionStateMachine.ChangeActionState(ActionStates.Idle);
		if(StateTransitions != null)
			for (int i = 0; i < StateTransitions.Length; i--)
				if (StateTransitions[i].CanTransition(smartObject))
					smartObject.ActionStateMachine.ChangeActionState(StateTransitions[i].TransitionState);

	}

    protected void CreateHitboxes(SmartObject smartObject) 
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
						OnHitReaction(smartObject,activeHitbox ,hitBox);
			}
			else
			{
				smartObject.Hitboxes[hitboxes[i].Hitbox].GetComponent<Hitbox>().Active = false;	
			}
    }

	public void OnHitReaction(SmartObject smartObject,Hitbox hitbox,CombatBox hurtBox)// do FX stuff and change staes if needed
	{
		switch (hurtBox.CurrentBoxTangibility)
		{
			case PhysicalObjectTangibility.Normal:
				{
					CreateHitFX(0, hitbox);
				}
				break;
			case PhysicalObjectTangibility.Armor:
				{
					if (FlagsExtensions.HasFlag(hitbox.DamageInstance.breakthroughType, BreakthroughType.ArmorPierce))
					{
						CreateHitFX(0, hitbox);
					}
					else
					{
						CreateHitFX(1, hitbox);
					}
				}
				break;
			case PhysicalObjectTangibility.Guard:
				{
					if(FlagsExtensions.HasFlag(hitbox.DamageInstance.breakthroughType, BreakthroughType.GuardPierce) || hitbox.DamageInstance.unstoppable)
					{
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
	}

	public void CreateProjectiles(SmartObject smartObject)
	{
		if (Projectiles == null || Projectiles.Length == 0)
			return;

		for(int i = 0; i < Projectiles.Length; i++)
		{
			if (smartObject.CurrentFrame == Projectiles[i].Time)
			{
				smartObject.Guns[Projectiles[i].Transform].Active = Projectiles[i].Toggle;
			}
		}
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