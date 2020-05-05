using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatSyncReceiver : MonoBehaviour
{

    float lastTargetBeatQueued = float.MinValue;
    
    /// <summary>
    /// The main beat receiver in the game
    /// </summary>
    public static BeatSyncReceiver BeatReceiver
    {
        private set;
        get;
    }

    private void Awake()
    {
        if(BeatReceiver != null)
        {
            Destroy(BeatReceiver.gameObject);
        }

        // Assign this instance as the beat receiver
        BeatReceiver = this;
        
    }

    /// <summary>
    /// Method called to queue a beat to the beat receiver
    /// </summary>
    /// <param name="beatData"></param>
    public void QueueBeat(BeatSyncData beatData)
    {
        // If the beat target time is less than the last target beat queued, then log an error
        // This is not allowed
        if(beatData.BeatTargetTime < lastTargetBeatQueued)
        {
            Debug.LogError("You cannot queue a beat that will be audible before the last beat queued. Last beat queued playback time = " + lastTargetBeatQueued + "; attempted beat queue play time = " + beatData.BeatTargetDSPTime);
            return;
        }
        // Assign the beat data's target time to the last target beat queued variable
        lastTargetBeatQueued = beatData.BeatTargetTime;
        // Process the beat
        StartCoroutine(ProcessBeat(beatData));
    }
    
    IEnumerator ProcessBeat(BeatSyncData beat)
    {
        Debug.Log("Preview Beat!");
        while(AudioSettings.dspTime < beat.BeatTargetDSPTime)
        {
            yield return null;
        }
        Debug.Log("Beat!");
    }

}

/// <summary>
/// Class used to store timing data of a beat
/// </summary>
public class BeatSyncData
{
    /// <summary>
    /// The beat's target time (in AudioSettings.DSPtime)
    /// </summary>
    public double BeatTargetDSPTime
    {
        get;
        private set;
    }

    /// <summary>
    /// The beat's creation time (in AudioSettings.DSPtime)
    /// </summary>
    public double BeatCreationAudioDSPScale
    {
        get;
        private set;
    }

    /// <summary>
    /// The beat creation time (in Time.time scale)
    /// </summary>
    public float BeatCreationTimeScale
    {
        get;
        private set;
    }

    /// <summary>
    /// The beat target time (in Time.time scale)
    /// </summary>
    public float BeatTargetTime
    {
        private set;
        get;
    }

    /// <summary>
    /// Method called to construct a beat sync data class (Note: Creation timestamps are cached upon construction)
    /// </summary>
    /// <param name="beatTargetTime"></param>
    public BeatSyncData(double beatTargetTime)
    {
        // Cache the beat target dsp time
        this.BeatTargetDSPTime = beatTargetTime;
        // Cache the beat creation time (dsp time)
        this.BeatCreationAudioDSPScale = AudioSettings.dspTime;
        // Cache the beat creation time (regular time)
        this.BeatCreationTimeScale = Time.time;
        // Cache the beat target time (regular time) 
        // This is the current time + (seconds until the beat target time)
        BeatTargetTime = BeatCreationTimeScale + (float) (BeatTargetDSPTime - BeatCreationAudioDSPScale);
    }
}

