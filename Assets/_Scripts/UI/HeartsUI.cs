using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HeartsUI : MonoBehaviour
{
    public Sprite heartSprite;
    public PlayerHealth playerHealth;
    private List<GameObject> hearts = new List<GameObject>();

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

    void Start()
    {
        int totalHearts = Mathf.Min(5, Mathf.CeilToInt(playerHealth.maxHealth)); 

        for (int i = 0; i < totalHearts; i++)
        {
            GameObject heartObj = new GameObject("Heart", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            heartObj.transform.SetParent(transform);
            heartObj.SetActive(true);

            Image heartImage = heartObj.GetComponent<Image>();
            heartImage.sprite = heartSprite;
           RectTransform rt = heartObj.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(86, 86); 

            hearts.Add(heartObj);
        }

        UpdateHearts();
    }

    void UpdateHearts()
    {
        int currentLives = Mathf.Clamp(playerHealth.CurrentHealth, 0, 5);

        for (int i = 0; i < hearts.Count; i++)
        {
            bool shouldBeActive = i < currentLives;
            hearts[i].SetActive(shouldBeActive);
        }
    }
}