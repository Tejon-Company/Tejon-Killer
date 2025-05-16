using _Scripts.Managers.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.SceneTransitions
{
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

            if (!string.IsNullOrEmpty(nextLevelName))
            {
                MusicManager.Instance.PlayMusic(nextLevelMusic);
                SceneManager.LoadScene(nextLevelName);
            }
            else
            {
                Debug.LogWarning("Next scene name is not set in the inspector");
            }
        }
    }
}
