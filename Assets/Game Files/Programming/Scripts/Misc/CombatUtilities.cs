using System;
using UnityEngine;

public static class CombatUtilities
{
	public static bool ValidTarget(TangibleObject selfObject, TangibleObject targetObject, ActionTarget targetAlliance, bool targetDead)
	{
		if (!targetDead)	
			if (targetObject.Stats.HP <= 0)
				return false;

		if (FlagsExtensions.HasFlag(targetAlliance, ActionTarget.Self))
			if (selfObject == targetObject)
				return true;
			else
				return false;

		if (FlagsExtensions.HasFlag(targetAlliance, ActionTarget.Ally))
			if (selfObject.BaseObjectProperties.BaseAlliance == targetObject.BaseObjectProperties.BaseAlliance)
				return true;

		if (FlagsExtensions.HasFlag(targetAlliance, ActionTarget.Enemy))
			if (selfObject.BaseObjectProperties.BaseAlliance != targetObject.BaseObjectProperties.BaseAlliance && targetObject.BaseObjectProperties.BaseAlliance != Alliance.Neutral)
				return true;

		if (FlagsExtensions.HasFlag(targetAlliance, ActionTarget.Neutral))
			if (targetObject.BaseObjectProperties.BaseAlliance == Alliance.Neutral)
				return true;

		return false;
	}

	public static bool ValidTarget(SmartObject selfObject, TangibleObject targetObject, ActionTarget targetAlliance, bool targetDead)
	{
		if (!targetDead)
			if (targetObject.Stats.HP <= 0)
				return false;

		if (FlagsExtensions.HasFlag(targetAlliance, ActionTarget.Self))
			if (selfObject == targetObject)
				return true;
			else
				return false;

		if (FlagsExtensions.HasFlag(targetAlliance, ActionTarget.Ally))
			if (selfObject.SmartObjectProperties.Alliance == targetObject.BaseObjectProperties.BaseAlliance)
				return true;

		if (FlagsExtensions.HasFlag(targetAlliance, ActionTarget.Enemy))
			if (selfObject.SmartObjectProperties.Alliance != targetObject.BaseObjectProperties.BaseAlliance && targetObject.BaseObjectProperties.BaseAlliance != Alliance.Neutral)
				return true;

		if (FlagsExtensions.HasFlag(targetAlliance, ActionTarget.Neutral))
			if (targetObject.BaseObjectProperties.BaseAlliance == Alliance.Neutral)
				return true;

		return false;
	}

	public static bool BoxGroupMatch(GroupFlag hitboxGroupA, GroupFlag hitboxGroupB)
	{
		 //Loop through entire available enum values of FlagCategory.
		foreach (GroupFlag flag in GroupFlagValues.Values)
			if (FlagsExtensions.HasFlag(hitboxGroupA, flag))
				if (FlagsExtensions.HasFlag(hitboxGroupB, flag))
					return true;

		return false;
	}

	public static void CreateTangibilityFrames(PhysicalObject physicalObject, TangibilityFrames[] tangibilityFrames)
	{
		for (int i = 0; i < tangibilityFrames.Length; i++)
			if (tangibilityFrames[i].Hurtbox)
			{
				for (int j = 0; j < physicalObject.Hurtboxes.Count; j++)
					if (BoxGroupMatch(physicalObject.Hurtboxes[j].GetComponent<CombatBox>().CombatBoxGroup, tangibilityFrames[i].BoxGroup) && Mathf.RoundToInt(tangibilityFrames[i].ActiveFrames.Evaluate(physicalObject.CurrentFrame)) > 0)
					{
						switch (tangibilityFrames[i].ActiveTangibility)
						{
							case PhysicalObjectTangibility.Intangible:
								{
									physicalObject.Hurtboxes[j].GetComponent<CombatBox>().IntangibleFrames += Mathf.RoundToInt(tangibilityFrames[i].ActiveFrames.Evaluate(physicalObject.CurrentFrame));
									break;
								}
							case PhysicalObjectTangibility.Invincible:
								{
									physicalObject.Hurtboxes[j].GetComponent<CombatBox>().IFrames += Mathf.RoundToInt(tangibilityFrames[i].ActiveFrames.Evaluate(physicalObject.CurrentFrame));
									break;
								}
							case PhysicalObjectTangibility.Guard:
								{
									physicalObject.Hurtboxes[j].GetComponent<CombatBox>().GuardFrames += Mathf.RoundToInt(tangibilityFrames[i].ActiveFrames.Evaluate(physicalObject.CurrentFrame));
									break;
								}
							case PhysicalObjectTangibility.Armor:
								{
									physicalObject.Hurtboxes[j].GetComponent<CombatBox>().ArmorFrames += Mathf.RoundToInt(tangibilityFrames[i].ActiveFrames.Evaluate(physicalObject.CurrentFrame));
									break;
								}
						}
					}
			}
			else
				for (int j = 0; j < physicalObject.Hitboxes.Length; j++)
					if (BoxGroupMatch(physicalObject.Hitboxes[j].GetComponent<CombatBox>().CombatBoxGroup, tangibilityFrames[i].BoxGroup) && Mathf.RoundToInt(tangibilityFrames[i].ActiveFrames.Evaluate(physicalObject.CurrentFrame)) > 0)
						switch (tangibilityFrames[i].ActiveTangibility)
						{
							case PhysicalObjectTangibility.Intangible:
								{
									physicalObject.Hitboxes[j].GetComponent<CombatBox>().IntangibleFrames += Mathf.RoundToInt(tangibilityFrames[i].ActiveFrames.Evaluate(physicalObject.CurrentFrame));
									break;
								}
							case PhysicalObjectTangibility.Invincible:
								{
									physicalObject.Hitboxes[j].GetComponent<CombatBox>().IFrames += Mathf.RoundToInt(tangibilityFrames[i].ActiveFrames.Evaluate(physicalObject.CurrentFrame));
									break;
								}
							case PhysicalObjectTangibility.Guard:
								{
									physicalObject.Hitboxes[j].GetComponent<CombatBox>().GuardFrames += Mathf.RoundToInt(tangibilityFrames[i].ActiveFrames.Evaluate(physicalObject.CurrentFrame));
									break;
								}
							case PhysicalObjectTangibility.Armor:
								{
									physicalObject.Hitboxes[j].GetComponent<CombatBox>().ArmorFrames += Mathf.RoundToInt(tangibilityFrames[i].ActiveFrames.Evaluate(physicalObject.CurrentFrame));
									break;
								}
						}

	}

