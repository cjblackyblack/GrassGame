using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    public AudioMixer masterAudio;
    public Slider masterSlider;
    public float volumeLevel;
    void Start()
    {
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 0.025f);
    }

    private void Update()
    {
        //Gotta do this dumb hack because Slider.DynamicFLoat is bugged on Runtime
        if (volumeLevel != masterSlider.value)
        {
            volumeLevel = masterSlider.value;
            masterAudio.SetFloat("MasterVolume", Mathf.Log10(volumeLevel) * 20);
            PlayerPrefs.SetFloat("MasterVolume", volumeLevel);
            PlayerPrefs.Save();
        }
    }
}
