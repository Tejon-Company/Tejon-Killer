using System.Collections;
using _Scripts.Events;
using UnityEngine;

namespace _Scripts.SceneTransitions
{
    /// <summary>
    /// Controla el comportamiento de la puerta que lleva al siguiente nivel.
    /// Se desactiva automáticamente cuando todos los enemigos han sido derrotados.
    /// </summary>
    public class NextLevelDoor : MonoBehaviour
    {
        private void OnEnable() => StartCoroutine(InitializeNextLevelDoor());

        private IEnumerator InitializeNextLevelDoor()
        {
            yield return null;

            EventManager.Instance?.allEnemiesDefeated.AddListener(OnAllEnemiesDefeated);
        }

        private void OnDisable()
        {
            EventManager.Instance?.allEnemiesDefeated.RemoveListener(OnAllEnemiesDefeated);
        }

        private void OnAllEnemiesDefeated() => gameObject.SetActive(false);
    }
}
