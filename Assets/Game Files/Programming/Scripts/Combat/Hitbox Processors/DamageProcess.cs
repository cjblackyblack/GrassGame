using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "HitboxProcess/DamageProcess")]
public class DamageProcess : HitboxProcess
{

	public override bool Process(Hitbox sourceBox, CombatBox hurtBox)
	{
		hurtBox.ID = sourceBox.AttackID;

		if (!sourceBox.CachedColliders.Contains(hurtBox.GetComponent<Collider>()))
			sourceBox.CachedColliders.Add(hurtBox.GetComponent<Collider>());

		Hitbox hitBox = hurtBox as Hitbox;

		if (hitBox != null && hitBox.Active)
		{
			return ProcessHitbox(sourceBox, hitBox);
		}
		else
			return ProcessHurtbox(sourceBox, hurtBox);
	}

	public bool ProcessHitbox(Hitbox sourceBox, Hitbox hitBox)
	{
		//we need to properly cache all data before the first clang happens 
		//each hitbox should be

		if (!hitBox.CachedColliders.Contains(sourceBox.GetComponent<Collider>()))// if the hitbox doesn't have us cached
			hitBox.CachedColliders.Add(sourceBox.GetComponent<Collider>());//they update their cache so they don't hit us again

		if (hitBox.ShareIncomingHitboxes)//if the attacked hitbox shares its hitboxes
		{
			for (int k = 0; k < hitBox.SourceObject.Hitboxes.Length; k++)//get all the hitboxes
				if (hitBox.SourceObject.Hitboxes[k] != null)
				{
					Hitbox relatedHitbox = hitBox.SourceObject.Hitboxes[k].GetComponent<Hitbox>();
					if (relatedHitbox.ShareIncomingHitboxes)
						if(CombatUtilities.BoxGroupMatch(hitBox.CombatBoxGroup, relatedHitbox.CombatBoxGroup))//&& relatedHitbox.CombatBoxGroupIntOBS == hitBox.CombatBoxGroupIntOBS)
						{
						relatedHitbox.ID = sourceBox.AttackID;//and update their ID so we don't hit them again

						if (!relatedHitbox.CachedColliders.Contains(sourceBox.GetComponent<Collider>()))// if the shared hitboxes don't have us cached
							relatedHitbox.CachedColliders.Add(sourceBox.GetComponent<Collider>());//they update their cache so they don't hit us again

						if (!sourceBox.CachedColliders.Contains(hitBox.SourceObject.Hitboxes[k]))//and they don't have the hurtbox cached
							sourceBox.CachedColliders.Add(hitBox.SourceObject.Hitboxes[k]);//give it to them
						}
				}
		}


		if (!sourceBox.DamageInstance.AttackIndividualBoxes) //if we are attacking a group, we need to add all of the hitBox's source's matching hitBoxes to the sourcebox's cache, if the sourcebox shares its attack update other source hitboxes that match the ID
		{
			for (int i = 0; i < hitBox.SourceObject.Hitboxes.Length; i++) //get all hitboxes from hitbox
			{
				if (hitBox.SourceObject.Hitboxes[i] != null)
				{
					Hitbox relatedHitbox = hitBox.SourceObject.Hitboxes[i].GetComponent<Hitbox>();
					if (CombatUtilities.BoxGroupMatch(relatedHitbox.CombatBoxGroup, hitBox.CombatBoxGroup))
					{

						if (!sourceBox.CachedColliders.Contains(hitBox.SourceObject.Hitboxes[i])) // if the sourcebox does not have the hitbox cached
							sourceBox.CachedColliders.Add(hitBox.SourceObject.Hitboxes[i]); //cache it
																							//somewhat redundant here and need to test if loop will properly grab without the two above


						for (int j = 0; j < sourceBox.SourceObject.Hitboxes.Length; j++)//get all hitboxes
						{
							Hitbox _hitbox = sourceBox.SourceObject.Hitboxes[j].GetComponent<Hitbox>();
							if (CombatUtilities.BoxGroupMatch(sourceBox.CombatBoxGroup, _hitbox.CombatBoxGroup))//if they're in the group
								if (!_hitbox.CachedColliders.Contains(hitBox.SourceObject.Hitboxes[i]))//and they don't have the hurtbox cached
									_hitbox.CachedColliders.Add(hitBox.SourceObject.Hitboxes[i]);//give it to them
						}
					}
				}
			}
		}
		else
		{
			if (!sourceBox.CachedColliders.Contains(hitBox.GetComponent<Collider>()))
				sourceBox.CachedColliders.Add(hitBox.GetComponent<Collider>());
		}

		//DONT FORGET TO UPDATE THE OVERLOAD
		return sourceBox.ProcessHitReaction(hitBox.ProcessHitAction(ref sourceBox.DamageInstance), ref sourceBox.DamageInstance);
	}

