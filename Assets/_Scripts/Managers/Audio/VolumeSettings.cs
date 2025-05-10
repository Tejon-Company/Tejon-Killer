using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace _Scripts.Managers.Audio
{
    public class VolumeSettings : MonoBehaviour
    {
        public static VolumeSettings Instance { get; private set; }
        
        [SerializeField]
        private AudioMixer audioMixer;

        [SerializeField]
        private Slider musicSlider;

        [SerializeField]
        private Slider sfxSlider;

        private void Awake()
        {
            if (Instance is null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        private void Start()
        {
            if (PlayerPrefs.HasKey("Volume"))
            {
                var volume = PlayerPrefs.GetFloat("Volume");
                musicSlider.value = volume;
                SetMusicVolume();
            }
            else
            {
                PlayerPrefs.SetFloat("Volume", 1);
            }

            if (PlayerPrefs.HasKey("SFXVolume"))
            {
                var sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
                sfxSlider.value = sfxVolume;
                SetSfxVolume();
            }
            else
            {
                PlayerPrefs.SetFloat("SFXVolume", 1);
            }
        }

        public void SetMusicVolume()
        {
            var volume = musicSlider.value;
            PlayerPrefs.SetFloat("Volume", volume);
            
            var volumeInDecibels = Mathf.Log10(volume) * 20;
            audioMixer.SetFloat("Music", volumeInDecibels);
        }

        public void SetSfxVolume()
        {
            var volume = sfxSlider.value;
            PlayerPrefs.SetFloat("SFXVolume", volume);
            
            var volumeInDecibels = Mathf.Log10(volume) * 20;
            audioMixer.SetFloat("SFX", volumeInDecibels);
        }
    }
}
