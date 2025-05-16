using System.Collections;
using _Scripts.Camera;
using _Scripts.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Menus
{
    /// <summary>
    /// Gestiona las funcionalidades del menú de victoria, permitiendo al jugador volver al menú
    /// principal o salir del juego.
    /// </summary>
    public class VictoryMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject victoryMenu;

        private void Start() => victoryMenu.SetActive(false);

        private void OnEnable() => StartCoroutine(InitializeVictoryMenu());

        private IEnumerator InitializeVictoryMenu()
        {
            yield return null;

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
