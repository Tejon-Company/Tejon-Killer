using System.Collections;
using _Scripts.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Events
{
    
    /// <summary>
    /// Clase que gestiona el evento de fin de juego. Monitorea la salud del jugador y 
    /// carga la escena de "Game Over" cuando la salud llega a cero o menos.
    /// </summary>
    public class GameOverEvent : MonoBehaviour
    {
        private PlayerHealth playerHealth;

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
            playerHealth = playerObject.GetComponent<PlayerHealth>();

            EventManager.Instance.healthChangedEvent.AddListener(OnHealthChanged);
        }

        private void OnDisable()
        {
            EventManager.Instance.healthChangedEvent.RemoveListener(OnHealthChanged);
        }

        private void OnHealthChanged()
        {
            if (playerHealth && playerHealth.CurrentHealth <= 0)
                SceneManager.LoadScene("Game Over Menu");
        }
    }
}
