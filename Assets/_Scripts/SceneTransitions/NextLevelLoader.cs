using _Scripts.Audio.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.SceneTransitions
{
    /// <summary>
    /// Gestiona la transición al siguiente nivel cuando el jugador interactúa con el trigger,
    /// permitiendo cambiar la escena y la música de fondo.
    /// </summary>
    public class NextLevelLoader : MonoBehaviour
    {
        [SerializeField]
        private string nextLevelName;

        [SerializeField]
        private AudioClip nextLevelMusic;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            if (string.IsNullOrEmpty(nextLevelName))
                return;

            MusicManager.Instance.PlayMusic(nextLevelMusic);
            SceneManager.LoadScene(nextLevelName);
        }
    }
}
