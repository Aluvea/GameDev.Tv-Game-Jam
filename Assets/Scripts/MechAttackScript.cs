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

    [Min(0.0f)]
    [Tooltip("How much damage this mech will deal when it hits the target")]
    [SerializeField] float hitDamage = 0.5f;

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

    public void StartAttackingTarget(Transform target)
    {
        if(attackCoroutine != null)
        {
            Debug.LogWarning("StartAttackingTarget was called but this mech attack script is already targeting another target!");
            StopAttackingTarget();
        }

        attackCoroutine = StartCoroutine(AttackTargetCoroutine(target));
    }

    public void StopAttackingTarget()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
            ikSetup.StopInverseKinematics();
        }
    }

    IEnumerator AttackTargetCoroutine(Transform target)
    {
        Vector3 lookAtDirection;

        ikSetup.SetupIKTargetAnimation(target);
        float nextAttackTimestamp = Time.time + attackFrequency + Random.Range(0.0f, attackFrequencyMaxRandomizedTimeModifier);
        currentTarget = target;

        while (true)
        {
            lookAtDirection = target.position - mechRootTransformRef.position;
            lookAtDirection.y = 0.0f;
            mechRootTransformRef.rotation = Quaternion.LookRotation(lookAtDirection, mechRootTransformRef.up);
            if(Time.time >= nextAttackTimestamp)
            {
                nextAttackTimestamp = Time.time + attackFrequency + Random.Range(0.0f, attackFrequencyMaxRandomizedTimeModifier);

                if(IsTargetInLineOfSight(target))
                {
                    Debug.LogWarning("Target is in line of sight!");
                    shouldHitTarget = Random.Range(0, 1.0f) < hitChance;
                    mechAnimator.PlayAttackAnimation();
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

                
            }
            yield return null;
        }
    }

    /// <summary>
    /// Returns whether or not a given transform target is in a line of sight
    /// </summary>
    /// <param name="target">The target transform reference</param>
    /// <returns></returns>
    private bool IsTargetInLineOfSight(Transform target)
    {
        Vector3 rayDirection = target.position - sightReference.position;
        float lineOfSightDistance = Vector3.Distance(target.position, sightReference.position);

        Ray lineOfSightRay = new Ray(sightReference.position, rayDirection);
        //Debug.DrawRay(sightReference.position, rayDirection, Color.red, 3.0f);
        bool hitSomething = Physics.Raycast(lineOfSightRay, out RaycastHit raycastHit, lineOfSightDistance, sightReferenceMask.value);
        if(hitSomething)
        {
            return raycastHit.collider.transform == target;
        }
        else
        {
            return true;
        }
    }

}
