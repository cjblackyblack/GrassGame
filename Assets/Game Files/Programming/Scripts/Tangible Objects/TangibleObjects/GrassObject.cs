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
	public GameObject GrassVFX;

	private void FixedUpdate()
	{
		if (regrowCounter > 0)
			regrowCounter--;

		if(regrowCounter == 1)
		{
			Stats.HP = Stats.MaxHP;
			foreach(MeshRenderer meshRenderer in LODGroup.transform.GetComponentsInChildren<MeshRenderer>())
			{
				meshRenderer.material = FullGrass;
			}
		}
	}
	public override void TakeDamage(ref DamageInstance damageInstance)
	{
		//base.TakeDamage(ref damageInstance);
		if (Stats.HP <= 0)
			return;

		Stats.HP--;
		EntityManager.Instance.GrassFX();


		if (Stats.HP <= 0)
		{
			Instantiate(GrassVFX, transform.position, GrassVFX.transform.rotation);
			regrowCounter = regrowTime;
			foreach (MeshRenderer meshRenderer in LODGroup.transform.GetComponentsInChildren<MeshRenderer>())
			{
				meshRenderer.material = CutGrass;
			}
		}
	}
}
