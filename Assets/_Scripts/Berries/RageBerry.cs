using UnityEngine;

public class RageBerry : BerryEffect
{
    [SerializeField] private float playerSpeedMultiplier = 2f;  
    [SerializeField] private float weaponFireRateMultiplier = 0.5f; 
    [SerializeField] private float rageDuration = 7f;

protected override void ApplyEffect(Collider player)
    {
        EventManager.current.rageBerryEvent.Invoke(playerSpeedMultiplier, weaponFireRateMultiplier, rageDuration);
    }
}
