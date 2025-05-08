using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        AudioManager.instance.ReproduceMenuMusic();
    }

    public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitApp()
    {
        AudioManager.instance.StopMusic();
        Application.Quit();
        Debug.Log("Application has quit");
    }
}
