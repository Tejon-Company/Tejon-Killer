using _Scripts.Managers.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Menus
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

        public void StartFirstLevel()
        {
            SceneManager.LoadScene("Level 1");
        }

        public void QuitGame()
        {
            Debug.Log("Quit game");
            Application.Quit();
        }
    }
}
