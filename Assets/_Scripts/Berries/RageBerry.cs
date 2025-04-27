using UnityEngine;

public class RageBerry : BerryEffect
{
    [SerializeField] private float rageDuration = 20f;
    [SerializeField] private float rageMultiplier = 0.5f;

    protected override void ApplyEffect(Collider player)
    {
        EventManager.current.rageBerryEvent.Invoke(rageMultiplier, rageDuration);
    }
}
