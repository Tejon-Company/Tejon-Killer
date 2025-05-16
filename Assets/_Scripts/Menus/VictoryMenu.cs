using System;
using _Scripts.Camera;
using _Scripts.Managers;
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

        private void OnEnable()
        {
            EventManager.Instance?.allEnemiesDefeated.AddListener(OnAllEnemiesDefeated);
        }

        private void OnDisable()
        {
            EventManager.Instance?.allEnemiesDefeated.RemoveListener(OnAllEnemiesDefeated);
        }

        private void OnAllEnemiesDefeated()
        {
            CameraEffects.UnlockCursor();
            PauseMenu.IsPaused = true;
            Time.timeScale = 0f;
            victoryMenu.SetActive(true);
        }

        public void GoToMainMenu() => SceneManager.LoadScene("Main Menu");

        public void QuitGame()
        {
            Debug.Log("Quit Game");
            Application.Quit();
        }
    }
}
