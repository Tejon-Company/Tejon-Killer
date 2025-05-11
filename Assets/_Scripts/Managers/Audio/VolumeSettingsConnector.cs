using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Managers.Audio
{
    public class VolumeSettingsConnector : MonoBehaviour
    {
        [SerializeField]
        private Slider musicSlider;

        [SerializeField]
        private Slider sfxSlider;

        private void Start()
        {
            if (VolumeSettings.Instance is null)
                return;

            musicSlider.value = VolumeSettings.Instance.MusicVolume;
            musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

            sfxSlider.value = VolumeSettings.Instance.SfxVolume;
            sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
        }

        private static void OnMusicVolumeChanged(float value)
        {
            VolumeSettings.Instance.SetMusicVolume(value);
        }

        private static void OnSfxVolumeChanged(float value)
        {
            VolumeSettings.Instance.SetSfxVolume(value);
        }

        private void OnDestroy()
        {
            musicSlider?.onValueChanged.RemoveListener(OnMusicVolumeChanged);
            sfxSlider?.onValueChanged.RemoveListener(OnSfxVolumeChanged);
        }
    }
}
