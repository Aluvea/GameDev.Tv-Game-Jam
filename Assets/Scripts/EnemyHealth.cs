using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float hitPoints = 3f;
    [Tooltip("The enemy script used to handle taking damage")]
    [SerializeField] EnemyDamageHandler enemyDamageHandler;
    [SerializeField] Animations.AnimationController enemyAnimator;
    [SerializeField] LockableTarget lockableTargetReference;

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
            enemyDamageHandler.OnDamageTaken();
            hitPoints -= damage;
            if (lockableTargetReference != null) lockableTargetReference.OnDamageTaken();
        }
        else if (hitPoints <= 0)
        {
            if (lockableTargetReference != null)
            {
                lockableTargetReference.OnDamageTaken();
                lockableTargetReference.ToggleLockableTarget(false);
            }
            if(enemyAnimator != null)
            {
                if (enemyAnimator is Animations.IPlayDeathAnimation)
                {
                    (enemyAnimator as Animations.IPlayDeathAnimation).PlayDeathAnimation();
                }
            }
            GetComponent<AIRoamingController>().enabled = false;
        }
    }

    private void OnDestroy()
    {
        if (lockableTargetReference != null)
        {
            lockableTargetReference.ToggleLockableTarget(false);
        }
        Destroy(gameObject);
    }

    public LockableTarget LockableTargetReference
    {
        get
        {
            return lockableTargetReference;
        }
    }
}
