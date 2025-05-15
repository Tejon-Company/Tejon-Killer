using UnityEngine;

namespace _Scripts.SceneTransitions
{
    public class UnlockNextLevelLoader : MonoBehaviour
    {
        [SerializeField]
        private GameObject door;

        public void UnlockDoor()
        {
            if (door != null)
            {
                door.SetActive(false);
            }
        }
    }
}
