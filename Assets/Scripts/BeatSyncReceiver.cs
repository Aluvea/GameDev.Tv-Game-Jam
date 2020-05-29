using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatSyncReceiver : MonoBehaviour
{

    [Header("Beat Sync Type Synchronization Settings")]
    [Range(0.0f,3.0f)]
    [SerializeField] float maxTimeToLandPerfectSync = 0.1f;
    [Range(0.0f, 3.0f)]
    [SerializeField] float maxTimeToLandGoodSync = 0.1f;
    [Range(0.0f, 3.0f)]
    [SerializeField] float maxTimeToLandOKSync = 0.1f;

    [Header("Singular Beat Queue Options")]
    [Tooltip("Limits the beat queue to only one beat at a time between the current beat's preview time to the audible time")]
    [SerializeField] bool limitConsecutiveBeatQueueToOne = false;
    [Tooltip("Optional additional time to include to the one-beat-to-queue time window")]
    [Range(0.0f,5.0f)]
    [SerializeField] float optionalConsecutiveBeatQueueOffset = 0.0f;

    [Header("Consecutive Beat Limit")]
    [SerializeField] bool limitConsecutiveBeatToTimeWindow = false;
    [Range(0.0f, 5.0f)]
    [SerializeField] float limitedConsecutiveBeatTimeWindow = 1.0f;


    [Header("Debug Receiver Settings")]
    [Tooltip("Whether or not you want to debug this beat sync receiver to respond to player input")]
    [SerializeField] bool debugReceiver = false;
    [Tooltip("The Unity Input button name to debug player input")]
    [SerializeField] string debugInputName = "Fire1";
    [Tooltip("Whether or not beats should be debugged in the console")]
    [SerializeField] bool debugBeatsInConsole;

    /// <summary>
    /// The last target beat queued
    /// </summary>
    BeatSyncData lastTargetBeatQueued = null;

    private int comboCount;

    

    /// <summary>
    /// The player's current beat sync combo count
    /// </summary>
    public int ComboCount
    {
        get
        {
            return comboCount;
        }
    }

    /// <summary>
    /// The last beat sync queued (If you want to observe beats when they're audible, please subscribe to the PlayedBeatToSync event)
    /// </summary>
    public BeatSyncData LastBeatQueued
    {
        get
        {
            return lastTargetBeatQueued;
        }
    }
    
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
        if (debugReceiver)
        {
            StartCoroutine(DebugReceiverCoroutine());
        }
    }



    /// <summary>
    /// Method called to queue a beat to the beat receiver
    /// </summary>
    /// <param name="beatData"></param>
    public void QueueBeat(BeatSyncData beatData)
    {
        // If the beat target time is less than the last target beat queued, then log an error
        // This is not allowed
        if(lastTargetBeatQueued != null)
        {
            if (beatData.BeatTargetTime < lastTargetBeatQueued.BeatTargetTime)
            {
                Debug.LogError("You cannot queue a beat that will be audible before the last beat queued. Last beat queued playback time = " + lastTargetBeatQueued.BeatMapTimestamp + "; attempted beat queue beatmap timestamp = " + beatData.BeatMapTimestamp + ", index = " + beatData.BeatMapTimestampIndex);
                return;
            }
            if (limitConsecutiveBeatQueueToOne)
            {
                if(lastTargetBeatQueued.BeatTargetTime + optionalConsecutiveBeatQueueOffset > Time.time)
                {
                    Debug.LogWarning("Skipping beat because the last beat hasn't been played yet UPCOMING BEAT: " + lastTargetBeatQueued.ToString() + "; ATTEMPTED QUEUED BEAT: " + beatData.ToString());
                    return;
                }
            }
            if (limitConsecutiveBeatToTimeWindow)
            {
                if(beatData.BeatTargetTime - lastTargetBeatQueued.BeatTargetTime < limitedConsecutiveBeatTimeWindow)
                {
                    Debug.LogWarning("Skipping beat because there is a limited time window for consecutive beats. UPCOMING BEAT:" + lastTargetBeatQueued.ToString() + "; ATTEMPTED QUEUED BEAT: " + beatData.ToString());
                    return;
                }
            }
        }
        
        // Assign the beat data's target time to the last target beat queued variable
        lastTargetBeatQueued = beatData;
        // Display the beat
        RaiseBeatReceiverQueuedEvent(beatData);
        // Start a coroutine to monitor input for the beat
        StartCoroutine(MonitorBeatForCatching(beatData));
        if (debugBeatsInConsole)
        {
            Debug.Log("QUEUED BEAT " + beatData.ToString());
        }

        if (NextAudibleBeat == null)
        {
            NextAudibleBeat = beatData;
        }
        else
        {
            queuedBeats.Enqueue(NextAudibleBeat);
        }
    }

    /// <summary>
    /// The list of catchable beats
    /// </summary>
    List<BeatSyncData> catchableBeats = new List<BeatSyncData>();

    /// <summary>
    /// Queued beats
    /// </summary>
    private Queue<BeatSyncData> queuedBeats = new Queue<BeatSyncData>();

    /// <summary>
    /// The next audible beat
    /// </summary>
    public BeatSyncData NextAudibleBeat
    {
        private set;
        get;
    }


    /// <summary>
    /// Coroutine used to monitor a beat for catching
    /// </summary>
    /// <param name="beat">The beat to catch</param>
    /// <returns></returns>
    IEnumerator MonitorBeatForCatching(BeatSyncData beat)
    {
        // The starting time when the beat is catchable (target time - max OK time)
        float minCatchTime = beat.BeatTargetTime - maxTimeToLandOKSync;
        // The ending time when the beat is catchable (target time + max OK time)
        float maxCatchTime = beat.BeatTargetTime + maxTimeToLandOKSync;
        // Wait for the beat to be catchable
        while (Time.time < minCatchTime)
        {
            yield return null;
        }
        // Add the beat to the list of catchable beats
        catchableBeats.Add(beat);

        while (Time.time < beat.BeatTargetTime)
        {
            yield return null;
        }

        if (debugBeatsInConsole)
        {
            Debug.Log("BEAT PLAYED " + beat.ToString());
        }

        RaiseBeatPlayedEvent(beat);

        if(NextAudibleBeat == beat)
        {
            if(queuedBeats.Count > 0)
            {
                NextAudibleBeat = queuedBeats.Dequeue();
            }
            else
            {
                NextAudibleBeat = null;
            }
        }

        // Wait until the beat is no longer catchable
        while (Time.time <= maxCatchTime)
        {
            yield return null;
        }
        // If the beat is still in the list of catchable beats, then it's been missed
        // Tell the display beat UI that the beat has been missed
        if (catchableBeats.Contains(beat))
        {
            // Remove the beat from the list of catchable beats
            catchableBeats.Remove(beat);
        }
    }
    

    /// <summary>
    /// Method called to request an input action; returns whether or not the player input action is in sync with a beat.
    /// </summary>
    /// <returns></returns>
    public bool RequestInputAction()
    {
        return UserRequestInputAction() != BeatInputSync.MISS;
    }

    /// <summary>
    /// Method called to request an input action; returns whether or not the player input action is in sync with a beat as well as the beat synchronization type.
    /// </summary>
    /// <param name="inputType">Outputs the input type (if desired)</param>
    /// <returns></returns>
    public bool RequestInputAction(out BeatInputSync inputType)
    {
        inputType = UserRequestInputAction();
        return inputType != BeatInputSync.MISS;
    }

    /// <summary>
    /// Method called to request an input action; returns whether or not the player input action is in sync with a beat as well as the beat synchronization type and combo count
    /// </summary>
    /// <param name="inputType"></param>
    /// <param name="comboCount"></param>
    /// <returns></returns>
    public bool RequestInputAction(out BeatInputSync inputType, out int comboCount)
    {
        inputType = UserRequestInputAction();
        comboCount = this.comboCount;
        return inputType != BeatInputSync.MISS;
    }

    int lastInputActionRequestTimeframe = int.MinValue;
    BeatInputSync lastInputActionRequestResult;

    /// <summary>
    /// The last beat sync the player hit
    /// </summary>
    public BeatSyncData LastBeatHit
    {
        private set;
        get;
    } = null;

    /// <summary>
    /// The input sync type of the last beat the player hit
    /// </summary>
    public BeatInputSync LastBeatHitSyncType
    {
        private set;
        get;
    } = BeatInputSync.MISS;

    /// <summary>
    /// The maximum amount of time allowed to hit a beat (INCLUSIVE SECONDS NOT EXCLUSIVE)
    /// </summary>
    public float MaxTimeToHitABeat
    {
        get
        {
            return maxTimeToLandOKSync;
        }
    }

    /// <summary>
    /// Method called when the user requests an input action, returns the input beat synchronization type
    /// </summary>
    /// <returns></returns>
    private BeatInputSync UserRequestInputAction()
    {
        // If the user already requested an action in the same frame, then return the last hit sync type
        if (Time.frameCount == lastInputActionRequestTimeframe) return lastInputActionRequestResult;
        // Cache the closest beat to the user's input
        BeatSyncData hitBeat = null;
        // Cache the hit beat target time difference
        float hitBeatTimeDifference = float.MaxValue;
        // Iterate through our list of catchable beats
        foreach (BeatSyncData catchableBeat in catchableBeats)
        {
            // Cache the time difference between the time and the catchable beat's target time
            float catchableBeatTimeDifference = Mathf.Abs(Time.time - catchableBeat.BeatTargetTime);
            // If the beat to hit is null, then assign this catchable beat to it
            if (hitBeat == null)
            {
                hitBeat = catchableBeat;
                hitBeatTimeDifference = catchableBeatTimeDifference;
                continue;
            }
            // Otherwise, if this catchable beat's target time difference is less than the previous beat to hit
            // Then hit this beat
            else if(catchableBeatTimeDifference < hitBeatTimeDifference)
            {
                hitBeat = catchableBeat;
                hitBeatTimeDifference = catchableBeatTimeDifference;
            }
        }

        // If no beat was hit, then the user didn't hit a beat
        if(hitBeat == null)
        {
            lastInputActionRequestTimeframe = Time.frameCount;
            lastInputActionRequestResult = BeatInputSync.MISS;
            // Reset the combo count
            ResetComboCount();
            // Display the proper state that the played completely missed the a beat
            RaisePlayerInputEvent(null, BeatInputSync.MISS);
            // Return the beat missed sync type
            return BeatInputSync.MISS;
        }
        // If a beat is hit, then remove it from our catchable beats list and return
        // the input sync type
        else
        {
            lastInputActionRequestTimeframe = Time.frameCount;
            // Remove the hit beat from our beats to catch list
            catchableBeats.Remove(hitBeat);
            
            // Increment the combo count
            IncrementComboCount();
            // Get the sync type based on the time difference between the beat's target time and the input
            BeatInputSync syncType;
            if(hitBeatTimeDifference <= maxTimeToLandPerfectSync)
            {
                syncType = BeatInputSync.PERFECT;
            }
            else if(hitBeatTimeDifference <= maxTimeToLandGoodSync)
            {
                syncType = BeatInputSync.GOOD;
            }
            else
            {
                syncType = BeatInputSync.OK;
            }
            lastInputActionRequestResult = syncType;
            RaisePlayerInputEvent(hitBeat, syncType);
            LastBeatHit = hitBeat;
            LastBeatHitSyncType = syncType;
            return syncType;
        }
    }

    /// <summary>
    /// Method called to reset the count combo number
    /// </summary>
    /// <param name="newCount"></param>
    private void ResetComboCount()
    {
        comboCount = 0;
        RaiseComboCountChangedEvent();
    }


    /// <summary>
    /// Method called to increment the combo count
    /// </summary>
    private void IncrementComboCount()
    {
        comboCount++;
        RaiseComboCountChangedEvent();
    }
    

    /// <summary>
    /// Coroutine used to debug the beat receiver to the player's input
    /// </summary>
    /// <returns></returns>
    IEnumerator DebugReceiverCoroutine()
    {
        while (true)
        {
            // If the player is pressing the debug input button, then request an input action
            if (Input.GetButtonDown(debugInputName))
            {
                RequestInputAction();
            }
            yield return null;
        }
    }

    /// <summary>
    /// Event raised on the first frame the player is warned about a beat
    /// </summary>
    public event BeatQueued QueuedBeatToSync;

    /// <summary>
    /// Event raised on the first frame a beat is audible / played
    /// </summary>
    public event BeatPlayed PlayedBeatToSync;

    /// <summary>
    /// Event raised when the player input is received and synchronized. Note: Your delegate should be able to handle null BeatSyncData in the event the player tries an action when no beat is available
    /// </summary>
    public event BeatInputSynced PlayerInputSynced;

    /// <summary>
    /// Raises the player sync input received event
    /// </summary>
    /// <param name="beat">The beat sync data</param>
    /// <param name="beatSyncType">The beat synchronization type</param>
    private void RaisePlayerInputEvent(BeatSyncData beat, BeatInputSync beatSyncType)
    {
        if(PlayerInputSynced != null)
        {
            PlayerInputSynced.Invoke(beat, beatSyncType);
        }
    }

    /// <summary>
    /// Method called raise the beat queued event
    /// </summary>
    /// <param name="beatQueued"></param>
    private void RaiseBeatReceiverQueuedEvent(BeatSyncData beatQueued)
    {
        if(QueuedBeatToSync != null)
        {
            try
            {
                QueuedBeatToSync.Invoke(beatQueued);
            }
            catch(System.Exception e)
            {
                Debug.LogError(e.Message);
            }
            
        }
    }

    /// <summary>
    /// Method called to raise the beat played event
    /// </summary>
    /// <param name="beatPlayed"></param>
    private void RaiseBeatPlayedEvent(BeatSyncData beatPlayed)
    {
        if(PlayedBeatToSync != null)
        {
            try
            {
                PlayedBeatToSync.Invoke(beatPlayed);
            }
            catch(System.Exception e)
            {
                Debug.LogError(e.Message);
            }
            
        }
    }

    /// <summary>
    /// Event raised when the combo count is changed
    /// </summary>
    public event ComboCountChange OnComboCountChanged;

    /// <summary>
    /// Method called to raise the combo count changed e
    /// </summary>
    private void RaiseComboCountChangedEvent()
    {
        if(OnComboCountChanged != null)
        {
            OnComboCountChanged.Invoke(ComboCount);
        }
    }


}
/// <summary>
/// Delegate used for handling beat queueing (this is when the player is informed about a beat to respond to)
/// </summary>
/// <param name="beatQueued">The beat queued</param>
public delegate void BeatQueued (BeatSyncData beatQueued);

