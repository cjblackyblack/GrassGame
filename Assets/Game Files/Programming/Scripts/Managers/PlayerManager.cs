using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager> 
{
	public PlayerController PlayerController;
	public SmartObject PlayerObject;

	public override void Start()
	{
		base.Start();
		Application.targetFrameRate = 60;
	}
}
