using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class used for a beat sample ui manager
/// </summary>
public abstract class BeatSampleUIManager : MonoBehaviour
{
    
    /// <summary>
    /// Base method that needs to be overwritten for a beat sample UI manager to display a beat
    /// </summary>
    /// <param name="beatSyncData">The beat sync data</param>
    public abstract void DisplayBeat(BeatSyncData beatSyncData);

    /// <summary>
    /// Displays when the user inputs an action but misses a beat completely
    /// </summary>
    public abstract void DisplayInputMissedBeat();


    /// <summary>
    /// Updates the sync state of a beat sample UI
    /// </summary>
    /// <param name="beatHit">The beat hit</param>
    /// <param name="beatInput">The hit sync type</param>
    public abstract void UpdateBeatSyncUIState(BeatSyncData beatHit, BeatInputSync beatInput);

    private void SetupBeatUIManager()
    {
        if(BeatSyncReceiver.BeatReceiver != null)
        {
            BeatSyncReceiver.BeatReceiver.PlayerInputSynced += BeatReceiver_PlayerInputSynced;
            BeatSyncReceiver.BeatReceiver.QueuedBeatToSync += DisplayBeat;
        }
    }

    private void Start()
    {
        SetupBeatUIManager();
    }

    private void OnDestroy()
    {
        DeconstructUIManager();
    }

    private void DeconstructUIManager()
    {
        if (BeatSyncReceiver.BeatReceiver != null)
        {
            BeatSyncReceiver.BeatReceiver.PlayerInputSynced -= BeatReceiver_PlayerInputSynced;
            BeatSyncReceiver.BeatReceiver.QueuedBeatToSync -= DisplayBeat;
        }
    }

    private void BeatReceiver_PlayerInputSynced(BeatSyncData beatData, BeatInputSync syncType)
    {
        if(beatData == null && syncType == BeatInputSync.MISS)
        {
            this.DisplayInputMissedBeat();
        }
        else
        {
            this.UpdateBeatSyncUIState(beatData, syncType);
        }
    }
}


