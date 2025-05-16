using _Scripts.Camera;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Menus
{
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
