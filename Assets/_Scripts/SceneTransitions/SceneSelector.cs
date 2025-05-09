using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSelector : MonoBehaviour
{
    void Start()
    {
        AudioManager.instance.ReproduceMenuMusic();
    }

    public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
