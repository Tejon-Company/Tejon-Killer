using System.Collections;
using _Scripts.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Events
{
    public class GameOverEvent : MonoBehaviour
    {
        private PlayerHealth _playerHealth;

        private void Awake() => StartCoroutine(SetupPlayerHealth());

        private IEnumerator SetupPlayerHealth()
        {
            yield return null;

            var playerObject = GameObject.FindGameObjectWithTag("Player");
            if (!playerObject)
            {
                Debug.LogError("Player object not found in the scene.");
                yield break;
            }
            _playerHealth = playerObject.GetComponent<PlayerHealth>();

            EventManager.Instance.healthChangedEvent.AddListener(OnHealthChanged);
        }

        private void OnDisable()
        {
            EventManager.Instance.healthChangedEvent.RemoveListener(OnHealthChanged);
        }

        private void OnHealthChanged()
        {
            if (_playerHealth && _playerHealth.CurrentHealth <= 0)
                SceneManager.LoadScene("Game Over Menu");
        }
    }
}
