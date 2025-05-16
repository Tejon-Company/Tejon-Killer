using _Scripts.Camera;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Menus
{
    /// <summary>
    /// Clase que controla las funcionalidades del menú de game over,
    /// incluyendo navegación al menú principal y salir del juego.
    /// </summary>
    public class GameOverMenu : MonoBehaviour
    {
        private void Start() => CameraEffects.UnlockCursor();

        public void GoToMainMenu() => SceneManager.LoadScene("Main Menu");

        public void QuitGame()
        {
            Debug.Log("Quit Game");
            Application.Quit();
        }
    }
}
