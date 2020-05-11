using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatSampleUIManager_Version05 : BeatSampleUIManager
{
    [SerializeField] BeatSampleUI_version4 beatSamplePrefab;
    [SerializeField] Transform beatUIParent;
    [SerializeField] Transform startingPositionReference;

    [Header("Beat Sync Color Settings")]
    [SerializeField] Color startincCircleColor;
    [SerializeField] Color perfectCircleColor;
    [SerializeField] Color goodCircleColor;
    [SerializeField] Color okCircleColor;
    [SerializeField] Color missedCircleColor;

    private static BeatSampleUIManager_Version05 CurrenBeatSampleUIManager
    {
        set;
        get;
    }

    /// <summary>
    /// Object pool queue of beat sample UIs (ready to play a beat)
    /// </summary>
    Queue<BeatSampleUI_version4> beatSampleObjectPool = new Queue<BeatSampleUI_version4>();

    /// <summary>
    /// List of currently playing beat sample UIs
    /// </summary>
    List<BeatSampleUI_version4> playingBeatSamplePool = new List<BeatSampleUI_version4>();

    private void Awake()
    {
        CurrenBeatSampleUIManager = this;
    }

    /// <summary>
    /// Called to reset a beat sample UI
    /// </summary>
    /// <param name="beatSampleUI"></param>
    public static void OnBeatSampleUIAnimationEnded(BeatSampleUI_version4 beatSampleUI)
    {
        CurrenBeatSampleUIManager.ResetBeatInstance(beatSampleUI);
    }

    private void ResetBeatInstance(BeatSampleUI_version4 beatSample)
    {
        // Remove the beat sample from the playing beat sample list
        playingBeatSamplePool.Remove(beatSample);
        // Disable the game object
        beatSample.gameObject.SetActive(false);
        // Reset its position
        beatSample.transform.position = startingPositionReference.position;
        // Restart its color
        beatSample.SetBeatSampleColor(startincCircleColor);
        // Queue it to our object pool of beat sample UIs
        beatSampleObjectPool.Enqueue(beatSample);
    }
    

    public override void DisplayBeat(BeatSyncData beatSyncData)
    {
        // If our object pool has a beat sample ready to be played, then dequeue a beat sample and use it
        if(beatSampleObjectPool.Count > 0)
        {
            BeatSampleUI_version4 dequeuedBeatSample = beatSampleObjectPool.Dequeue();
            dequeuedBeatSample.AnimateBeatData(beatSyncData);
            playingBeatSamplePool.Add(dequeuedBeatSample);
            return;
        }
        // Otherwise, instantiate a beat sample UI
        BeatSampleUI_version4 beatSample = Instantiate(beatSamplePrefab, startingPositionReference.position, Quaternion.identity, beatUIParent);
        // Set its starting color
        beatSample.SetBeatSampleColor(startincCircleColor);
        // Animate the beat beat sync data towards the ending beat ui position
        beatSample.AnimateBeatData(beatSyncData);
        // Add the beat sample to our currently playing beat sample pool
        playingBeatSamplePool.Add(beatSample);
    }

    private bool direction = false;
    
    /// <summary>
    /// Displays a compltely missed beat
    /// </summary>
    public override void DisplayInputMissedBeat()
    {
        Debug.Log("Completely missed beat");
    }
    
    /// <summary>
    /// Updates a beat sample UI to a given hit state
    /// </summary>
    /// <param name="beatHit"></param>
    /// <param name="beatInput"></param>
    public override void UpdateBeatSyncUIState(BeatSyncData beatHit, BeatInputSync beatInput)
    {
        Debug.Log("Beat " + beatInput.ToString());
        // Iterate through our list of beat samples playing
        foreach (BeatSampleUI_version4 playingSample in playingBeatSamplePool)
        {
            // If the beat sample matches the beat hit, then update that beat samples UI state
            if (playingSample.PlayingBeatSync == beatHit)
            {
                switch (beatInput)
                {
                    case BeatInputSync.PERFECT:
                        playingSample.SetBeatSampleColor(perfectCircleColor);
                        break;
                    case BeatInputSync.GOOD:
                        playingSample.SetBeatSampleColor(goodCircleColor);
                        break;
                    case BeatInputSync.OK:
                        playingSample.SetBeatSampleColor(okCircleColor);
                        break;
                    case BeatInputSync.MISS:
                        playingSample.SetBeatSampleColor(missedCircleColor);
                        break;
                }
                return;
            }
        }
    }
}
