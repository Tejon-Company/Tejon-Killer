using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LifeUI : MonoBehaviour
{
    public Image heartTemplate;
    public int maxLives = 3;        
    public PlayerHealth playerHealth;    
    private List<GameObject> hearts = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < maxLives; i++)
        {
            Image heart = Instantiate(heartTemplate, transform);
            heart.gameObject.SetActive(true); 
            hearts.Add(heart.gameObject);
        }
    }

    void Update()
    {
        UpdateHearts();
    }

    void UpdateHearts()
    {
        int currentLives = Mathf.CeilToInt(playerHealth.currentHealth / (playerHealth.maxHealth / maxLives));

        for (int i = 0; i < hearts.Count; i++)
        {
            hearts[i].SetActive(i < currentLives);
        }
    }
}
