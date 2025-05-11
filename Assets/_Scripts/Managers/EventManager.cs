using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

[Serializable]
public class Int2Event : UnityEvent<int, int> { }


[Serializable]
public class RageEvent : UnityEvent<float, float, float, float> { }

public class EventManager : MonoBehaviour
{
    public static EventManager current;

    void Awake()
    {
        if (current == null)
        {
            current = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public Int2Event updateBulletsEvent = new Int2Event();
    public UnityEvent NewGunEvent = new UnityEvent();
    public UnityEvent healthChangedEvent = new UnityEvent();
    public RageEvent rageBerryEvent = new RageEvent();

}
