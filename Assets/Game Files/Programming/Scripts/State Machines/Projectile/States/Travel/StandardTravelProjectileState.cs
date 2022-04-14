using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ProjectileState/Travel/Standard")]
public class StandardTravelProjectileState : ProjectileState
{
	public ProjectileState ExitState;
	public MotionCurve MotionCurve;
	public HitboxData[] hitboxes;
	public TangibilityFrames[] TangibilityFrames;
	public GameObject[] HitParticles = new GameObject[4];// match index to PhysicalTangibility Enum for reaction none for intangible ever
	public SFX HitSFX;
	public bool Speculative;
	public override void OnEnter(ProjectileObject projectileObject)
	{
		projectileObject.CurrentFrame = -1;
		projectileObject.CurrentTime = -1;
	}

	public override void OnExit(ProjectileObject projectileObject)
	{
	
	}

	public override void OnFixedUpdate(ProjectileObject projectileObject)
	{

		//CombatUtilities.CreateTangibilityFrames(projectileObject, TangibilityFrames);
		//if (projectileObject.CurrentFrame > MaxTime)
		//{
		//	projectileObject.CurrentTime = -1;
		//	projectileObject.CurrentFrame = -1;
		//}

		if (projectileObject.BounceBuffered)
		{
			//Debug.Log("Reflecting");
			projectileObject.transform.forward = Vector3.Reflect(projectileObject.transform.forward, projectileObject.CollisionNormal);
			projectileObject.BounceBuffered = false;
			projectileObject.CollisionNormal *= 0;
		}

		projectileObject.Velocity = MotionCurve.GetFixedTotalCurve(projectileObject);

		if (projectileObject.CanTrack)
		{
			projectileObject.Velocity += MotionCurve.TrackingVelocity(projectileObject);
		}

		projectileObject.RBody.velocity = projectileObject.Velocity;

		MotionCurve.Rotation(projectileObject);

		if(projectileObject.Target && projectileObject.CanTrack)
			projectileObject.transform.rotation = MotionCurve.TrackingRotation(projectileObject);



		if (Speculative)
		{
			CreateSpeculativeHitboxes(projectileObject);
		}
		else
			CreateHitboxes(projectileObject);
		//projectileObject.Direction = 

		if (projectileObject.CurrentFrame > MaxTime)
			projectileObject.StateMachine.ChangeState(ExitState);

	}

	public override void OnUpdate(ProjectileObject projectileObject)
	{
		
	}

	public override void HandleState(ProjectileObject projectileObject)
	{

	}

	protected void CreateHitboxes(ProjectileObject projectileObject)
	{
		for (int i = 0; i < hitboxes.Length; i++)
			if (projectileObject.CurrentFrame >= hitboxes[i].ActiveFrames.x && projectileObject.CurrentFrame <= hitboxes[i].ActiveFrames.y)
			{

				Hitbox activeHitbox = projectileObject.Hitboxes[hitboxes[i].Hitbox].GetComponent<Hitbox>();
				activeHitbox.Active = true;

				if (projectileObject.CurrentFrame == hitboxes[i].ActiveFrames.x)
				{

					activeHitbox.SetHitboxData(hitboxes[i]);

					for (int j = 0; j < hitboxes.Length; j++) //catch any late blooming hitboxes that got unlucky with no box ID set
					{
						if (!hitboxes[j].ShareID && !hitboxes[j].RefreshID)
							if (CombatUtilities.BoxGroupMatch(hitboxes[j].HitboxGroup, hitboxes[i].HitboxGroup))
							{
								projectileObject.Hitboxes[hitboxes[j].Hitbox].GetComponent<Hitbox>().AttackID = activeHitbox.AttackID;
								projectileObject.Hitboxes[hitboxes[j].Hitbox].GetComponent<Hitbox>().CombatBoxGroup = hitboxes[i].HitboxGroup;
							}
					}
				}//share shit with other hitboxes

				
				CombatBox hitBox = activeHitbox.CheckOverlap(hitboxes[i]); //Check Overlap, Filter Hitboxes, Apply Processes to hurtboxes, should only return combat boxes that were processed
				if (hitBox != null)
					if (hitBox.CurrentBoxTangibility != PhysicalObjectTangibility.Intangible)
						OnHitReaction(projectileObject, activeHitbox, hitBox);

			}
			else
			{
				projectileObject.Hitboxes[hitboxes[i].Hitbox].GetComponent<Hitbox>().Active = false;

			}
	}

