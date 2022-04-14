using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class UIManager : Singleton<UIManager>
{

	public GameObject MainMenuUI;
	public GameObject CharacterSelectUI;
	public GameObject LoadingUI;
	public GameObject StaticUI;
	public GameObject PlayerUI;
	public GameObject AdditionalUI;
	public GameObject PausedUI;
	public GameObject StartCountdownUI;
	public GameObject GameOverUI;
	public GameObject CreditsUI;
	public GameObject ControlsUI;

	public TextMeshProUGUI StartCountdownText;
	public TextMeshProUGUI GameOverCountdownText;

	public GameObject[] CreditsLines;

	public void ToggleMainMenuUI(bool enabled)
	{
		MainMenuUI.gameObject.SetActive(enabled);
			if(enabled)ShowTitle();
	}

	public void ToggleCharacterSelectUI(bool enabled)
	{
		CharacterSelectUI.gameObject.SetActive(enabled);
	}

	public void ToggleLoadingUI(bool enabled)
	{
		LoadingUI.gameObject.SetActive(enabled);
	}

	public void TogglePlayerUI(bool enabled)
	{
		PlayerUI.gameObject.SetActive(enabled);
	}

	public void ToggleAdditionalUI(bool enabled)
	{
		AdditionalUI.gameObject.SetActive(enabled);
	}

	public void TogglePauseUI(bool enabled)
	{
		PausedUI.gameObject.SetActive(enabled);
	}

	public void ToggleStartCountdownUI(bool enabled)
	{
		StartCountdownUI.SetActive(enabled);
	}

	public void ToggleGameOverUI(bool enabled)
	{
		GameOverUI.SetActive(enabled);
	}

	public void ToggleCreditsUI(bool enabled)
	{
		CreditsUI.SetActive(enabled);
		if (enabled)
			ExitCredits();
	}

	public void ToggleControlsUI(bool enabled)
	{
		ControlsUI.SetActive(enabled);
	}


	public void ChangeUIPanel(GameState gameState)
	{
		StopAllCoroutines();
		ToggleMainMenuUI(false);
		ToggleCharacterSelectUI(false);
		ToggleLoadingUI(false);
		TogglePlayerUI(false);
		ToggleAdditionalUI(false);
		TogglePauseUI(false);
		ToggleStartCountdownUI(false);
		ToggleGameOverUI(false);
		ToggleCreditsUI(false);
		ToggleControlsUI(false);
		FlashStatic();
	
		switch (gameState)
		{
			case GameState.Start:
				{
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
					ToggleMainMenuUI(true);
					break;
				}
			case GameState.CharacterSelect:
				{
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
					ToggleCharacterSelectUI(true);
					break;
				}
			case GameState.Loading:
				{
					ToggleLoadingUI(true);
					break;
				}
			case GameState.Gameplay:
				{
					Cursor.lockState = CursorLockMode.Locked;
					Cursor.visible = false;
					TogglePlayerUI(true);
					ToggleAdditionalUI(true);
	
					break;
				}
			case GameState.Paused:
				{
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
					TogglePauseUI(true);
					break;
				}
			case GameState.GameOver:
				{
					ToggleGameOverUI(true);
					GameOverCountdown();
					break;
				}
			case GameState.Credits:
				{
					ToggleCreditsUI(true);
					break;
				}
			case GameState.Controls:
				{
					ToggleControlsUI(true);
					break;
				}
		}
	}

	public void OnStartButton()
	{
		GameManager.Instance.StartGame();
	}

	public void OnMainStart()
	{
		GameManager.Instance.ChangeGameState(GameState.CharacterSelect);
	}

	public void OnCharacterSelectButton()
	{
		int index = (int)GameManager.Instance.SelectedCharacter;
		index++;
		if (index > 1)
			index = 0;
		GameManager.Instance.SelectedCharacter = (PlayerCharacter)index;
	}

	public void OnOptionsButton()
	{
		GameManager.Instance.ChangeGameState(GameState.Controls);
	}

	public void OnQuitButton()
	{
		Application.Quit();
	}

	public void OnCharacterSelectBack()
	{
		GameManager.Instance.ChangeGameState(GameState.Start);
	}

	public void RoundStartCountdown()
	{
		StartCoroutine(Countdown());

		IEnumerator Countdown()
		{

			yield return new WaitForSecondsRealtime(1f);
			ToggleStartCountdownUI(true);
			StartCountdownText.gameObject.SetActive(true);
			StartCountdownText.text = "READY";
			AudioManager.Instance.SFXSource.PlayOneShot(AudioManager.Instance.Pogs[0]);
			yield return new WaitForSecondsRealtime(2f);
			for (int i = 3; i >= 0; i--)
			{
				StartCountdownText.text = $"{i}";
				AudioManager.Instance.SFXSource.PlayOneShot(AudioManager.Instance.CountdownClips[i]);
				yield return new WaitForSecondsRealtime(1f);
			}

			#region Hard Coded Cowntdown of Shame
			//StartCountdownText.text = "3";
			//AudioManager.Instance.SFXSource.PlayOneShot(AudioManager.Instance.CountdownClips[3]);
			//yield return new WaitForSecondsRealtime(1f);
			//StartCountdownText.text = "2";
			//AudioManager.Instance.SFXSource.PlayOneShot(AudioManager.Instance.CountdownClips[2]);
			//yield return new WaitForSecondsRealtime(1f);
			//StartCountdownText.text = "1";
			//AudioManager.Instance.SFXSource.PlayOneShot(AudioManager.Instance.CountdownClips[1]);
			//yield return new WaitForSecondsRealtime(1f);
			#endregion

			StartCountdownText.text = "GO";
			AudioManager.Instance.SFXSource.PlayOneShot(AudioManager.Instance.CountdownClips[0]);
			yield return new WaitForSecondsRealtime(1f);

			StartCountdownText.gameObject.SetActive(false);
			StartCountdownUI.SetActive(false);
			ToggleStartCountdownUI(false);
		}
	}

	public void GameOverCountdown()
	{
		StartCoroutine(Countdown());
		IEnumerator Countdown()
		{
			GameOverCountdownText.gameObject.SetActive(true);
			for (int i = 10; i >= 0; i--)
			{
				GameOverCountdownText.text = $"{i}";
				AudioManager.Instance.SFXSource.PlayOneShot(AudioManager.Instance.CountdownClips[i]);
				yield return new WaitForSecondsRealtime(1f);
			}
			GameOverCountdownText.gameObject.SetActive(false);

			#region Dumb Hard Coded Countdown that is shameful
			//GameOverCountdownText.text = "10";
			//AudioManager.Instance.SFXSource.PlayOneShot(AudioManager.Instance.CountdownClips[10]);
			//yield return new WaitForSecondsRealtime(1f);
			//GameOverCountdownText.text = "9";
			//AudioManager.Instance.SFXSource.PlayOneShot(AudioManager.Instance.CountdownClips[9]);
			//yield return new WaitForSecondsRealtime(1f);
			//GameOverCountdownText.text = "8";
			//AudioManager.Instance.SFXSource.PlayOneShot(AudioManager.Instance.CountdownClips[8]);
			//yield return new WaitForSecondsRealtime(1f);
			//GameOverCountdownText.text = "7";
			//AudioManager.Instance.SFXSource.PlayOneShot(AudioManager.Instance.CountdownClips[7]);
			//yield return new WaitForSecondsRealtime(1f);
			//GameOverCountdownText.text = "6";
			//AudioManager.Instance.SFXSource.PlayOneShot(AudioManager.Instance.CountdownClips[6]);
			//yield return new WaitForSecondsRealtime(1f);
			//GameOverCountdownText.text = "5";
			//AudioManager.Instance.SFXSource.PlayOneShot(AudioManager.Instance.CountdownClips[5]);
			//yield return new WaitForSecondsRealtime(1f);
			//GameOverCountdownText.text = "4";
			//AudioManager.Instance.SFXSource.PlayOneShot(AudioManager.Instance.CountdownClips[4]);
			//yield return new WaitForSecondsRealtime(1f);
			//GameOverCountdownText.text = "3";
			//AudioManager.Instance.SFXSource.PlayOneShot(AudioManager.Instance.CountdownClips[3]);
			//yield return new WaitForSecondsRealtime(1f);
			//GameOverCountdownText.text = "2";
			//AudioManager.Instance.SFXSource.PlayOneShot(AudioManager.Instance.CountdownClips[2]);
			//yield return new WaitForSecondsRealtime(1f);
			//GameOverCountdownText.text = "1";
			//AudioManager.Instance.SFXSource.PlayOneShot(AudioManager.Instance.CountdownClips[1]);
			//yield return new WaitForSecondsRealtime(1f);
			//GameOverCountdownText.text = "0";
			//AudioManager.Instance.SFXSource.PlayOneShot(AudioManager.Instance.CountdownClips[0]);
			//yield return new WaitForSecondsRealtime(1f);
			#endregion
		}
	}

	public void FlashStatic()
	{
		StartCoroutine(FlashStaticCoroutine());
		IEnumerator FlashStaticCoroutine()
		{
			StaticUI.gameObject.SetActive(true);
			AudioManager.Instance.PlayStaticSFX(true);
			yield return new WaitForSecondsRealtime(0.15f);
			AudioManager.Instance.PlayStaticSFX(false);
			StaticUI.gameObject.SetActive(false);
		}
	}

	public void ShowTitle()
	{
		StartCoroutine(ShowTitleCoroutine());

		IEnumerator ShowTitleCoroutine()
		{
			yield return new WaitForSeconds(1f);
		}
	}



	public void ExitCredits()
	{
		StartCoroutine(ShowTitleCoroutine());

		IEnumerator ShowTitleCoroutine()
		{
			Debug.Log("I see you seeing in that you see me seeing you");
			yield return new WaitForSecondsRealtime(6);

			for(int i = 0; i < CreditsLines.Length; i++)
			{
				CreditsLines[i].gameObject.SetActive(true);
				yield return new WaitForSecondsRealtime(2);
			}

			#region Hard Coded Credits of Shame
			//CreditsLines[0].gameObject.SetActive(true);
			//yield return new WaitForSecondsRealtime(2);
			//
			//CreditsLines[1].gameObject.SetActive(true);
			//yield return new WaitForSecondsRealtime(2);
			//
			//CreditsLines[2].gameObject.SetActive(true);
			//yield return new WaitForSecondsRealtime(2);
			//
			//CreditsLines[3].gameObject.SetActive(true);
			//yield return new WaitForSecondsRealtime(2);
			//
			//CreditsLines[4].gameObject.SetActive(true);
			//yield return new WaitForSecondsRealtime(2);
			//
			//CreditsLines[5].gameObject.SetActive(true);
			//yield return new WaitForSecondsRealtime(2);
			//
			//CreditsLines[6].gameObject.SetActive(true);
			//yield return new WaitForSecondsRealtime(2);
			//
			//
			//CreditsLines[7].gameObject.SetActive(true);
			//yield return new WaitForSecondsRealtime(2);
			#endregion

			yield return new WaitForSecondsRealtime(90);
			GameManager.Instance.ChangeGameState(GameState.Start);
		}
	}
}