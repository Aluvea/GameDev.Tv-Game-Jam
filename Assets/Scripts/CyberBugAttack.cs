using Animations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyberBugAttack : MonoBehaviour
{
    PlayerHealth target;
    [SerializeField] float damage = 3f;

    // Start is called before the first frame update
    void Start()
    {
        target = FindObjectOfType<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AttackHitEvent()
    {
        if (target == null) return;
        target.TakeDamage(damage);
    }
}
