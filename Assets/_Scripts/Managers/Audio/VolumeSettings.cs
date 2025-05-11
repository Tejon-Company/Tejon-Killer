using UnityEngine;
using UnityEngine.Audio;

namespace _Scripts.Managers.Audio
{
    public class VolumeSettings : MonoBehaviour
    {
        public static VolumeSettings Instance { get; private set; }
        
        [SerializeField]
        private AudioMixer audioMixer;

        public float MusicVolume { get; private set; } = 1f;

        public float SfxVolume { get; private set; } = 1f;

        private void Awake()
        {
            if (Instance is null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadSavedVolumes();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void LoadSavedVolumes()
        {
            Debug.Log("load seved volumes");
            MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
            SfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            
            ApplyMusicVolume(MusicVolume);
            ApplySfxVolume(SfxVolume);
        }

        public void SetMusicVolume(float volume)
        {
            MusicVolume = volume;
            PlayerPrefs.SetFloat("MusicVolume", volume);
            ApplyMusicVolume(volume);
        }

        public void SetSfxVolume(float volume)
        {
            SfxVolume = volume;
            PlayerPrefs.SetFloat("SFXVolume", volume);
            ApplySfxVolume(volume);
        }
        
        private void ApplyMusicVolume(float volume)
        {
            var volumeInDecibels = Mathf.Log10(volume) * 20;
            audioMixer.SetFloat("Music", volumeInDecibels);
        }
        
        private void ApplySfxVolume(float volume)
        {
            var volumeInDecibels = Mathf.Log10(volume) * 20;
            audioMixer.SetFloat("SFX", volumeInDecibels);
        }
    }
}
