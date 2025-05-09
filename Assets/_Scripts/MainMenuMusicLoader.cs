using _Scripts.Managers.Audio;
using UnityEngine;

namespace _Scripts
{
    public class MainMenuMusicLoader : MonoBehaviour
    {
        private void Start()
        {
            AudioManager.Instance.PlayMusic(AudioManager.Instance.MenuBackgroundMusic);
        }
    }
}