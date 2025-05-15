using System.Collections;
using _Scripts.Managers;
using _Scripts.SceneTransitions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Hud
{
    public class EnemiesLeftHUD : MonoBehaviour
    {
        private int _enemiesLeft;
        public int EnemiesLeft => _enemiesLeft;
        private UnlockNextLevelLoader unlockNextLevelLoader;

        [SerializeField]
        private TMP_Text enemiesLeftText;

        private void Start()
        {
            StartCoroutine(CountEnemies());
        }

        private IEnumerator CountEnemies()
        {
            yield return null;
            _enemiesLeft = GameObject.FindGameObjectsWithTag("Enemies").Length;
            unlockNextLevelLoader = GameObject.FindFirstObjectByType<UnlockNextLevelLoader>();
            Debug.Log("contador global:" + _enemiesLeft);
            UpdateHUD();

            if (EventManager.Current != null)
                EventManager.Current.enemyDiedEvent.AddListener(OnEnemyDied);
        }

        private void OnDestroy()
        {
            if (EventManager.Current != null)
                EventManager.Current.enemyDiedEvent.RemoveListener(OnEnemyDied);
        }

        private void OnEnemyDied()
        {
            _enemiesLeft--;
            if (_enemiesLeft == 0)
            {
                unlockNextLevelLoader.UnlockDoor();
            }
            UpdateHUD();
        }

        private void UpdateHUD()
        {
            enemiesLeftText.text = _enemiesLeft.ToString();
        }
    }
}
