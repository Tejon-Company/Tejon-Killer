using System.Collections;
using _Scripts.Events;
using TMPro;
using UnityEngine;

namespace _Scripts.Hud
{
    /// <summary>
    /// Gestiona la visualización y actualización del contador de enemigos restantes en la interfaz de usuario,
    /// monitorizando cuando los enemigos son derrotados y notificando cuando todos han sido eliminados.
    /// </summary>
    public class EnemiesLeftHUD : MonoBehaviour
    {
        private int _enemiesLeft;

        [SerializeField]
        private TMP_Text enemiesLeftText;

        private void Start() => StartCoroutine(CountEnemies());

        private IEnumerator CountEnemies()
        {
            yield return null;
            _enemiesLeft = GameObject.FindGameObjectsWithTag("Enemies").Length;
            UpdateHUD();

            EventManager.Instance?.enemyDiedEvent.AddListener(OnEnemyDied);
        }

        private void OnDestroy()
        {
            EventManager.Instance?.enemyDiedEvent.RemoveListener(OnEnemyDied);
        }

        private void OnEnemyDied()
        {
            _enemiesLeft--;
            if (_enemiesLeft == 0)
                EventManager.Instance.allEnemiesDefeated.Invoke();

            UpdateHUD();
        }

        private void UpdateHUD() => enemiesLeftText.text = _enemiesLeft.ToString();
    }
}
