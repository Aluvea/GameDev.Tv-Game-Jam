using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIRoamingController : MonoBehaviour
{
    public UnityEngine.AI.NavMeshAgent agent { get; private set; }             // the navmesh agent required for the path finding

    [Header("Roaming Controller Settings")]

    [Tooltip("The character movement controller script of the AI")]
    [SerializeField] CharacterMovementController characterMovementController;

    [Tooltip("The distance the AI should stop from the destination position")]
    /// <summary>
    /// The distance required between the citizen and the destination position before the citizen stops and selects another position to move towards
    /// </summary>
    [SerializeField] float agentStoppingDistance = 0.0f;

    [Tooltip("The the count of seconds the AI should pause when it reaches the destination position")]
    [Range(0.0f, 30.0f)]
    [SerializeField] float pauseDurationUponReachingDestination = 0.0f;
    [Tooltip("The type of roaming this AI should execute; Randomized or Defined pathing roam")]
    [SerializeField] RoamMode roamingMode;
    [Tooltip("Should this AI start roaming when the game object is intialized?")]
    [SerializeField] bool startRoamingOnAwake;
    [Tooltip("The roam speed of the AI (0 - 1 of the character's max speed)")]
    [Range(0.05f,1.0f)]
    [SerializeField] float roamSpeed = 0.5f;

    [Header("Randomized Pathing Settings")]

    [Tooltip("How far a randomized path destination is from the AI")]
    /// <summary>
    /// The maximum distance the citizen can wander towards when selecting a random destination
    /// </summary>
    [SerializeField] float randomizedPathRange = 15.0f;
    [Tooltip("The count of seconds the AI should wander towards a randomized path (this is useful in the event that the AI can't reach the randomized destination)")]
    /// <summary>
    /// The amount of seconds needed for this citizen to reselect a random position to move towards
    /// </summary>
    [SerializeField]
    float pathWanderDuration = 5.0f;

    /// <summary>
    /// The layermask name of the NavMesh used to create randomized pathing
    /// </summary>
    [Tooltip("The layermask name of the NavMesh used to create randomized pathing")]
    [SerializeField] string navMeshLayerName = "NavMesh";

    /// <summary>
    /// The path position references array
    /// </summary>
    [Header("Defined Pathing Settings")]
    [Tooltip("Enter the transform reference positions for this AI to cycle and roam towards")]
    [SerializeField] Transform [] pathPositionReferences;
    
    /// <summary>
    /// The roaming coroutine of this AI
    /// </summary>
    Coroutine roamingCoroutine = null;

    private void Start()
    {
        // get the components on the object we need ( should not be null due to require component so no need to check )
        agent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
        // If this AI should roam on awake, then start roaming
        if (startRoamingOnAwake)
        {
            SetRoam(true);
        }
    }

    private void OnDisable()
    {
        SetRoam(false);
    }

    /// <summary>
    /// Method called to make this AI start roaming
    /// </summary>
    /// <param name="roaming">Whether or not this AI should be roaming</param>
    public void SetRoam(bool roaming)
    {
        if (roaming)
        {
            // If the roaming coroutine is null, then start the roam coroutine
            if(roamingCoroutine == null)
            {
                roamingCoroutine = StartCoroutine(Roam());
            }
        }
        else
        {
            // If the romaing coroutine isn't null, then stop it
            // Reassign the roaoming coroutine to null
            if(roamingCoroutine != null)
            {
                StopCoroutine(roamingCoroutine);
                roamingCoroutine = null;
                if(characterMovementController != null) characterMovementController.MoveCharacter(Vector2.zero);
            }
        }
    }

    /// <summary>
    /// Coroutine called for this script to make AI roam. Note: This coroutine continues indefinitely unless explicitly stopped via StopCoroutine
    /// </summary>
    /// <returns></returns>
    IEnumerator Roam()
    {
        // Continue roaming indefinitely
        while (true)
        {
            // Cache the starting time when this random roaming destination was selected
            float startingRoamTimestamp = Time.time;
            // Get a next roaming position for this AI
            // This will return a random position or defined path position based on the settings
            Vector3 nextRoamDestinationPosition = GetNextRoamDestinationPosition();
            // Set the NavMesh agent's destination to the next roaming position
            agent.SetDestination(nextRoamDestinationPosition);
            // Wait for the agent path to completely generate
            while (agent.pathPending) yield return null;
            // Keep roaming until the agent reaches the destination point
            // or the path wandering duration has been exceeded (for randomized pathing)
            while (agent.remainingDistance > agentStoppingDistance &&

                ((Time.time - startingRoamTimestamp) < pathWanderDuration || roamingMode == RoamMode.DefinedPathing))
            {
                // Update the character movement controller with the navemesh desired velocity (converted to a 2D input vector)
                characterMovementController.MoveCharacter(CharacterMovementController.AgentVelocityToVector2DInput(agent) * roamSpeed);
                yield return null;
            }
            // Get the timestamp when the AI should resume pathfinding
            float resumePathFindingTimestamp = Time.time + pauseDurationUponReachingDestination;
            // While the time is less than the resume pathfinding timestamp, make the character idle
            while (Time.time < resumePathFindingTimestamp)
            {
                characterMovementController.MoveCharacter(Vector2.zero);
                yield return null;
            }
        }
    }

    /// <summary>
    /// The last defined path index used
    /// </summary>
    private int lastDefinedPathIndex = 0;

    /// <summary>
    /// Returns the next roaming destination position for the AI (based on the roaming mode settings)
    /// </summary>
    /// <returns></returns>
    private Vector3 GetNextRoamDestinationPosition()
    {
        // If the roaming mode is random, then return a randomized point
        if(roamingMode == RoamMode.RandomizedPathing)
        {
            return GetRandomNavMeshRoamPoint();
        }
        // Otherwise, return the next predefined path position reference
        else
        {
            // Get the path to return
            Vector3 pathToReturn = pathPositionReferences[lastDefinedPathIndex].position;
            // Increment the last defined path index returned
            lastDefinedPathIndex++;
            // If the index is at the max length of the array, then reset the index to 0
            if(lastDefinedPathIndex >= pathPositionReferences.Length)
            {
                lastDefinedPathIndex = 0;
            }
            // Return the path
            return pathToReturn;
        }
    }

    /// <summary>
    /// Returns a random position within a nav mesh for this citizen to roam towards
    /// </summary>
    /// <returns></returns>
    Vector3 GetRandomNavMeshRoamPoint()
    {
        // Cache the nav mesh layer mask
        int navMeshLayerMask = UnityEngine.AI.NavMesh.GetAreaFromName(navMeshLayerName);
        // Get a random position relative to the AI at the given wandering range and layer mask
        Vector3 randomPosition = RandomNavSphere(transform.position, randomizedPathRange, navMeshLayerMask);
        // If the distance between the random position and the citizen is less than 3, keep trying to pick a random nav mesh position
        while (Vector3.Distance(randomPosition, transform.position) < 3.0f)
        {
            randomPosition = RandomNavSphere(transform.position, randomizedPathRange, navMeshLayerMask);
        }
        // Finally, return the random nav mesh position
        return randomPosition;
    }

    /// <summary>
    /// Returns a random position within the given nav mesh layer
    /// </summary>
    /// <param name="origin">The origin point of the random position</param>
    /// <param name="distance">The distance of the randomly selected position</param>
    /// <param name="layermask">The layer mask applied when searching for a navmesh</param>
    /// <returns></returns>
    private Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
    {
        // Multiply a random direction by the distance provided
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * distance;
        // Add the origin point to the position
        randomDirection += origin;
        // Cache a navHit variable for the nav mesh data
        UnityEngine.AI.NavMeshHit navHit;
        // Get a random nav mesh position
        UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);
        // Return the nav mesh position
        return navHit.position;
    }

    /// <summary>
    /// Different roaming modes of AI
    /// </summary>
    public enum RoamMode { RandomizedPathing, DefinedPathing}

}