	public bool ProcessHurtbox(Hitbox sourceBox, CombatBox hurtBox)
	{
		if (hurtBox.ShareIncomingHitboxes)//if the attacked hitbox shares its hurtboxes
			for (int k = 0; k < hurtBox.SourceObject.Hurtboxes.Count; k++)//get all the hurtboxes
				if (hurtBox.SourceObject.Hurtboxes[k] != null)
				{
					CombatBox relatedHurtbox = hurtBox.SourceObject.Hurtboxes[k].GetComponent<CombatBox>();
					if (relatedHurtbox.ShareIncomingHitboxes)
						if(CombatUtilities.BoxGroupMatch(hurtBox.CombatBoxGroup, relatedHurtbox.CombatBoxGroup))//&& relatedHitbox.CombatBoxGroupIntOBS == hitBox.CombatBoxGroupIntOBS)
						{
						relatedHurtbox.ID = sourceBox.AttackID;//and update their ID 
						if (!sourceBox.CachedColliders.Contains(hurtBox.SourceObject.Hurtboxes[k]))//and they don't have the hurtbox cached
							sourceBox.CachedColliders.Add(hurtBox.SourceObject.Hurtboxes[k]);//give it to them
						}
				}
		if (!sourceBox.DamageInstance.AttackIndividualBoxes) //if we are attacking a group, we need to add all of the hitBox's source's hurtboxes to the sourcebox's cache, if the sourcebox shares its attack update other source hitboxes that match the ID
		{
			for (int i = 0; i < hurtBox.SourceObject.Hurtboxes.Count; i++) //get all hurtboxes from hitbox
			{
				if (hurtBox.SourceObject.Hurtboxes[i] != null)
				{
					if (CombatUtilities.BoxGroupMatch(hurtBox.CombatBoxGroup, hurtBox.SourceObject.Hurtboxes[i].GetComponent<CombatBox>().CombatBoxGroup))
					{
						if (!sourceBox.CachedColliders.Contains(hurtBox.SourceObject.Hurtboxes[i])) // if the sourcebox does not have the hitbox cached
							sourceBox.CachedColliders.Add(hurtBox.SourceObject.Hurtboxes[i]); //cache it
																							  //somewhat redundant here and need to test if loop will properly grab without the two above

						for (int j = 0; j < sourceBox.SourceObject.Hitboxes.Length; j++)//get all hitboxes
							if (CombatUtilities.BoxGroupMatch(sourceBox.SourceObject.Hitboxes[j].GetComponent<Hitbox>().CombatBoxGroup, sourceBox.CombatBoxGroup))//if they're in the group
								if (!sourceBox.SourceObject.Hitboxes[j].GetComponent<Hitbox>().CachedColliders.Contains(hurtBox.SourceObject.Hurtboxes[i]))//and they don't have the hurtbox cached
									sourceBox.SourceObject.Hitboxes[j].GetComponent<Hitbox>().CachedColliders.Add(hurtBox.SourceObject.Hurtboxes[i]);//give it to them
					}
				}
			}
		}
		else
		{
			if (!sourceBox.CachedColliders.Contains(hurtBox.GetComponent<Collider>()))
				sourceBox.CachedColliders.Add(hurtBox.GetComponent<Collider>());
		}
		//DONT FORGET TO UPDATE THE OVERLOAD
		return sourceBox.ProcessHitReaction(hurtBox.ProcessHitAction(ref sourceBox.DamageInstance), ref sourceBox.DamageInstance);
	}

