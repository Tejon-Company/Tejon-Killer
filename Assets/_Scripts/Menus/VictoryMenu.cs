using System;
using _Scripts.Camera;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Menus
{
    public class VictoryMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject victoryMenu;

        private void Start()
        {
            victoryMenu.SetActive(false);
        }

        private void Update()
        {
            var enemies = GameObject.FindGameObjectsWithTag("Enemies");
            if (enemies.Length == 0)
                ShowVictoryMenu();
        }

        private void ShowVictoryMenu()
        {
            CameraEffects.UnlockCursor();
            PauseMenu.IsPaused = true;
            Time.timeScale = 0f;
            victoryMenu.SetActive(true);
        }

        public void GoToMainMenu()
        {
            SceneManager.LoadScene("Main Menu");
        }

        public void QuitGame()
        {
            Debug.Log("Quit Game");
            Application.Quit();
        }
    }
}
