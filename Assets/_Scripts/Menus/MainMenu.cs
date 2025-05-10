using _Scripts.Managers.Audio;
using UnityEngine;

namespace _Scripts
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject mainMenuUI;
        [SerializeField]
        private GameObject settingsUI;
        
        private void Start()
        {
            MusicManager.Instance.PlayMusic(MusicManager.Instance.MenuBackgroundMusic);
            ShowMenuUI();
        }
        
        public void ShowMenuUI()
        {
            mainMenuUI.SetActive(true);
            settingsUI.SetActive(false);
        }
        
        public void ShowSettingsUI()
        {
            mainMenuUI.SetActive(false);
            settingsUI.SetActive(true);
        }
    }
}