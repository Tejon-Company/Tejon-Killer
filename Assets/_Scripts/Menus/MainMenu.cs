using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Menus
{
    
    /// <summary>
    /// Gestiona las funcionalidades del menú principal, controlando la navegación 
    /// entre interfaces y las acciones básicas como iniciar el juego o salir.
    /// </summary>
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject mainMenuUI;

        [SerializeField]
        private GameObject settingsUI;

        private void Start() => ShowMenuUI();

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

        public void StartFirstLevel(string levelName) => SceneManager.LoadScene(levelName);

        public void QuitGame()
        {
            Debug.Log("Quit game");
            Application.Quit();
        }
    }
}
