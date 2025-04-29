using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HeartsUI : MonoBehaviour
{
    public Image heartTemplate;
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
            Image heart = Instantiate(heartTemplate, transform);
            heart.gameObject.SetActive(true);
            hearts.Add(heart.gameObject);
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