	protected void CreateSpeculativeHitboxes(ProjectileObject projectileObject)
	{
		for (int i = 0; i < hitboxes.Length; i++)
			if (projectileObject.CurrentFrame >= hitboxes[i].ActiveFrames.x && projectileObject.CurrentFrame <= hitboxes[i].ActiveFrames.y)
			{

				Hitbox activeHitbox = projectileObject.Hitboxes[hitboxes[i].Hitbox].GetComponent<Hitbox>();
				activeHitbox.Active = true;

				if (projectileObject.CurrentFrame == hitboxes[i].ActiveFrames.x)
				{

					activeHitbox.SetHitboxData(hitboxes[i]);

					for (int j = 0; j < hitboxes.Length; j++) //catch any late blooming hitboxes that got unlucky with no box ID set
					{
						if (!hitboxes[j].ShareID && !hitboxes[j].RefreshID)
							if (CombatUtilities.BoxGroupMatch(hitboxes[j].HitboxGroup, hitboxes[i].HitboxGroup))
							{
								projectileObject.Hitboxes[hitboxes[j].Hitbox].GetComponent<Hitbox>().AttackID = activeHitbox.AttackID;
								projectileObject.Hitboxes[hitboxes[j].Hitbox].GetComponent<Hitbox>().CombatBoxGroup = hitboxes[i].HitboxGroup;
							}
					}
				}//share shit with other hitboxes

				CombatBox hitBox = activeHitbox.SpeculativeOverlap(hitboxes[i]); //Check Overlap, Filter Hitboxes, Apply Processes to hurtboxes, should only return combat boxes that were processed
				if (hitBox != null)
					if (hitBox.CurrentBoxTangibility != PhysicalObjectTangibility.Intangible)
						OnHitReaction(projectileObject, activeHitbox, hitBox);

			}
			else
			{
				projectileObject.Hitboxes[hitboxes[i].Hitbox].GetComponent<Hitbox>().Active = false;

			}
	}

	public void OnHitReaction(ProjectileObject projectileObject, Hitbox hitbox, CombatBox hurtBox)// do FX stuff and change staes if needed
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
					if (FlagsExtensions.HasFlag(hitbox.DamageInstance.breakthroughType, BreakthroughType.GuardPierce) || hitbox.DamageInstance.unstoppable)
					{
						if(!hitbox.DamageInstance.unstoppable)
							CreateHitFX(0, hitbox);
					}
					else
					{
						
					}

				}
				break;
			case PhysicalObjectTangibility.Invincible:
				{

				}
				break;
		}
		projectileObject.CurrentTime = MaxTime + 100;
		projectileObject.CurrentFrame = MaxTime + 100;
		//Instantiate(HitParticles[(int)hitBox.CurrentBoxTangibility], hitBox.transform.position, Quaternion.identity);
	}

	void CreateHitFX(int index, CombatBox hitbox)
	{
		//Instantiate(HitParticles[index], hitbox.transform.position, Quaternion.identity);
		HitSFX.PlaySFX(hitbox.SourceObject);
	}

	private void OnValidate()
	{
		if (hitboxes.Length > 0)
		{
			for (int i = 0; i < hitboxes.Length; i++)
				hitboxes[i].MaxTime = MaxTime;

			hitboxes[0].RefreshID = true;
		}
	}
}