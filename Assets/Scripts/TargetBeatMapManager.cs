using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Target Beat Map Manager responsible for assigning beat maps to targets
/// </summary>
public class TargetBeatMapManager : MonoBehaviour
{
    [Header("Targetable Beat Map References")]
    [SerializeField] TargetableBeatMapUI targetableBeatMapA;
    [SerializeField] TargetableBeatMapUI targetableBeatMapB;
    [Tooltip("Layer mask applied when testing if an enemy is in the crosshair")]
    [SerializeField] LayerMask enemyRayTesterMask;
    [Tooltip("Layer mask applied when testing if obstacles are between the player and an enemy")]
    [SerializeField] LayerMask obstacleTesterMask;
    private TargetableBeatMapUI currentlyActiveBeatMap;
    private LockableTarget currentTarget = null;

    private void Awake()
    {
        TargetBeatMapManagerSingleton = this;
        currentlyActiveBeatMap = targetableBeatMapA;
        StartCoroutine(MonitorNextVisibleTargetToLockOnto());
    }

    private void Start()
    {
        if (PlayerController.PlayerCamera != null)
        {
            Canvas targetCanvas = GetComponent<Canvas>();

            targetCanvas.worldCamera = PlayerController.PlayerCamera;
            targetCanvas.planeDistance = PlayerController.PlayerCamera.nearClipPlane + 0.15f;
        }
    }


    public static TargetBeatMapManager TargetBeatMapManagerSingleton
    {
        private set;
        get;
    } = null;

    /// <summary>
    /// Should be called by a target when it's lockable (visible to the main camera & lockable)
    /// </summary>
    /// <param name="lockableTarget">The lockable target</param>
    public void OnTargetLockable(LockableTarget lockableTarget)
    {
        if (lockableTarget == null) return;

        if (lockableTargets.Contains(lockableTarget) == true) return;

        // Add the target to our list of lockable targets
        lockableTargets.Add(lockableTarget);

        // Immediately evaluate 
        if(lockableTarget == GetNextTargetToLockOnto())
        {
            currentTarget = lockableTarget;
            currentlyActiveBeatMap.SetLockableTarget(null);
            currentlyActiveBeatMap = currentlyActiveBeatMap == targetableBeatMapA ? targetableBeatMapB : targetableBeatMapA;
            currentlyActiveBeatMap.SetLockableTarget(lockableTarget);
        }
    }

    /// <summary>
    /// Should be called by a target when it's unlockable (invisible to the main camera or unlockable)
    /// </summary>
    /// <param name="unlockableTarget">The unlockable target</param>
    public void OnTargetUnlockable(LockableTarget unlockableTarget)
    {
        // If our list of lockable targets doesn't have the target, then do nothing
        if (lockableTargets.Contains(unlockableTarget) == false) return;
        // Otherwise, remove the unlockable target from our list of lockable targets
        lockableTargets.Remove(unlockableTarget);
        // If this target is the current lockable target, then unlock it from a beat map UI
        if(currentTarget == unlockableTarget)
        {
            // Set the currently active beat map so it's locked onto nothing
            currentlyActiveBeatMap.SetLockableTarget(null);
            // Get the next target to lock onto
            currentTarget = GetNextTargetToLockOnto();
            // Alternate to the next beat map UI
            currentlyActiveBeatMap = currentlyActiveBeatMap == targetableBeatMapA ? targetableBeatMapB : targetableBeatMapA;
            // Lock onto the current target
            currentlyActiveBeatMap.SetLockableTarget(currentTarget);
        }
        
    }

    /// <summary>
    /// The target beat map manager's list of lockable targets (visible and invisible)
    /// </summary>
    List<LockableTarget> lockableTargets = new List<LockableTarget>();


