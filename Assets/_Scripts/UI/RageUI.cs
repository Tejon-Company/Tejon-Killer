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

    private void Start()
    {
        frameRoot.SetActive(false); 
    }

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
            frameRoot.SetActive(false); // Oculta visualmente todo
            return;
        }

        rageBarFill.fillAmount = remaining / maxDuration;
    }

    private void OnRageActivated(float speed, float fireRate, float duration)
    {
        frameRoot.SetActive(true); 
        StartRageBar(duration);
    }

    public void StartRageBar(float duration)
    {
        maxDuration = duration;
        endTime = Time.time + duration;
        rageBarFill.fillAmount = 1f;
        isActive = true;
    }
}
