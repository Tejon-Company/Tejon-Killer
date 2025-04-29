using UnityEngine;
using UnityEngine.UI;

public class RageUI : MonoBehaviour
{
    [SerializeField] private Image rageBarFill;

    private float maxDuration;
    private float endTime;
    private bool isActive = false;


    private void OnRageActivated(float playerSpeedMultiplier, float weaponFireRateMultiplier, float duration)
    {
        StartRageBar(duration);
    }

    public void StartRageBar(float duration)
    {
        maxDuration = duration;
        endTime = Time.time + duration;
        rageBarFill.fillAmount = 1f;
        rageBarFill.gameObject.SetActive(true);
        isActive = true;
    }

private bool listenerRegistered = false;

    private void Update()
    {
        if (!listenerRegistered && EventManager.current != null)
        {
            EventManager.current.rageBerryEvent.AddListener(OnRageActivated);
            listenerRegistered = true;
        }

        if (!isActive) return;

        float remaining = endTime - Time.time;

        if (remaining <= 0f)
        {
            rageBarFill.fillAmount = 0f;
            isActive = false;
            rageBarFill.gameObject.SetActive(false);
            return;
        }

        rageBarFill.fillAmount = remaining / maxDuration;
    }
}
