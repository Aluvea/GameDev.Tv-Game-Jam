using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] HealthUI healthUI;
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {

        if (currentHealth > 0)
        {
            currentHealth -= damage;
            healthUI.UpdateHealthUIMeter(currentHealth, maxHealth);
        }

        else if (currentHealth <= 0)
        {
            print("Player is dead");
        }
    }
}
