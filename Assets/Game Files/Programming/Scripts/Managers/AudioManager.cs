using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager> 
{
	public AudioSource MusicSource;
	public AudioSource SFXSource;
	public AudioClip MainTrack;
	public AudioClip FightTrack;
	public AudioClip WinTrack;
	public AudioClip LoseTrack;
	public AudioClip[] CountdownClips;
	public AudioClip[] Pogs;
	public AudioClip Static;
	public AudioClip[] Beeps;
	public AudioClip[] Bangs;

	public void PlaySFX(AudioClip clip = null, bool loop = false)
	{
		if (clip != null)
			SFXSource.PlayOneShot(clip);
		else
			SFXSource.Stop();

		SFXSource.loop = loop;
	}

	public void PlayStaticSFX(bool play)
	{
		if (play)
			PlaySFX(Static, true);
		else
			PlaySFX();
	}

	public void PlayMusic(bool play, bool loop = false)
	{
		if(play)
			MusicSource.Play();
		else
			MusicSource.Stop();
		MusicSource.loop = loop;
	}

	public void PlayFightTrack()
	{
		PlayMusic(false);
		MusicSource.clip = FightTrack;
		PlayMusic(true, true);
	}

	public void PlayWinTrack()
	{
		PlayMusic(false);
		MusicSource.clip = WinTrack;
		PlayMusic(true);
	}

	public void PlayLoseTrack()
	{
		PlayMusic(false);
		MusicSource.clip = LoseTrack;
		PlayMusic(true);
	}
}
