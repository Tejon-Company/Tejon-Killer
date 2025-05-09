
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelLoader : MonoBehaviour
{
    [Tooltip("Next scene name")]
    public string nextLevelName;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (!string.IsNullOrEmpty(nextLevelName))
        {
            SceneManager.LoadScene(nextLevelName);
        }
        else
        {
            Debug.LogWarning("Next scene name is not set in the inspector");
        }
    }
}
