using System;
using _Scripts.Camera;
using UnityEngine;

namespace _Scripts.Menus
{
    public class PauseMenuHandler : MonoBehaviour
    {
        public static bool IsPaused { get; private set; }

        [SerializeField]
        private GameObject pauseMenu;

        private void Start()
        {
            if (pauseMenu is null)
                throw new NullReferenceException("Pause menu is not assigned in the inspector.");

            pauseMenu.SetActive(false);
            IsPaused = false;
        }

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
            IsPaused = true;
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }

        public void ResumeGame()
        {
            RotateView.LockCursor();
            IsPaused = false;
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}