	public bool ProcessHitbox(Hitbox sourceBox, ContactBox hitBox)
	{
		//we need to properly cache all data before the first clang happens 
		//each hitbox should be

		if (!hitBox.CachedColliders.Contains(sourceBox.GetComponent<Collider>()))// if the hitbox doesn't have us cached
			hitBox.CachedColliders.Add(sourceBox.GetComponent<Collider>());//they update their cache so they don't hit us again

		if (hitBox.ShareIncomingHitboxes)//if the attacked hitbox shares its hitboxes
		{
			for (int k = 0; k < hitBox.SourceObject.Hitboxes.Length; k++)//get all the hitboxes
				if (hitBox.SourceObject.Hitboxes[k] != null)
				{
					Hitbox relatedHitbox = hitBox.SourceObject.Hitboxes[k].GetComponent<Hitbox>();
					if (relatedHitbox.ShareIncomingHitboxes)
						if (CombatUtilities.BoxGroupMatch(hitBox.CombatBoxGroup, relatedHitbox.CombatBoxGroup))
						{
							relatedHitbox.ID = sourceBox.AttackID;//and update their ID so we don't hit them again

							if (!relatedHitbox.CachedColliders.Contains(sourceBox.GetComponent<Collider>()))// if the shared hitboxes don't have us cached
								relatedHitbox.CachedColliders.Add(sourceBox.GetComponent<Collider>());//they update their cache so they don't hit us again

							if (!sourceBox.CachedColliders.Contains(hitBox.SourceObject.Hitboxes[k]))//and they don't have the hurtbox cached
								sourceBox.CachedColliders.Add(hitBox.SourceObject.Hitboxes[k]);//give it to them
						}
				}
		}


		if (!sourceBox.DamageInstance.AttackIndividualBoxes) //if we are attacking a group, we need to add all of the hitBox's source's matching hitBoxes to the sourcebox's cache, if the sourcebox shares its attack update other source hitboxes that match the ID
		{
			for (int i = 0; i < hitBox.SourceObject.Hitboxes.Length; i++) //get all hitboxes from hitbox
			{
				if (hitBox.SourceObject.Hitboxes[i] != null)
				{
					Hitbox relatedHitbox = hitBox.SourceObject.Hitboxes[i].GetComponent<Hitbox>();
					if (CombatUtilities.BoxGroupMatch(relatedHitbox.CombatBoxGroup, hitBox.CombatBoxGroup))
					{

						if (!sourceBox.CachedColliders.Contains(hitBox.SourceObject.Hitboxes[i])) // if the sourcebox does not have the hitbox cached
							sourceBox.CachedColliders.Add(hitBox.SourceObject.Hitboxes[i]); //cache it
																							//somewhat redundant here and need to test if loop will properly grab without the two above


						for (int j = 0; j < sourceBox.SourceObject.Hitboxes.Length; j++)//get all hitboxes
						{
							Hitbox _hitbox = sourceBox.SourceObject.Hitboxes[j].GetComponent<Hitbox>();
							if (CombatUtilities.BoxGroupMatch(sourceBox.CombatBoxGroup, _hitbox.CombatBoxGroup)) //if (sourceBox.SourceObject.Hitboxes[j].GetComponent<Hitbox>().AttackID == sourceBox.AttackID)//if they're in the group
								if (!_hitbox.CachedColliders.Contains(hitBox.SourceObject.Hitboxes[i]))//and they don't have the hurtbox cached
									_hitbox.CachedColliders.Add(hitBox.SourceObject.Hitboxes[i]);//give it to them
						}
					}
				}
			}
		}
		else
		{
			if (!sourceBox.CachedColliders.Contains(hitBox.GetComponent<Collider>()))
				sourceBox.CachedColliders.Add(hitBox.GetComponent<Collider>());
		}

		//DONT FORGET TO UPDATE THE OVERLOAD
		return sourceBox.ProcessHitReaction(hitBox.ProcessHitAction(ref sourceBox.DamageInstance),ref sourceBox.DamageInstance);
	}
}