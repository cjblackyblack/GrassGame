using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
	public GameState CurrentGameState;
	public GameState PreviousGameState;
	public PlayerCharacter SelectedCharacter;

	public int BattleScene = 1;
	public bool Debug;

	public override void Start()
	{
		base.Start();
		Application.targetFrameRate = 60;

		StartCoroutine(WaitStart());

		IEnumerator WaitStart()
		{
			yield return new WaitForEndOfFrame();
			if (SceneManager.GetActiveScene().buildIndex == 0 && !Debug)
				ChangeGameState(GameState.Start);
			else
				ChangeGameState(GameState.Gameplay);
		}
	}

	public void SetCharacter(int index)
	{
		Mathf.Clamp(index, 0, 1); //Set 1 to player count
		SelectedCharacter = (PlayerCharacter)index;
	}

	public void Pause(bool pause)
	{
		Time.timeScale = pause ? 1 : 0;
	}

	public void StartGame()
	{
		LoadBattleSceneAsync(BattleScene);
	}

	public void ResetGame()
	{
		AudioManager.Instance.PlayMusic(false);
		UnLoadSceneAsync(BattleScene);
		SceneManager.SetActiveScene(SceneManager.GetSceneAt(0));
		BattleScene = 1;
		StartCoroutine(LoadMainMenuCoroutine());

		IEnumerator LoadMainMenuCoroutine()
		{
			yield return new WaitForSecondsRealtime(3f);
			ChangeGameState(GameState.Start);
		}
	}

	public void GameWin()
	{
		LoadCreditsScene();
	}

	public void GameOver()
	{
		ChangeGameState(GameState.GameOver);
		PlayerHUDManager.Instance.SpeedrunTime = 0;
	}

	public void ChangeGameState(GameState newGameState)
	{
		PreviousGameState = CurrentGameState;
		CurrentGameState = newGameState;
		switch (CurrentGameState)
		{
			case GameState.Start:
				{
					break;
				}
			case GameState.CharacterSelect:
				{
					break;
				}
			case GameState.Loading:
				{
					break;
				}
			case GameState.Gameplay:
				{
					if (PreviousGameState == GameState.Paused)
						Pause(false);
					AudioManager.Instance.PlayMusic(true, true);
					break;
				}
			case GameState.Paused:
				{
					Pause(true);
					break;
				}
			case GameState.GameOver:
				{
					break;
				}
			case GameState.Credits:
				{
					break;
				}
			case GameState.Controls:
				{
					break;
				}
		}
		UIManager.Instance.ChangeUIPanel(CurrentGameState);
	}

	public void LoadCreditsScene()
	{
		AudioManager.Instance.PlayMusic(false);
		ChangeGameState(GameState.Loading);
		UnLoadSceneAsync(BattleScene);
		SceneManager.SetActiveScene(SceneManager.GetSceneAt(0));
		BattleScene++;
		StartCoroutine(LoadSceneAsyncCoroutine(BattleScene));

		IEnumerator LoadSceneAsyncCoroutine(int index)
		{
			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);

			while (!asyncLoad.isDone)
			{
				yield return null;
			}
			yield return new WaitForSecondsRealtime(3f);
			SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(index));
		}
		ChangeGameState(GameState.Credits);
	}

	void LoadBattleSceneAsync(int index)
	{
		AudioManager.Instance.PlayMusic(false);
		ChangeGameState(GameState.Loading);
		StartCoroutine(LoadSceneAsyncCoroutine(index));

		IEnumerator LoadSceneAsyncCoroutine(int index)
		{
			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);

			while (!asyncLoad.isDone)
			{
				yield return null;
			}
			yield return new WaitForSecondsRealtime(2f);
			SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(index));
			yield return new WaitForSecondsRealtime(1f);
			ChangeGameState(GameState.Gameplay);

		}
	}

	void UnLoadSceneAsync(int index)
	{
		StartCoroutine(UnLoadSceneAsyncCoroutine(index));

		IEnumerator UnLoadSceneAsyncCoroutine(int index)
		{
			AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(index);

			while (!asyncLoad.isDone)
			{
				yield return null;
			}
		}
	}

	public void GlobalHitStop(float length)
	{
		StartCoroutine(GlobalHitStopCoroutine(length));
		IEnumerator GlobalHitStopCoroutine(float length)
		{
			Time.timeScale = 0;
			yield return new WaitForSecondsRealtime(length);
			Time.timeScale = 1;
		}
	}
}