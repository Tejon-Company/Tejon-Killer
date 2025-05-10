using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace _Scripts.Managers
{
    [Serializable]
    public class Int2Event : UnityEvent<int, int>
    {
    }

    public class EventManager : MonoBehaviour
    {
        public static EventManager current;

        public Int2Event updateBulletsEvent = new();
        [FormerlySerializedAs("NewGunEvent")] public UnityEvent newGunEvent = new();
        public UnityEvent healthChangedEvent = new();

        private void Awake()
        {
            if (current is null)
                current = this;
            else
                Destroy(this);
        }
    }
}