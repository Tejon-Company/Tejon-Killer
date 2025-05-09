using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    
    public void SetMusicVolume()
    {
        var volume = musicSlider.value;
        var volumeInDecibels = Mathf.Log10(volume) * 20;
        audioMixer.SetFloat("Music", volumeInDecibels);
    }

    public void SetSfxVolume()
    {
        var volume = sfxSlider.value;
        var volumeInDecibels = Mathf.Log10(volume) * 20;
        audioMixer.SetFloat("SFX", volumeInDecibels);
    }
}
