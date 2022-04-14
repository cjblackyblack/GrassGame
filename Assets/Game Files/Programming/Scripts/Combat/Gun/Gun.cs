using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
	public SmartObject SourceObject => transform.root.GetComponent<SmartObject>();

	public bool Active;
	public float CurrentTime;
	public int CurrentFrame;

	public GunData GunData;
	float inactiveTimer;
	public GameObject GunFX;
	public SFX ShootFX;
	public bool OneShot;
	private void Update()
	{
		if (!Active)
			inactiveTimer += Time.deltaTime;

		if(inactiveTimer > 3f)
		{
			inactiveTimer = 0;
			CurrentTime = 0;
			CurrentFrame = 0;
		}
	}
	private void FixedUpdate()
	{
		if (CurrentTime > GunData.MaxTime)
		{
			inactiveTimer = 0;
			CurrentTime = 0;
			CurrentFrame = 0;
		}

		if (!GunData || !Active)
			return;

		if (Active && OneShot && CurrentTime > 0)
		{
			Active = false;
			inactiveTimer = 0;
			CurrentTime = 0;
			CurrentFrame = 0;
			return;
		}

		CurrentTime += SourceObject.LocalTimeScale;
		if (CurrentTime - CurrentFrame >= 1)
		{
			CurrentFrame = (int)CurrentTime;

			foreach(BulletData bulletData in GunData.Bullets)
			{
				if (CurrentFrame == bulletData.Frame)
				{ 
					GameObject bullet = Instantiate(bulletData.Bullet, transform.position,Quaternion.identity);
					if (GunFX)
					{
						GunFX.SetActive(false);
						GunFX.SetActive(true);
					}
					if(ShootFX)
						ShootFX.PlaySFX(SourceObject);
					bullet.GetComponent<TangibleObject>().BaseObjectProperties.BaseAlliance = SourceObject.SmartObjectProperties.Alliance;
					if (SourceObject.GetComponent<PlayerController>())
					{
						bullet.transform.rotation = CameraManager.Instance.MainCamera.transform.rotation * Quaternion.Euler(bulletData.Direction);
						if (SourceObject.Motor.GroundingStatus.IsStableOnGround && CameraManager.Instance.FreeLookCam.m_YAxis.Value > 0.5f)
							bullet.transform.rotation = SourceObject.transform.rotation;
					}
					else
					{
						if (SourceObject.Target)
						{
							bullet.GetComponent<ProjectileObject>().Target = SourceObject.Target;
							bullet.transform.rotation = SourceObject.transform.rotation * Quaternion.Euler(bullet.transform.position - SourceObject.Target.transform.position) * Quaternion.Euler(bulletData.Direction);
						}
						else
							bullet.transform.rotation = Quaternion.Euler(SourceObject.Motor.CharacterForward) * Quaternion.Euler(bulletData.Direction);
					}

				}
			}
		}
	}
}