	public static void ResetTangibilityFrames(SmartObject smartObject, TangibilityFrames[] tangibilityFrames)
	{
		for (int i = 0; i < tangibilityFrames.Length; i++)
			if (tangibilityFrames[i].Hurtbox) {
				for (int j = 0; j < smartObject.Hurtboxes.Count; j++)
					if (BoxGroupMatch(smartObject.Hitboxes[j].GetComponent<CombatBox>().CombatBoxGroup, tangibilityFrames[i].BoxGroup))
					{
						smartObject.Hitboxes[j].GetComponent<CombatBox>().IntangibleFrames = 0;
						smartObject.Hitboxes[j].GetComponent<CombatBox>().IFrames = 0;
						smartObject.Hitboxes[j].GetComponent<CombatBox>().GuardFrames = 0;
						smartObject.Hitboxes[j].GetComponent<CombatBox>().ArmorFrames = 0;
					}
			}
			else
				for (int j = 0; j < smartObject.Hurtboxes.Count; j++)
					if (BoxGroupMatch(smartObject.Hitboxes[j].GetComponent<CombatBox>().CombatBoxGroup, tangibilityFrames[i].BoxGroup))
					{
						smartObject.Hitboxes[j].GetComponent<CombatBox>().IntangibleFrames = 0;
						smartObject.Hitboxes[j].GetComponent<CombatBox>().IFrames = 0;
						smartObject.Hitboxes[j].GetComponent<CombatBox>().GuardFrames = 0;
						smartObject.Hitboxes[j].GetComponent<CombatBox>().ArmorFrames = 0;
					}

	}

	public static Vector3 CalculateOverlapNormal(Collider colliderA, Collider colliderB)
	{
		int colliderALayer = colliderA.gameObject.layer;
		int colliderBLayer = colliderB.gameObject.layer;
		Physics.ComputePenetration(
		colliderA, colliderA.transform.position, colliderA.transform.rotation,
		colliderB, colliderB.transform.position, colliderB.transform.rotation,
		out Vector3 direction, out float distance);

		Physics.ClosestPoint(colliderA.ClosestPoint(colliderB.transform.position), colliderB, colliderB.transform.position, colliderB.transform.rotation);

		Physics.Raycast(Physics.ClosestPoint(colliderA.ClosestPoint(colliderB.transform.position), colliderB, colliderB.transform.position, colliderB.transform.rotation), direction, out RaycastHit hit, Mathf.Infinity, LayerMask.NameToLayer("Hidden"), QueryTriggerInteraction.Collide) ;
		if(hit.collider)
			return hit.normal;
		return Vector3.zero;
		//return -direction;
	}
}

public static class FlagsExtensions
{
	public static bool HasFlag(this BreakthroughType flag, BreakthroughType toCheck)
	{
		return (flag & toCheck) == toCheck;
	}

	public static bool HasFlag(this HitstopType flag, HitstopType toCheck)
	{
		return (flag & toCheck) == toCheck;
	}

	public static bool HasFlag(this KnockbackType flag, KnockbackType toCheck)
	{
		return (flag & toCheck) == toCheck;
	}

	public static bool HasFlag(this GroupFlag flag, GroupFlag toCheck)
	{
		return (flag & toCheck) == toCheck;
	}

	public static bool HasFlag(this ActionTarget flag, ActionTarget toCheck)
	{
		return (flag & toCheck) == toCheck;
	}
}
public static class GroupFlagValues
{
	public static readonly Array Values = Enum.GetValues(typeof(GroupFlag));
}
