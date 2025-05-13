using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartsHUD : MonoBehaviour
{
    [SerializeField]
    private Sprite heartSprite;

    [SerializeField]
    private PlayerHealth playerHealth;

    [SerializeField]
    private int heartPixelSize = 5;
    float flickerSpeed = 6f;

    private List<GameObject> hearts = new List<GameObject>();
    private int currentLives;
    private int totalHearts;
    private GameObject heart;
    private Image heartImage;
    private RectTransform rt;
    private Coroutine flickerCoroutine;
    float flickerTime = Mathf.PI / 2f;
    float alpha;
    Color currentHeartColor;

    private void OnEnable()
    {
        if (EventManager.current != null)
        {
            EventManager.current.healthChangedEvent.AddListener(UpdateHearts);
            EventManager.current.damageCooldownEvent.AddListener(SetFlickerState);
            UpdateHearts();
        }
    }

    private void OnDisable()
    {
        if (EventManager.current != null)
        {
            EventManager.current.damageCooldownEvent.RemoveListener(SetFlickerState);
            EventManager.current.healthChangedEvent.RemoveListener(UpdateHearts);
        }
    }

    private void Start()
    {
        totalHearts = playerHealth.MaxHealth;

        for (int i = 0; i < totalHearts; i++)
        {
            heart = CreateHeart();
            hearts.Add(heart);
        }

        UpdateHearts();
    }

    private GameObject CreateHeart()
    {
        heart = new GameObject(
            "Heart",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image)
        );
        heart.transform.SetParent(transform, false);
        heart.SetActive(true);

        heartImage = heart.GetComponent<Image>();
        heartImage.sprite = heartSprite;

        rt = heart.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(heartPixelSize, heartPixelSize);

        return heart;
    }

    private void UpdateHearts()
    {
        currentLives = Mathf.Clamp(playerHealth.CurrentHealth, 0, playerHealth.MaxHealth);

        for (int i = 0; i < hearts.Count; i++)
        {
            hearts[i].SetActive(i < currentLives);
        }
    }

    private void SetFlickerState(bool shouldFlicker)
    {
        if (shouldFlicker)
        {
            if (flickerCoroutine == null)
                flickerCoroutine = StartCoroutine(FlickerHearts());
        }
        else
        {
            if (flickerCoroutine != null)
            {
                StopCoroutine(flickerCoroutine);
                flickerCoroutine = null;
                SetHeartsAlpha(1f);
            }
        }
    }

    private IEnumerator FlickerHearts()
    {
        while (true)
        {
            alpha = Mathf.Lerp(0.3f, 1f, (Mathf.Sin(flickerTime) + 1f) / 2f);
            SetHeartsAlpha(alpha);
            flickerTime += Time.deltaTime * flickerSpeed;
            yield return null;
        }
    }

    private void SetHeartsAlpha(float alpha)
    {
        foreach (GameObject heart in hearts)
        {
            if (heart.TryGetComponent(out Image img))
            {
                currentHeartColor = img.color;
                currentHeartColor.a = alpha;
                img.color = currentHeartColor;
            }
        }
    }
}
