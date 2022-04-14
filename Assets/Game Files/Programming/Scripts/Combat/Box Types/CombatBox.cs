using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public abstract class CombatBox : MonoBehaviour
{
	private int[] Values = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
	public bool Active;
	[ValueDropdown("Values")]
	public int CombatBoxID; //value for assigning position on corresponding parent box array
	public GroupFlag CombatBoxGroup;
	public TangibleObject SourceObject => transform.GetComponentInParent<TangibleObject>();
	public PhysicalObjectTangibility CurrentBoxTangibility;
	public PhysicalObjectTangibility DefaultBoxTangibility;
	public int IntangibleFrames;
	public int IFrames;
	public int GuardFrames;
	public int ArmorFrames;
	public int ID;
	public bool ShareIncomingHitboxes; //will probably want to change that to a group
	public abstract PhysicalObjectTangibility ProcessHitAction(ref DamageInstance damageInstance);

	private void FixedUpdate()
	{
		BufferTangibilityFrames();
	}

	public void BufferTangibilityFrames()
	{
		if (IntangibleFrames > 0)
		{
			CurrentBoxTangibility = PhysicalObjectTangibility.Intangible;
			IntangibleFrames--;
			return;
		}
		if (IFrames > 0)
		{
			CurrentBoxTangibility = PhysicalObjectTangibility.Invincible;
			IFrames--;
			return;
		}
		else if (GuardFrames > 0)
		{
			CurrentBoxTangibility = PhysicalObjectTangibility.Guard;
			GuardFrames--;
			return;
		}
		else if (ArmorFrames > 0)
		{
			CurrentBoxTangibility = PhysicalObjectTangibility.Armor;
			ArmorFrames--;
			return;
		}
		else if (CurrentBoxTangibility != DefaultBoxTangibility)
		{
			CurrentBoxTangibility = DefaultBoxTangibility;
			return;
		}
	}
}