using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class used to manage beat sync indication UI (PERFECT, GOOD, OK, and MISS)
/// </summary>
public class BeatSyncIndicatorManager : MonoBehaviour
{
    [Tooltip("The beat sync prefab")]
    [SerializeField] BeatSyncIndicator beatSyncPrefab;
    [Tooltip("The left spawn position reference of a beat sync indicator")]
    [SerializeField] Transform leftBeatIndicatorPositionRef;
    [Tooltip("The right spawn position reference of a beat sync indicator")]
    [SerializeField] Transform rightBeatIndicatorPositionRef;

    
    /// <summary>
    /// The indicator pool queue
    /// </summary>
    Queue<BeatSyncIndicator> indicatorPool = new Queue<BeatSyncIndicator>();
    

    private void Start()
    {
        SetupBeatSyncIndicator();
    }

    private void OnDestroy()
    {
        DeconstructBeatSyncIndicator();
    }

    private void SetupBeatSyncIndicator()
    {
        if(BeatSyncReceiver.BeatReceiver != null)
        {
            BeatSyncReceiver.BeatReceiver.PlayerInputSynced += OnPlayerInputReceived;
        }
        if(PlayerController.PlayerCamera != null)
        {
            GetComponent<Canvas>().worldCamera = PlayerController.PlayerCamera.transform.Find("FPS Camera").GetComponent<Camera>();
        }
    }

    private void DeconstructBeatSyncIndicator()
    {
        if (BeatSyncReceiver.BeatReceiver != null)
        {
            BeatSyncReceiver.BeatReceiver.PlayerInputSynced -= OnPlayerInputReceived;
        }
    }

    private void OnPlayerInputReceived(BeatSyncData beatData, BeatInputSync syncType)
    {
        SpawnSyncIndicator(syncType);
    }


    /// <summary>
    /// Method called to spawn a beat sync indicator
    /// </summary>
    /// <param name="syncType"></param>
    private void SpawnSyncIndicator(BeatInputSync syncType)
    {
        // Cache the beat sync indicator about to be used
        BeatSyncIndicator indicatorSpawned;
        // Get the next position, rotation, and sign of the next beat UI
        GetNextBeatSettings(out Vector3 nextPosition, out Quaternion nextRotation, out float targetSign);
        // If the indicator pool queue is empty, then create a new beat indicator
        if (indicatorPool.Count == 0)
        {
            // Instantiate the beat sync prefab at the correct position / rotation
            indicatorSpawned = Instantiate(beatSyncPrefab, nextPosition, nextRotation, transform);
            // Assign its target sign
            indicatorSpawned.ChangeBounceSettings(targetSign);
            // Play the sync animation
            indicatorSpawned.PlayAnimation(syncType);
            // Subscribe to its on animation ended event
            indicatorSpawned.OnAnimationEnded += IndicatorSpawned_OnAnimationEnded;
        }
        // Otherwise, pull an indicator UI from our pool
        else
        {
            // Dequeue the next indicator
            indicatorSpawned = indicatorPool.Dequeue();
            // Assign its proper position / rotation / target sign
            indicatorSpawned.transform.position = nextPosition;
            indicatorSpawned.transform.rotation = nextRotation;
            indicatorSpawned.ChangeBounceSettings(targetSign);
            // Play the sync animation
            indicatorSpawned.PlayAnimation(syncType);
        }
    }

    public void SpawnSyncIndicator(int syncCounterPart)
    {
        /// Reference Link: https://stackoverflow.com/a/856165
        if (System.Enum.GetNames(typeof(BeatInputSync)).Length > syncCounterPart && syncCounterPart >= 0)
        {
            SpawnSyncIndicator((BeatInputSync)syncCounterPart);
        }
        else
        {
            Debug.LogError("Could not cast " + syncCounterPart + " to a BeatInputSync enum");
        }
    }

    /// <summary>
    /// Method intended to be subscribed to beat sync indicators when they're done playing animations. This re-adds the indicator into the queue pool
    /// </summary>
    /// <param name="beatPlayer"></param>
    private void IndicatorSpawned_OnAnimationEnded(BeatSyncIndicator beatPlayer)
    {
        // Add the indicator to our object pool
        indicatorPool.Enqueue(beatPlayer);
    }

    /// <summary>
    /// The last position used for a beat indicator
    /// </summary>
    Transform lastSpawnPositionUsed;
    
    /// <summary>
    /// Returns the next beat indicator settings (position, rotation, target sign)
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="targetSign"></param>
    private void GetNextBeatSettings(out Vector3 position, out Quaternion rotation, out float targetSign)
    {
        // If the last spawn position used was the right indication reference position,
        // Then return the left reference position, rotation, and sign
        if (lastSpawnPositionUsed == rightBeatIndicatorPositionRef)
        {
            // Alternate to the last spawn position used
            lastSpawnPositionUsed = leftBeatIndicatorPositionRef;
            position = lastSpawnPositionUsed.position;
            rotation = lastSpawnPositionUsed.rotation;
            targetSign = 1.0f;
        }
        // Otherwise the last spawn position used was the left indication reference position,
        // Then return the right reference position, rotation, and sign
        else
        {
            lastSpawnPositionUsed = rightBeatIndicatorPositionRef;
            position = lastSpawnPositionUsed.position;
            rotation = lastSpawnPositionUsed.rotation;
            targetSign = -1.0f;
        }
    }


}
