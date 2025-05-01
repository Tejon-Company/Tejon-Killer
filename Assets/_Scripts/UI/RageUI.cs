using UnityEngine;
using UnityEngine.UI;

public class RageUI : MonoBehaviour
{
    [SerializeField] private GameObject frameRoot;
    [SerializeField] private Image rageBarFill;

    private float maxDuration;
    private float endTime;
    private bool isActive = false;
    private bool listenerRegistered = false;
    private float remainingTime;

    [Header("PULSE EFFECT")]
    [SerializeField] private float pulseSpeed = 0.2f;
    [SerializeField] private float pulseForce = 0.05f;
    private Vector3 originalScale;

    private void Start()
    {
        originalScale = frameRoot.transform.localScale;
        frameRoot.SetActive(false);
    }

    private void Update()
    {
        RegisterEventListener();
        UpdateRageBar();
    }

    private void RegisterEventListener()
    {
        if (listenerRegistered || EventManager.current == null)
            return;

        EventManager.current.rageBerryEvent.AddListener(OnRageActivated);
        listenerRegistered = true;
    }

    private void UpdateRageBar()
    {
        if (!isActive)
            return;

        remainingTime = endTime - Time.time;

        if (EndRage())
            return;

        rageBarFill.fillAmount = remainingTime / maxDuration;

        float pulse = Mathf.PingPong(Time.time * pulseSpeed, pulseForce);
        frameRoot.transform.localScale = originalScale + new Vector3(pulse, pulse, pulse);
    }

    private bool EndRage()
    {
        if (remainingTime > 0f)
            return false;

        rageBarFill.fillAmount = 0f;
        isActive = false;
        frameRoot.SetActive(false);
        frameRoot.transform.localScale = originalScale;
        return true;
    }

    private void OnRageActivated(float basespeed, float jumpForce, float fireRate, float duration)
    {
        frameRoot.SetActive(true);
        InitializeRageBar(duration);
    }

    public void InitializeRageBar(float duration)
    {
        maxDuration = duration;
        endTime = Time.time + duration;
        rageBarFill.fillAmount = 1f;
        isActive = true;
    }
}
