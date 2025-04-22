using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LifeUI : MonoBehaviour
{
    public Image heartTemplate;
    public PlayerHealth playerHealth;
    private List<GameObject> hearts = new List<GameObject>();

    private void OnEnable()
    {
        if (EventManager.current != null)
        {
            Debug.Log("SUSCRIBIENDO A HEALTH EVENT desde LifeUI");
            EventManager.current.healthChangedEvent.AddListener(UpdateHearts);
            UpdateHearts();
        }
        else
        {
            Debug.LogWarning("EventManager no encontrado al suscribir desde LifeUI.");
        }
    }

    private void OnDisable()
    {
        Debug.Log("DESUSCRIBIENDO DE HEALTH EVENT desde LifeUI");
        if (EventManager.current != null)
        {
            EventManager.current.healthChangedEvent.RemoveListener(UpdateHearts);
        }
    }


    void Start()
    {
        int totalHearts = Mathf.CeilToInt(playerHealth.maxHealth);

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
        Debug.Log("UpdateHearts llamado: vidas actuales = " + playerHealth.currentHealth + " / max: " + playerHealth.maxHealth);

        int currentLives = Mathf.CeilToInt(playerHealth.currentHealth);

        for (int i = 0; i < hearts.Count; i++)
        {
            bool shouldBeActive = (i < currentLives);
            hearts[i].SetActive(shouldBeActive);
            Debug.Log($"Heart {i}: {(shouldBeActive ? "ON" : "OFF")}");
        }
    }

}
