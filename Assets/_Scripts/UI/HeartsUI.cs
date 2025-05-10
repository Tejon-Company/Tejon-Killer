using System.Collections.Generic;
using _Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

public class HeartsUI : MonoBehaviour
{
    [SerializeField]
    private Sprite heartSprite;

    [SerializeField]
    private PlayerHealth playerHealth;

    [SerializeField]
    private int heartPixelSize = 5;

    private List<GameObject> hearts = new List<GameObject>();
    private int currentLives;
    private int totalHearts;
    private GameObject heart;
    private Image heartImage;
    private RectTransform rt;

    private void OnEnable()
    {
        if (EventManager.current != null)
        {
            EventManager.current.healthChangedEvent.AddListener(UpdateHearts);
            UpdateHearts();
        }
    }

    private void OnDisable()
    {
        if (EventManager.current != null)
        {
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
}
