using System;
using _Scripts.Camera;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Menus
{
    public class PauseMenu : MonoBehaviour
    {
        public static bool IsPaused { get; private set; }

        [SerializeField]
        private GameObject canvas;

        private void Start()
        {
            if (canvas is null)
                throw new NullReferenceException("Pause menu is not assigned in the inspector.");

            canvas.SetActive(false);
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
            CameraEffects.UnlockCursor();
            IsPaused = true;
            canvas.SetActive(true);
            Time.timeScale = 0f;
        }

        public void ResumeGame()
        {
            CameraEffects.LockCursor();
            IsPaused = false;
            canvas.SetActive(false);
            Time.timeScale = 1f;
        }

        public void ReloadLevel()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void QuitGame()
        {
            Debug.Log("Quit game");
            Application.Quit();
        }
    }
}
