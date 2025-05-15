using _Scripts.Managers;
using UnityEngine;

namespace _Scripts.SceneTransitions
{
    public class UnlockNextLevelLoader : MonoBehaviour
    {
        private void OnEnable()
        {
            EventManager.Instance?.allEnemiesDefeated.AddListener(OnAllEnemiesDefeated);
        }

        private void OnDisable()
        {
            EventManager.Instance?.allEnemiesDefeated.RemoveListener(OnAllEnemiesDefeated);
        }

        private void OnAllEnemiesDefeated()
        {
            Debug.Log("on all enemies defeated called in unlock next level loader");
            gameObject.SetActive(false);
        }
    }
}
