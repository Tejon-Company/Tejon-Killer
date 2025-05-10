using System;
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
            //pauseMenu.SetActive(false);
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Escape))
            {
                return;
            }

            Debug.Log("scape pressed");

            if (IsPaused)
            {
                ResumeGame();
            }
            else
            {
                ShowPauseMenu();
            }
        }

        private void ShowPauseMenu()
        {
            IsPaused = true;
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }

        public void ResumeGame()
        {
            IsPaused = false;
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}