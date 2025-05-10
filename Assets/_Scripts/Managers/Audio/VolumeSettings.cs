using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace _Scripts.Managers.Audio
{
    public class VolumeSettings : MonoBehaviour
    {
        [SerializeField]
        private AudioMixer audioMixer;

        [SerializeField]
        private Slider musicSlider;

        [SerializeField]
        private Slider sfxSlider;

        public void SetMusicVolume()
        {
            Debug.Log("Setting music volume");
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
}
