using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace _Scripts.Audio
{
    /// <summary>
    /// Gestor singleton para controlar y persistir la configuración de volumen de música y efectos de sonido.
    /// </summary>
    public class VolumeSettings : MonoBehaviour
    {
        public static VolumeSettings Instance { get; private set; }

        [SerializeField]
        private AudioMixer audioMixer;

        public float MusicVolume { get; private set; } = 1f;

        public float SfxVolume { get; private set; } = 1f;

        private void Awake()
        {
            if (Instance is not null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSavedVolumes();
            StartCoroutine(ApplyVolumes());
        }

        private IEnumerator ApplyVolumes()
        {
            yield return null;

            ApplyMusicVolume();
            ApplySfxVolume();
        }

        private void LoadSavedVolumes()
        {
            MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
            SfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

            ApplyMusicVolume();
            ApplySfxVolume();
        }

        public void SetMusicVolume(float volume)
        {
            MusicVolume = volume;
            PlayerPrefs.SetFloat("MusicVolume", volume);
            ApplyMusicVolume();
        }

        public void SetSfxVolume(float volume)
        {
            SfxVolume = volume;
            PlayerPrefs.SetFloat("SFXVolume", volume);
            ApplySfxVolume();
        }

        private void ApplyMusicVolume()
        {
            var volumeInDecibels = Mathf.Log10(MusicVolume) * 20;
            audioMixer.SetFloat("Music", volumeInDecibels);
        }

        private void ApplySfxVolume()
        {
            var volumeInDecibels = Mathf.Log10(SfxVolume) * 20;
            audioMixer.SetFloat("SFX", volumeInDecibels);
        }
    }
}
