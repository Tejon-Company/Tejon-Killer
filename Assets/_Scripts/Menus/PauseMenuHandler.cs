using System;
using _Scripts.Camera;
using UnityEngine;

namespace _Scripts.Menus
{
    public class PauseMenuHandler : MonoBehaviour
    {
        private static bool isPaused;
        public static bool IsPaused => isPaused;

        [SerializeField]
        private GameObject pauseMenu;

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Escape))
                return;

            if (IsPaused)
                ResumeGame();
            else
                ShowPauseMenu();
        }

        private void ShowPauseMenu()
        {
            RotateView.UnlockCursor();
            isPaused = true;
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }

        public void ResumeGame()
        {
            RotateView.LockCursor();
            isPaused = false;
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}