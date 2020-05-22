using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class used for lockable targets, objects with this script will attempt to get a beat map  
/// UI around them when they are visible to the camera
/// </summary>
public class LockableTarget : MonoBehaviour
{
    [Tooltip("This is the rendering object that the beat map circles will follow and surround")]
    [SerializeField] Renderer targetRendererReference;

    [SerializeField] bool targetIsLockableOnAwake = true;

    [Header("Test Beat Map UI")]
    [SerializeField] bool testBeatMap;
    [SerializeField] string testInputButtonName = "Fire1";
    [SerializeField] KeyCode requestBeatMapUIButton;

    private bool targetLockable = true;

    private void Start()
    {
        targetLockable = !targetIsLockableOnAwake;
        ToggleLockableTarget(targetIsLockableOnAwake);
    }

    /// <summary>
    /// This lockable target's renderer reference
    /// </summary>
    public Renderer TargetRenderer
    {
        get { return targetRendererReference; }
    }

    /// <summary>
    /// The targetable beat map assigned to this lockable target
    /// </summary>
    private TargetableBeatMapUI assignedBeatMapUI;

    public void AssignTargetableBeatMap(TargetableBeatMapUI targetableBeatMapUI)
    {
        this.assignedBeatMapUI = targetableBeatMapUI;
    }

    private void Update()
    {
        // If this beat map is supposed to be tested, then continue checking for the test input button
        if (testBeatMap)
        {
            if (Input.GetButtonDown(testInputButtonName))
            {
                // Request an input action
                if (BeatSyncReceiver.BeatReceiver.RequestInputAction())
                {
                    OnDamageTaken();
                }
            }
        }
    }

    /// <summary>
    /// Method called when this target takes damage
    /// </summary>
    public void OnDamageTaken()
    {
        if (assignedBeatMapUI != null)
        {
            // If the request was successfull, then update the beat sample display with the hit beat
            assignedBeatMapUI.UpdateDisplayedBeatSample(BeatSyncReceiver.BeatReceiver.LastBeatHit, BeatSyncReceiver.BeatReceiver.LastBeatHitSyncType);
        }
        else
        {
            Debug.LogWarning("Lockable target took damage but it has no beat map UI to display to the user!");
        }
    }

    /// <summary>
    /// Toggle whether this target is lockable to a beatmap UI
    /// </summary>
    /// <param name="targetLockable">Whether or not this target is lockable to a beat map UI</param>
    public void ToggleLockableTarget(bool targetLockable)
    {
        if (TargetBeatMapManager.TargetBeatMapManagerSingleton == null) throw new System.Exception("LOCKABLE TARGET CAN'T FIND A TARGETABLE BEAT MAP MANAGER! DID YOU FORGET TO PUT THE BEAT MAP TARGET MANAGER PREFAB IN YOUR SCENE?");
        if (this.targetLockable == targetLockable) return;
        // Otherwise, change whether or not this target is lockable
        this.targetLockable = targetLockable;
        // Notify the target beat map manager whether or not this target is lockable
        if (this.targetLockable == false)
        {
            TargetBeatMapManager.TargetBeatMapManagerSingleton.OnTargetUnlockable(this);
        }
        else
        {
            TargetBeatMapManager.TargetBeatMapManagerSingleton.OnTargetLockable(this);
        }
    }

    private void OnDestroy()
    {
        if(targetLockable)TargetBeatMapManager.TargetBeatMapManagerSingleton.OnTargetUnlockable(this);
    }



}
