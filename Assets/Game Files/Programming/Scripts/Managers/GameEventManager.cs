using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventManager : Singleton<GameEventManager>
{
	public Action playRoundStartTimeline;

	public void PlayRoundStartTimeline() => playRoundStartTimeline?.Invoke();
}