    /// <summary>
    /// Returns the next target to lock onto
    /// </summary>
    /// <returns></returns>
    private LockableTarget GetNextTargetToLockOnto()
    {
        // If there are no targets to lock onto, then return null
        if (lockableTargets.Count == 0) return null;

        // Otherwise, proceed with calculations to see which target is the closest to the crosshair / closest target
        // Cache the next target to lock onto
        LockableTarget nextTargetToLockOnto = null;
        // The viewpoert distance of the target to the center of the screen
        float closestTargetMagnitude = 0.5f;
        float closestTargetToPlayer = 0.5f;
        nextTargetToLockOnto = GetLockableTargetFromRayCast();
        if (nextTargetToLockOnto != null) return nextTargetToLockOnto;

        for (int i = 0; i < lockableTargets.Count; i++)
        {
            // If the lockable target is null at the given index, remove it and continue
            if(lockableTargets[i] == null)
            {
                lockableTargets.RemoveAt(i);
                i--;
                continue;
            }
            if (lockableTargets[i].TargetIsLockable == false)
            {
                lockableTargets.RemoveAt(i);
                i--;
                continue;
            }

            // If the target is not visible, then continue checking the next lockable target
            if (IsTargetWithinCameraView(lockableTargets[i]) == false) continue;


            // If the next target to lock onto is null, then this target will be the next target to lock onto
            if(nextTargetToLockOnto == null)
            {
                nextTargetToLockOnto = lockableTargets[i];
                // Get the viewport position of this target
                Vector3 viewportPosition = PlayerController.PlayerCamera.WorldToViewportPoint(lockableTargets[i].transform.position);
                viewportPosition.x = Mathf.Abs(viewportPosition.x - 0.5f);
                viewportPosition.y = Mathf.Abs(viewportPosition.y - 0.5f);
                viewportPosition.z = 0.0f;
                // Cahce the magnitude of the distance of this object form the center of the screen
                closestTargetMagnitude = viewportPosition.magnitude;
                closestTargetToPlayer = Vector3.Distance(nextTargetToLockOnto.transform.position, PlayerController.PlayerCamera.transform.position);
                continue;
            }
            else
            {
                // get the viewport position of the next lockable target
                Vector3 viewportPosition = PlayerController.PlayerCamera.WorldToViewportPoint(lockableTargets[i].transform.position);
                viewportPosition.x = Mathf.Abs(viewportPosition.x - 0.5f);
                viewportPosition.y = Mathf.Abs(viewportPosition.y - 0.5f);
                viewportPosition.z = 0.0f;
                float distanceFromPlayer = Vector3.Distance(lockableTargets[i].transform.position, PlayerController.PlayerCamera.transform.position);
                // If the viewport magnitude is closer to the center of the screen than the previous target cached
                // Then assign this target as the next target to lock onto
                if (closestTargetMagnitude > viewportPosition.magnitude || (distanceFromPlayer < 5.0f && distanceFromPlayer < closestTargetToPlayer))
                {
                    closestTargetMagnitude = viewportPosition.magnitude;
                    closestTargetToPlayer = distanceFromPlayer;
                    nextTargetToLockOnto = lockableTargets[i];
                }
            }
        }
        // Return the lockable target identified
        return nextTargetToLockOnto;
    }

    private Ray targetTesterRay = new Ray();
    private RaycastHit hitCast;
    private bool hit;
    EnemyHealth healthFound;

    /// <summary>
    /// Returns a target hit from a ray cast from the camera
    /// </summary>
    /// <returns></returns>
    private LockableTarget GetLockableTargetFromRayCast()
    {
        // Create a ray from the camera looking at its forward direction
        targetTesterRay = new Ray(PlayerController.PlayerCamera.transform.position, PlayerController.PlayerCamera.transform.forward);
        // Raycast the ray
        hit = Physics.Raycast(targetTesterRay, out hitCast, PlayerController.PlayerCamera.farClipPlane + PlayerController.PlayerCamera.nearClipPlane, enemyRayTesterMask.value);
        // If a target is hit, then check if it's an enemy
        if (hit)
        {
            if(hitCast.transform != null)
            {
                // If the game object hit has an enemy health component, then return the lockable target reference
                healthFound = hitCast.transform.GetComponent<EnemyHealth>();
                if(healthFound != null)
                {
                    if (healthFound.LockableTargetReference.TargetIsLockable)
                    {
                        return healthFound.LockableTargetReference;
                    }
                    else
                    {
                        return null;
                    }
                }
                // Otherwise, return null
                else
                {
                    return null;
                }
            }
        }
        // If nothing was hit, then return null
        return null;
    }


    IEnumerator MonitorNextVisibleTargetToLockOnto()
    {
        LockableTarget nextTarget = null;

        yield return new WaitForSeconds(0.1f);

        while (true)
        {
            nextTarget = GetNextTargetToLockOnto();

            if (currentTarget != nextTarget)
            {
                currentTarget = nextTarget;
                currentlyActiveBeatMap.SetLockableTarget(null);
                currentlyActiveBeatMap = currentlyActiveBeatMap == targetableBeatMapA ? targetableBeatMapB : targetableBeatMapA;
                currentlyActiveBeatMap.SetLockableTarget(currentTarget);
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary>
    /// Returns whether or not the target renderer is visible to a given camera
    /// </summary>
    /// <returns></returns>
    private bool IsTargetWithinCameraView(LockableTarget target)
    {
        bool isWithinCameraFrustum = GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(PlayerController.PlayerCamera), target.TargetRenderer.bounds);
        if (isWithinCameraFrustum == false) return false;

        if (IsObstacleBetweenPlayerAndTarget(target)) return false;

        return true;
    }


    Vector3 testPosition;

    /// <summary>
    /// Returns whether or not an obstacle exists between the player and the target
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private bool IsObstacleBetweenPlayerAndTarget(LockableTarget target)
    {
        testPosition = target.TargetRenderer.transform.position;
        if (IsObstacleBetweenPlayerAndTarget(testPosition) == false) return false;
        testPosition.y -= target.TargetRenderer.bounds.extents.y;
        if (IsObstacleBetweenPlayerAndTarget(testPosition) == false) return false;
        testPosition.y += target.TargetRenderer.bounds.size.y;
        if (IsObstacleBetweenPlayerAndTarget(testPosition) == false) return false;
        return true;
    }

    private bool IsObstacleBetweenPlayerAndTarget(Vector3 worldPosition)
    {
        return Physics.Linecast(PlayerController.PlayerCamera.transform.position, worldPosition, obstacleTesterMask.value);
    }




}
