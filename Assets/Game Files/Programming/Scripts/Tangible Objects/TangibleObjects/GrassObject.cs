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
	public GameObject Reward;

	float rewardChance;
	public float rewardThreshold;
    public float comboRewardBonus = .5f;

	new private void Start()
	{
		StartCoroutine(SubscribeToManager());

		IEnumerator SubscribeToManager()
		{
			yield return new WaitForEndOfFrame();
			GrassManager.grassObjects.Add(this);
		}
	}
	public void OnFixedUpdate()
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
		rewardChance = Random.Range(0f, 100f);
		//base.TakeDamage(ref damageInstance);
		if (regrowCounter > 0)
			return;

		Stats.HP--;
		EntityManager.Instance.GrassSFX();


		if (Stats.HP <= 0)
		{
			if (rewardChance > rewardThreshold)
			{
				Instantiate(Reward, transform.position + (transform.up * 0.5f), Reward.transform.rotation);
				EntityManager.Instance.RewardSFX();
                ComboManager.Instance.AddCombo(comboRewardBonus);
			}
			Instantiate(GrassVFX, transform.position, GrassVFX.transform.rotation);
			regrowCounter = regrowTime;
			foreach (MeshRenderer meshRenderer in LODGroup.transform.GetComponentsInChildren<MeshRenderer>())
			{
				meshRenderer.material = CutGrass;
			}
		}
	}
}
