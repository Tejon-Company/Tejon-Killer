using UnityEngine;

namespace _Scripts.SceneTransitions
{
    public class UnlockNextLevelLoader : MonoBehaviour
    {
        [SerializeField] private GameObject door;
        private GameObject[] enemies;

        private void Update()
        {
            enemies = GameObject.FindGameObjectsWithTag("Enemies");
            if (enemies.Length != 0)
            {
                Debug.Log("Número de enemigos restantes: " + enemies.Length);
                return;
            }
            
            Destroy(door);
            enabled = false;
        }
    }
}