/// <summary>
/// Delegate used for reading combo count changes
/// </summary>
/// <param name="comboCount">The combo count</param>
public delegate void ComboCountChange(int comboCount);

/// <summary>
/// Delegate used for handling beat playing (this is the first frame when a beat is audible / played)
/// </summary>
/// <param name="beatPlayed">The beat data played</param>
public delegate void BeatPlayed(BeatSyncData beatPlayed);


public delegate void BeatInputSynced(BeatSyncData beatData, BeatInputSync syncType);

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
    /// The song timestamp of this beat within the beatmap
    /// </summary>
    public double BeatMapTimestamp
    {
        private set;
        get;
    }

    /// <summary>
    /// The index of this beat within the beatmap timestamps list
    /// </summary>
    public int BeatMapTimestampIndex
    {
        private set;
        get;
    }

    public bool UsingBPM
    {
        private set;
        get;
    }

    /// <summary>
    /// Method called to construct a beat sync data class (Note: Creation timestamps are cached upon construction)
    /// </summary>
    /// <param name="beatTargetTimeAudioDSP"></param>
    public BeatSyncData(double beatTargetTimeAudioDSP, double listedBeatTimestamp, int listedBeatIndex, bool isBPM)
    {
        // Cache the beat target dsp time
        this.BeatTargetDSPTime = beatTargetTimeAudioDSP;
        // Cache the beat creation time (dsp time)
        this.BeatCreationAudioDSPScale = AudioSettings.dspTime;
        // Cache the beat creation time (regular time)
        this.BeatCreationTimeScale = Time.time;
        // Cache the beat target time (regular time) 
        // This is the current time + (seconds until the beat target time)
        BeatTargetTime = BeatCreationTimeScale + (float) (BeatTargetDSPTime - BeatCreationAudioDSPScale);
        BeatMapTimestamp = listedBeatTimestamp;
        BeatMapTimestampIndex = listedBeatIndex;
        // Assign the is BPM variable
        this.UsingBPM = isBPM;
    }

    /// <summary>
    /// Returns a string with this beatmap's timestamp value, timestamp list index, and BPM generated info
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return "Beatmap Timestamp = " + BeatMapTimestamp + "; Beatmap Index = " + BeatMapTimestampIndex + "; BPM Generated = " + UsingBPM.ToString();
    }

    /// <summary>
    /// Logs this beat sync data into the console
    /// </summary>
    public void DebugBeatInConsole()
    {
        Debug.Log(this.ToString());
    }
}

public enum BeatInputSync { PERFECT, GOOD, OK, MISS};