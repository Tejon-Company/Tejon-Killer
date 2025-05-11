using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitHandler : MonoBehaviour
{
    public void QuitApp()
    {
        Application.Quit();
        Debug.Log("Application has quit");
    }
}
