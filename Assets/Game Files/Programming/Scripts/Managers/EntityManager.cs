using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : Singleton<EntityManager>
{
	public LayerMask Hittable;
	public LayerMask GeoLayers;

	public List<SmartObject> Entities;

	public bool grassBusy;
	public SFX GrassCut;

	private void Update()
	{
		if (Entities.Count > 0)
			for (int i = Entities.Count - 1; i >= 0; i--)
				if (Entities[i] == null)
					Entities.RemoveAt(i);
	}

	public void GrassFX()
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
}
