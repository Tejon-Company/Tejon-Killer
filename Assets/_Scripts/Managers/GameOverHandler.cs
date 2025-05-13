using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using _Scripts.Player;

namespace _Scripts.Managers
{
    public class GameOverHandler : MonoBehaviour
    {
        private PlayerHealth _playerHealth;

        private void OnEnable()
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            
            if (playerObj)
                _playerHealth = playerObj.GetComponent<PlayerHealth>();

            StartCoroutine(SubscribeToEventManager());
        }
        
        private IEnumerator SubscribeToEventManager()
        {
            yield return null;
            EventManager.Current.healthChangedEvent.AddListener(OnHealthChanged);
        }

        private void OnDisable()
        {
            EventManager.Current.healthChangedEvent.RemoveListener(OnHealthChanged);
        }

        private void OnHealthChanged()
        {
            if (_playerHealth && _playerHealth.CurrentHealth <= 0)
                SceneManager.LoadScene("Game Over Menu");
        }
    }
}