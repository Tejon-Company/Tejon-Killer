using System;
using UnityEngine;

namespace _Scripts
{
    public class PauseMenuHandler : MonoBehaviour
    {
        private bool isPaused;
        
        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Escape))
            {
                return;
            }
            
            if (!isPaused)
            {
                isPaused = true;
                Show();
            }
            else
            {
                isPaused = false;
                Resume();
            }
        }

        private void Show()
        {
            gameObject.SetActive(true);
            Time.timeScale = 0f;
        }
        
        public void Resume()
        {
            gameObject.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}