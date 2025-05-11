using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Int2Event : UnityEvent<int, int> { }


[Serializable]
public class RageEvent : UnityEvent<float, float, float, float> { }

public class EventManager : MonoBehaviour
{
    public static EventManager current;

    private void Awake()
    {
        if (current is null)
            current = this;
        else
            Destroy(this);
    }

    public Int2Event updateBulletsEvent = new();
    public UnityEvent newGunEvent = new();
    public UnityEvent healthChangedEvent = new();
    public RageEvent rageBerryEvent = new();
}
