﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float hitPoints = 3f;
    [SerializeField] EnemyAI enemyAIRef;
    [SerializeField] CyberBugAnimationController bugAnimator;

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
            enemyAIRef.Provoke();
            hitPoints -= damage;
        }
        else if (hitPoints <= 0)
        {
            bugAnimator.PlayDeathAnimation();
            GetComponent<AIRoamingController>().enabled = false;
        }
    }

    private void OnDestroy()
    {
        Destroy(gameObject);
    }
}
