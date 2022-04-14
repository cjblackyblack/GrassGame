using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterState/ActionState/Grounded/Guard")]
public class GuardState : SmartState
{
	public MotionCurve MotionCurve;
	public HitboxData[] hitboxes;
	public TangibilityFrames[] TangibilityFrames;
	public GameObject[] HitParticles = new GameObject[4];// match index to PhysicalTangibility Enum for reaction none for intangible ever

	public override void OnEnter(SmartObject smartObject)
	{

		base.OnEnter(smartObject);
		//if (smartObject.Target)
		//	smartObject.SetInputDir(Vector2.up, true);

		smartObject.Controller.Button1Buffer = 0;

		smartObject.MovementVector = smartObject.Motor.CharacterForward;
	}

	public override void OnExit(SmartObject smartObject)
	{
		smartObject.GravityModifier = 1;
		//CombatUtilities.ResetTangibilityFrames(smartObject, TangibilityFrames);
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

		if (smartObject.Target && Vector3.Distance(smartObject.transform.position, smartObject.Target.transform.position) > 0.8f)
		{
			if (MotionCurve.RotationTrackingCurve.Evaluate(smartObject.CurrentFrame) > 0)
				currentRotation = MotionCurve.TrackingRotation(smartObject, 25);
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
			//currentVelocity += smartObject.StoredVelocity * deltaTime;
			//smartObject.StoredVelocity *= friction;
		

	}

	public override void AfterCharacterUpdate(SmartObject smartObject, float deltaTime)
	{
		base.AfterCharacterUpdate(smartObject, deltaTime);
		CreateHitboxes(smartObject);

		if (smartObject.CurrentFrame > MaxTime)
			smartObject.ActionStateMachine.ChangeActionState(ActionStates.Idle);
	}

	protected void CreateHitboxes(SmartObject smartObject) //CHECK OVERLAPS SHOULD HAPPEN IN THE HITBOX CLASS TO SUPPORT CYLINDERS OR OTHER STRANGE MULTIOBJECT HITBOXES THIS CLASS NEEDS TO ONLY ACTIVATE THE ONES ACCORDING TO HITBOX DATA
	{
		for (int i = 0; i < hitboxes.Length; i++)
			if (smartObject.CurrentFrame >= hitboxes[i].ActiveFrames.x && smartObject.CurrentFrame <= hitboxes[i].ActiveFrames.y)
			{
				Hitbox activeHitbox = smartObject.Hitboxes[hitboxes[i].Hitbox].GetComponent<Hitbox>();

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
			}
	}

	private void OnValidate()
	{
		for (int i = 0; i < hitboxes.Length; i++)
			hitboxes[i].MaxTime = MaxTime;

		hitboxes[0].RefreshID = true;
	}
}