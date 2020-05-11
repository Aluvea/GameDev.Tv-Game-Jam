using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    [SerializeField] RectTransform healthMeterFill;

    [Range(0.0f,1.0f)]
    [SerializeField]
    float healthMeterFillAMT;

    [SerializeField] bool debugHealthMeter = false;

    float startingHealthMeterWidth;
    Vector2 healthMeterSize;
    private bool initialized = false;

    private void Awake()
    {
        InitializeHealthBar();
    }

    private void Update()
    {
        if (debugHealthMeter)
        {
            UpdateHealthUIMeter(this.healthMeterFillAMT);
        }
        
    }

    /// <summary>
    /// Updates the health meter UI filled amount
    /// </summary>
    /// <param name="healthMeterFillAMT">The health meter's filled amount on a scale of 0 - 1</param>
    public void UpdateHealthUIMeter(float healthMeterFillAMT)
    {
        InitializeHealthBar();
        this.healthMeterFillAMT = Mathf.Clamp01(healthMeterFillAMT);
        healthMeterSize.x = startingHealthMeterWidth * this.healthMeterFillAMT;
        healthMeterFill.sizeDelta = healthMeterSize;
    }

    /// <summary>
    /// Updates the health meter UI filled amount
    /// </summary>
    /// <param name="currentHealth">The player's current health</param>
    /// <param name="maxHealth">The player's max health</param>
    public void UpdateHealthUIMeter(float currentHealth, float maxHealth)
    {
        UpdateHealthUIMeter(currentHealth / maxHealth);
    }

    private void InitializeHealthBar()
    {
        if (initialized) return;
        healthMeterSize = healthMeterFill.sizeDelta;
        startingHealthMeterWidth = healthMeterSize.x;
        initialized = true;
    }

}
