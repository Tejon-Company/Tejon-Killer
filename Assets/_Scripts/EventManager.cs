using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;


[Serializable] public class Int2Event: UnityEvent<int, int> {}

public class EventManager : MonoBehaviour
{
    //--------Singleton
    public static EventManager current;
    void Awake()
    {
        if (current == null) { current = this; } else if (current != null) { Destroy(this); }
    }
    //---------
    public Int2Event updateBulletsEvent =new Int2Event();
    public UnityEvent NewGunEvent= new UnityEvent();
    public UnityEvent healthChangedEvent = new UnityEvent();

}