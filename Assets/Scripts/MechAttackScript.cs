using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechAttackScript : MonoBehaviour
{
    [Header("Game Object References")]
    [Tooltip("The mech's root transform reference")]
    [SerializeField] Transform mechRootTransformRef;
    [Tooltip("The mech's animator reference")]
    [SerializeField] MechAnimationController mechAnimator;
    [Tooltip("The mech's inverse kinematics setup reference")]
    [SerializeField] MechAttackInverseKinematicSetup ikSetup;
    [Tooltip("The mech's sight reference to test the line of sight towards the target")]
    [SerializeField] Transform sightReference;
    [Tooltip("The mech's gun muzzle reference for instantiating bullets to shoot from")]
    [SerializeField] Transform gunMuzzleReference;
    [SerializeField] Bullet bulletPrefab;
    [Tooltip("Layermask to test collisions against when checking the line of sight towards a target")]
    [SerializeField] LayerMask sightReferenceMask;
    [Header("Attack Settings")]
    [Tooltip("How often (in seconds) this mech will attack the target")]
    [Range(1.0f, 15.0f)]
    [SerializeField] float attackFrequency = 5.0f;
    [Tooltip("Randomized seconds modifier added to the attack frequency (randomized range will be between 0 and this value in seconds)")]
    [Range(0.0f,10.0f)]
    [SerializeField] float attackFrequencyMaxRandomizedTimeModifier = 5.0f;
    [Tooltip("The likelihood that this mech will hit the target when it attacks (0 = Never, 1 = Always)")]
    [Range(0.0f, 1.0f)]
    [SerializeField] float hitChance = 0.5f;

    [Tooltip("The range at which the mech will never miss a shot")]
    [Range(0.0f, 15.0f)]
    [SerializeField] float neverMissRange = 5.0f;

    [Tooltip("The range at which the mech will stop attacking the target")]
    [Range(30.0f, 300.0f)]
    [SerializeField] float attackRange = 65.0f;

    [Min(0.0f)]
    [Tooltip("How much damage this mech will deal when it hits the target")]
    [SerializeField] float hitDamage = 0.5f;

    [Tooltip("The physical contact layermask applied to this mech's bullet projectiles")]
    [SerializeField] LayerMask projectileLayerMask;

    [Header("Testing Settings")]
    [SerializeField] bool testTargeting;
    [SerializeField] Transform testTargetTransformRef;
    

    Coroutine attackCoroutine = null;

    private Transform currentTarget;

    bool shouldHitTarget = false;

    private void Start()
    {
        if(testTargeting && testTargetTransformRef != null)
        {
            StartAttackingTarget(testTargetTransformRef);
        }
    }

    /// <summary>
    /// Method called to start attacking a target
    /// </summary>
    /// <param name="target"></param>
    public void StartAttackingTarget(Transform target)
    {
        if(attackCoroutine != null)
        {
            Debug.LogWarning("StartAttackingTarget was called but this mech attack script is already targeting another target!");
            StopAttackingTarget();
        }

        attackCoroutine = StartCoroutine(AttackTargetCoroutine(target));
    }

    /// <summary>
    /// Method called to make this mech stop attacking the target
    /// </summary>
    public void StopAttackingTarget()
    {
        // If the attack coroutine is not null, then stop it
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
            // Also stop the inverse kinematics animation
            ikSetup.StopInverseKinematics();
        }
    }

    IEnumerator AttackTargetCoroutine(Transform target)
    {
        // Cahce a Vector3 direction to look at
        Vector3 lookAtDirection;
        // Setup the inverse kinematic to keep looking at the target
        ikSetup.SetupIKTargetAnimation(target);
        // Cache the next timestamp when the mech should attack (based on frequency)
        float nextAttackTimestamp = Time.time + attackFrequency + Random.Range(0.0f, attackFrequencyMaxRandomizedTimeModifier);
        // Assign the current target to the target variable
        currentTarget = target;
        // Keep attacking indefinitely, this coroutine will be stopped if StopAttacking is called
        while (true)
        {
            // Cache the direction this mech should look at
            lookAtDirection = target.position - mechRootTransformRef.position;
            // Reset the y axis direction
            lookAtDirection.y = 0.0f;
            // Rotate the mech towards the player
            mechRootTransformRef.rotation = Quaternion.LookRotation(lookAtDirection, mechRootTransformRef.up);
            // If the mech should attack, then proceed to do the attack check
            if(Time.time >= nextAttackTimestamp)
            {
                // Check if we can attack the target, also get the target's distance
                if(CanAttackTarget(target, out float targetDistance))
                {
                    Debug.LogWarning("Target is in line of sight!");
                    // If the target's distance is less than the never miss range, then hit the target
                    // Otherwise, set the hit target based on the hitchance assigned
                    shouldHitTarget = (targetDistance <= neverMissRange) ? true : Random.Range(0, 1.0f) < hitChance;
                    // Play the attack animation
                    mechAnimator.PlayAttackAnimation();
                    // Get the bullet's target position
                    Vector3 bulletTargetPosition = GetBulletTargetPosition(target, shouldHitTarget);
                    // Shoot a bullet projectile towards at the bullet target position determined
                    ShootBulletProjectile(bulletTargetPosition, hitDamage);
                    // Log if the target should be hit or missed
                    if (shouldHitTarget)
                    {
                        Debug.LogWarning("Mech Would Have Hit Target!");
                    }
                    else
                    {
                        Debug.LogWarning("Mech Would Have Missed Target!");
                    }
                }
                else
                {
                    Debug.LogWarning("Target is not in line of sight!");
                }

                // Setup the next attack timestamp
                nextAttackTimestamp = Time.time + attackFrequency + Random.Range(0.0f, attackFrequencyMaxRandomizedTimeModifier);
            }
            yield return null;
        }
    }

    Queue<Bullet> bulletObjectPool = new Queue<Bullet>();
    List<Bullet> instantiatedBullets = new List<Bullet>();

    /// <summary>
    /// Shoots a bullet projectile at a given target position
    /// </summary>
    /// <param name="targetPosition">The bullet projectile target position</param>
    /// <param name="hitDamage">The damage applied on hit</param>
    void ShootBulletProjectile(Vector3 targetPosition, float hitDamage)
    {
        // If the bullet object pool is available, then dequeue a bullet projectile
        if(bulletObjectPool.Count > 0)
        {
            Bullet dequeuedBullet = bulletObjectPool.Dequeue();
            // Reset its position to the gun muzzle position
            dequeuedBullet.transform.position = gunMuzzleReference.position;
            // Enable the game object
            dequeuedBullet.gameObject.SetActive(true);
            // Project the bullet at the target position
            dequeuedBullet.ProjectBullet(targetPosition, hitDamage, projectileLayerMask);
        }
        else
        {
            // Create a bullet projectile
            Bullet bulletCreated = Instantiate(bulletPrefab, gunMuzzleReference.position, Quaternion.identity);
            // Shoot it at the target position
            bulletCreated.ProjectBullet(targetPosition, hitDamage, projectileLayerMask);
            // Add the bullet to our list of instantiated bullets
            instantiatedBullets.Add(bulletCreated);
            // Subscribe to the bullet's ending travel event
            bulletCreated.BulletTravelEnded += OnBulletTravelEnded;
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe to our list of instantiated bullets
        foreach (Bullet bulletCreated in instantiatedBullets)
        {
            if(bulletCreated != null) bulletCreated.BulletTravelEnded -= OnBulletTravelEnded;
        }

        // Destroy the bullets in our object pool
        while(bulletObjectPool.Count > 0)
        {
            Bullet dequeuedBullet = bulletObjectPool.Dequeue();
            if(dequeuedBullet != null)
            {
                if(dequeuedBullet.gameObject != null) Destroy(dequeuedBullet.gameObject);
            }
        }
    }

    /// <summary>
    /// Callback method raised when a bullet is done travelling towards its destination
    /// </summary>
    /// <param name="bullet"></param>
    void OnBulletTravelEnded(Bullet bullet)
    {
        // Disable the game object
        bullet.gameObject.SetActive(false);
        // Enqueue the bullet into our object pool
        bulletObjectPool.Enqueue(bullet);
    }

    /// <summary>
    /// Returns whether or not a given transform target is in a line of sight
    /// </summary>
    /// <param name="target">The target transform reference</param>
    /// <returns></returns>
    private bool CanAttackTarget(Transform target, out float targetDistance)
    {
        // Return the target's distance from the mech
        targetDistance = Vector3.Distance(target.position, sightReference.position);
        // If the target's distance is greater than the attack range setup, then the mech
        // cannot attack the target
        if (targetDistance >= attackRange) return false;

        // Cache the direction from the mech's line of sight to the target
        Vector3 rayDirection = target.position - sightReference.position;
        // Create a ray from the line of sight position towards the target
        Ray lineOfSightRay = new Ray(sightReference.position, rayDirection);
        // Check the line of sight by raycasting
        // Cache whether something was hit or not
        bool hitSomething = Physics.Raycast(lineOfSightRay, out RaycastHit raycastHit, targetDistance, sightReferenceMask.value);
        // If something was hit, return whether or not the object hit was the target
        if(hitSomething)
        {
            return raycastHit.collider.transform == target;
        }
        // Otherwise, return false
        else
        {
            return true;
        }
    }


    /// <summary>
    /// Retuns a bullet projectile destination position based on a target and whether or not the target should be hit
    /// </summary>
    /// <param name="target"></param>
    /// <param name="shouldHit"></param>
    /// <returns></returns>
    private Vector3 GetBulletTargetPosition(Transform target, bool shouldHit)
    {
        // If the target should be missed, then offset the bullet target position towards on the x and y axis
        if (shouldHit == false)
        {
            float randomXAxisMissOffset = Random.Range(0.0f, 2f);
            float randomYAxisMissOffset = 2.0f - randomXAxisMissOffset;
            return target.position + (gunMuzzleReference.right * randomXAxisMissOffset) + (randomYAxisMissOffset * gunMuzzleReference.up);
        }
        // If the target should be hit, then offset the bullet target position down a little bit
        // It's annoying for bullets to be flying into the player's line of sight / camera
        else
        {
            return target.position - new Vector3(0.0f, 0.3f, 0.0f);
        }
    }

}
