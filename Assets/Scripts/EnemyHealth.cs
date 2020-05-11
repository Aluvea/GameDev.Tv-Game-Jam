﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float hitPoints = 3f;
    [SerializeField] float damage;

    // Create a PUBLIC method that reduces hitpoints by the amount of damage


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        if (hitPoints > 0)
        {
            hitPoints -= damage;
        }
        else if (hitPoints <= 0)
        {
            Destroy(gameObject);
        }
    }
}