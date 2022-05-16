using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassObject : TangibleObject
{
	public Material FullGrass;
	public Material CutGrass;

	public int regrowTime;
	int regrowCounter;

	public LODGroup LODGroup;

	private void FixedUpdate()
	{
		if (regrowCounter > 0)
			regrowCounter--;

		if(regrowCounter == 1)
		{
			foreach(MeshRenderer meshRenderer in LODGroup.transform.GetComponentsInChildren<MeshRenderer>())
			{
				meshRenderer.material = FullGrass;
			}
		}
	}
	public override void TakeDamage(ref DamageInstance damageInstance)
	{
		//base.TakeDamage(ref damageInstance);
		regrowCounter = regrowTime;
		foreach (MeshRenderer meshRenderer in LODGroup.transform.GetComponentsInChildren<MeshRenderer>())
		{
			meshRenderer.material = CutGrass;
		}
	}
}
