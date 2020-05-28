using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMech : MonoBehaviour
{
    [SerializeField] float rotateSpeed;

    [Header("Legs Game Object References")]
    [SerializeField] EnemyHealth frontRightLegHealth;
    [SerializeField] EnemyHealth frontLeftLegHealth;
    [SerializeField] EnemyHealth rearRightLegHealth;
    [SerializeField] EnemyHealth rearLeftLegHealth;
    [SerializeField] BossMechDestructorAnimator destructorAnimator;

    
    [SerializeField] BossCannon cannonL;
    [SerializeField] BossCannon cannonR;

    [Header("Attack Settings")]
    [SerializeField] float attackFrequency = 5.0f;
    [SerializeField] float attackBulletDamage = 1.4f;
    [Header("Bullet Barrage Settings")]
    [Tooltip("How many bullets should be shot in each wave of bullet barages")]
    [SerializeField] int bulletBarageCount = 15;
    [Tooltip("How many bullets should hit the player with each wave of bullet barages")]
    [SerializeField] int maxBulletsToHitPlayerInBarage = 4;
    [Min(0.01f)]
    [Tooltip("How quickly the bullets should be shot from each other in a bullet barage wave")]
    [SerializeField] float bulletBarageShotFrequency = 0.08f;
    [Tooltip("The maximum seconds randomized and added to the bullet barage shot frequency (to make it look / feel more authentic)")]
    [Range(0.0f, 5.0f)]
    [SerializeField] float bulletBarageShotFrequencySalt = 0.08f;

    private void Start()
    {
        StartCoroutine(BossMechExecute());
        frontRightLegHealth.Died += OnLegDied;
        frontLeftLegHealth.Died += OnLegDied;
        rearRightLegHealth.Died += OnLegDied;
        rearLeftLegHealth.Died += OnLegDied;
        StartCoroutine(Attack());
    }

    private void OnLegDied(EnemyHealth legDead)
    {
        int deadLegCount = 0;
        if (frontRightLegHealth == null || frontRightLegHealth == legDead) deadLegCount++;
        if (frontLeftLegHealth == null || frontLeftLegHealth == legDead) deadLegCount++;
        if (rearRightLegHealth == null || rearRightLegHealth == legDead) deadLegCount++;
        if (rearLeftLegHealth == null || rearLeftLegHealth == legDead) deadLegCount++;
        destructorAnimator.LegDied();

        if (deadLegCount == 4)
        {
            OnAllLegsDied();
        }
    }

    private void OnAllLegsDied()
    {

    }

    IEnumerator Attack()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackFrequency);
            if(cannonL != null || cannonR != null)
            {
                yield return FireCannons();
            }
        }
    }

    IEnumerator FireCannons()
    {
        // Play some machine gun reving sound?
        int shotsFired = 0;
        int shotsToHitPlayer = maxBulletsToHitPlayerInBarage;

        BossCannon cannonUsed;
        bool shouldHit = false;
        while (shotsFired < bulletBarageCount && (cannonL != null || cannonR != null))
        {
            cannonUsed = GetCanon();
            if (shotsToHitPlayer == 0) { shouldHit = false; }
            else
            {
                shouldHit = Random.Range(0.0f, 1.0f) < 0.5f;
                if(shouldHit) shotsToHitPlayer--;
            }

            if (cannonUsed != null) cannonUsed.ShootPlayer(attackBulletDamage, shouldHit);
            shotsFired++;
            yield return new WaitForSeconds(bulletBarageShotFrequency + Random.Range(0.0f, bulletBarageShotFrequencySalt));
        }

    }

    BossCannon lastCannonShot = null;

    BossCannon GetCanon()
    {
        if (cannonL == null) return cannonR;
        if (cannonR == null) return cannonL;
        lastCannonShot = lastCannonShot == cannonR ? cannonL : cannonR;
        return lastCannonShot;
    }


    IEnumerator BossMechExecute()
    {
        while (true)
        {
            Vector3 direction = PlayerController.PlayerCamera.transform.position - transform.position;
            direction.y = transform.position.y;

            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction, transform.up), rotateSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
