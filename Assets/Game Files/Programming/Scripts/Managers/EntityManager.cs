using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : Singleton<EntityManager>
{
	public LayerMask Hittable;
	public LayerMask GeoLayers;

	public List<SmartObject> Entities;

	public bool grassBusy;
	public bool rewardBusy;
	public SFX GrassCut;
	public SFX RewardFX;

	private void Update()
	{
		if (Entities.Count > 0)
			for (int i = Entities.Count - 1; i >= 0; i--)
				if (Entities[i] == null)
					Entities.RemoveAt(i);
	}

	public void GrassSFX()
	{
		if (grassBusy)
			return;

		GrassCut.PlaySFX(AudioManager.Instance.SFXSource);
		StartCoroutine(SetGrass());

		IEnumerator SetGrass()
		{
			grassBusy = true;
			yield return new WaitForSecondsRealtime(0.1f);
			grassBusy = false;	
		}
	}

	public void RewardSFX()
	{
		if (rewardBusy)
			return;

		RewardFX.PlaySFX(AudioManager.Instance.SFXSource2);
		StartCoroutine(SetBusy());

		IEnumerator SetBusy()
		{
			rewardBusy = true;
			yield return new WaitForSecondsRealtime(0.1f);
			rewardBusy = false;
		}
	}
}
