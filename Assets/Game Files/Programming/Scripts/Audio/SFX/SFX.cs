using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SFX")]
public class SFX : ScriptableObject
{
	public AudioClip[] PossibleClips;
	public Vector2 PitchRange = Vector2.one;

	public void PlaySFX(TangibleObject tangibleObject)
	{
		tangibleObject.AudioSource.PlayOneShot(PossibleClips[Random.Range(0,PossibleClips.Length)], Random.Range(PitchRange.x, PitchRange.y));
	}
}