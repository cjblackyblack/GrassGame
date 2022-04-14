using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public abstract class Hitbox : CombatBox
{

	public int AttackID;
	public DamageInstance DamageInstance;
	public Collider[] HitColliders = new Collider[16];
	public List<Collider> CachedColliders;
	public Collider Collider => GetComponent<Collider>();
	private List<CombatBox> sortedBoxes;

	//public Vector3 CurrentPos;
	public Vector3 PreviousPos;

	private void Start()
	{
		sortedBoxes = new List<CombatBox>();
		SourceObject.Hitboxes[CombatBoxID] = GetComponent<Collider>();
		Active = false;
	}

	public abstract CombatBox CheckOverlap(HitboxData hitboxData);

	public CombatBox SpeculativeOverlap(HitboxData hitboxData)
	{
		gameObject.layer = LayerMask.NameToLayer("Hidden");
		Physics.OverlapCapsuleNonAlloc(PreviousPos, transform.position, Collider.bounds.extents.x, HitColliders, EntityManager.Instance.Hittable);
		gameObject.layer = LayerMask.NameToLayer("Hitbox");
		return ProcessValidHitboxes(hitboxData);
	}

	public abstract void CheckOverlapGeo();

	public void SetHitboxData(HitboxData hitboxData) //set this so other hitboxes can know what they're colliding with
	{
		SmartObject smartObject = SourceObject as SmartObject;
		if (hitboxData.RefreshID)
		{
			AttackID = Random.Range(-1000000, 1000000);
			if (smartObject != null)
				if (hitboxData.ShareID)
				{
					for (int i = 0; i < smartObject.Hitboxes.Length; i++)
						if (CombatUtilities.BoxGroupMatch(hitboxData.HitboxGroup, smartObject.Hitboxes[i].GetComponent<Hitbox>().CombatBoxGroup))//&& relatedHitbox.CombatBoxGroupIntOBS == hitBox.CombatBoxGroupIntOBS)
						{
							smartObject.Hitboxes[i].GetComponent<Hitbox>().AttackID = AttackID;
							smartObject.Hitboxes[i].GetComponent<Hitbox>().HitColliders = new Collider[16];
							smartObject.Hitboxes[i].GetComponent<Hitbox>().CachedColliders = new List<Collider>();
						}
				}
			CachedColliders = new List<Collider>();
		}
		CachedColliders = new List<Collider>();
		ShareIncomingHitboxes = hitboxData.ShareIncomingHitboxes;
		HitColliders = new Collider[16];
		DamageInstance = new DamageInstance(SourceObject, AttackID, hitboxData.StatusEffects, hitboxData.Unstoppable, hitboxData.Damage, hitboxData.Hitstun, hitboxData.HitStopTime, hitboxData.HitstopType,
											hitboxData.BreakthroughType, hitboxData.KnockbackType, hitboxData.KnockbackStrength, hitboxData.KnockbackDirection,
											hitboxData.FlatDamage, hitboxData.IgnoreProtections, hitboxData.UseMagic, hitboxData.AttackBoxesIndividually);
	}

	public override PhysicalObjectTangibility ProcessHitAction(ref DamageInstance damageInstance)
	{
		return CurrentBoxTangibility;
	}

	public virtual bool ProcessHitReaction(PhysicalObjectTangibility hitTangibility, ref DamageInstance damageInstance)
	{
		return SourceObject.HitConfirmReaction(hitTangibility, damageInstance);
	}


	public virtual CombatBox ProcessValidHitboxes(HitboxData hitboxData)
	{
		if(HitColliders.Length == 0)
			return null;

		sortedBoxes.Clear();

		for (int i = 0; i < HitColliders.Length; i++)
			if (HitColliders[i] != null) 
			{
				if (HitColliders[i].TryGetComponent(out CombatBox possibleBox))//this could be a new best box
				{
					if (possibleBox.Active)
					{
						if (sortedBoxes.Count == 0)
							sortedBoxes.Add(possibleBox);

						else if (possibleBox as Hitbox != null || !sortedBoxes[0].GetComponent<Hitbox>())
						{
							sortedBoxes.Insert(0, possibleBox);

						}
						else
						{
							sortedBoxes.Add(possibleBox);
						}
					}
				}
			}

		for (int j = 0; j < sortedBoxes.Count; j++)
		{
			if (ValidHitID(sortedBoxes[j]))
			{
				SmartObject smartSource = SourceObject as SmartObject;
				if (smartSource != null)
				{
					if (CombatUtilities.ValidTarget(smartSource, sortedBoxes[j].SourceObject, hitboxData.ActionTarget, true))
						return OnHitConfirm(hitboxData, SourceObject, (sortedBoxes[j]));
				}
				else
				{
					if (CombatUtilities.ValidTarget(SourceObject, sortedBoxes[j].SourceObject, hitboxData.ActionTarget, true))
						return OnHitConfirm(hitboxData, SourceObject, (sortedBoxes[j])); //the return is here so that hitboxes are calculated first to detect incoming clangs, if there isn't one, the hurtbox is damaged next frame in a situation where hitboxes and hurtboxes overlap
				}
			}
		}
		return null;															// maybe I can run a check to only do this if there actually is a clank instead of having to calculate it, because what if two normals hit eachother, is it okay that both attacks are delayed a frame?
	}																			// probs just ovethinking and this is probably going to be fine

	public bool ValidHitID(CombatBox _hitBox)
	{
		if (_hitBox == this)
			return false;

		if (_hitBox.Active == false)
			return false;

		if (_hitBox.ID == AttackID)
			return false;

		for (int i = 0; i < SourceObject.Hitboxes.Length; i++) //if we hit one of our own hitboxes
			if (SourceObject.Hitboxes[i] == _hitBox.GetComponent<Collider>())
				return false;

		if (SourceObject.Hurtboxes.Contains(_hitBox.GetComponent<Collider>()))
			return false;

		if (CachedColliders.Contains(_hitBox.GetComponent<Collider>()))
			return false;

		return true;
	}

	public CombatBox OnHitConfirm(HitboxData hitboxData, TangibleObject sourceObject, CombatBox hitBox)
	{

		//Debug.Log($"Processing {sourceObject.gameObject.name} hit {hitBox.gameObject.name} with hitbox {sourceObject.Hitboxes[CombatBoxID].gameObject.name}");

		DamageInstance = new DamageInstance(SourceObject, AttackID, hitboxData.StatusEffects, hitboxData.Unstoppable, hitboxData.Damage, hitboxData.Hitstun, hitboxData.HitStopTime, hitboxData.HitstopType,
											hitboxData.BreakthroughType, hitboxData.KnockbackType, hitboxData.KnockbackStrength, hitboxData.KnockbackDirection,
											hitboxData.FlatDamage, hitboxData.IgnoreProtections, hitboxData.UseMagic, hitboxData.AttackBoxesIndividually);
		bool processing = true;

		for (int i = 0; i < hitboxData.HitboxProcesses.Length; i++)
			if (processing)
				processing = hitboxData.HitboxProcesses[i].Process(this, hitBox);
			else break;
		return hitBox;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(PreviousPos, Collider.bounds.extents.x);
		//Gizmos.DrawWireSphere(CurrentPos, Collider.bounds.extents.x);

	}
}