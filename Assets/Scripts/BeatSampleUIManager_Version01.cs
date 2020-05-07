using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatSampleUIManager_Version01 : BeatSampleUIManager
{
    [SerializeField] BeatSampleUI beatSamplePrefab;
    
    

    [SerializeField] Transform beatUIParent;
    [SerializeField] Transform startingBeatUIPosition;
    [SerializeField] Transform endingBeatUIPosition;

    [Header("Beat Sync Color Settings")]
    [SerializeField] Color startingInnerCircleColor;
    [SerializeField] Color startingOuterCircleColor;
    [SerializeField] Color perfectInnerCircleColor;
    [SerializeField] Color perfectOuterCircleColor;
    [SerializeField] Color goodInnerCircleColor;
    [SerializeField] Color goodOuterCircleColor;
    [SerializeField] Color okInnerCircleColor;
    [SerializeField] Color okOuterCircleColor;
    [SerializeField] Color missedInnerCircleColor;
    [SerializeField] Color missedOuterCircleColor;

    private static BeatSampleUIManager_Version01 CurrenBeatSampleUIManager
    {
        set;
        get;
    }

    /// <summary>
    /// Object pool queue of beat sample UIs (ready to play a beat)
    /// </summary>
    Queue<BeatSampleUI> beatSampleObjectPool = new Queue<BeatSampleUI>();

    /// <summary>
    /// List of currently playing beat sample UIs
    /// </summary>
    List<BeatSampleUI> playingBeatSamplePool = new List<BeatSampleUI>();

    private void Awake()
    {
        CurrenBeatSampleUIManager = this;
    }

    /// <summary>
    /// Called to reset a beat sample UI
    /// </summary>
    /// <param name="beatSampleUI"></param>
    public static void OnBeatSampleUIAnimationEnded(BeatSampleUI beatSampleUI)
    {
        CurrenBeatSampleUIManager.ResetBeatInstance(beatSampleUI);
    }

    private void ResetBeatInstance(BeatSampleUI beatSample)
    {
        // Remove the beat sample from the playing beat sample list
        playingBeatSamplePool.Remove(beatSample);
        // Disable the game object
        beatSample.gameObject.SetActive(false);
        // Reset its position
        beatSample.transform.position = startingBeatUIPosition.position;
        // Restart its color
        beatSample.SetBeatSampleColor(startingInnerCircleColor,startingOuterCircleColor);
        // Queue it to our object pool of beat sample UIs
        beatSampleObjectPool.Enqueue(beatSample);
    }
    

    public override void DisplayBeat(BeatSyncData beatSyncData)
    {
        // If our object pool has a beat sample ready to be played, then dequeue a beat sample and use it
        if(beatSampleObjectPool.Count > 0)
        {
            BeatSampleUI dequeuedBeatSample = beatSampleObjectPool.Dequeue();
            dequeuedBeatSample.AnimateBeatData(beatSyncData, endingBeatUIPosition.position);
            playingBeatSamplePool.Add(dequeuedBeatSample);
            return;
        }
        // Otherwise, instantiate a beat sample UI
        BeatSampleUI beatSample = Instantiate(beatSamplePrefab, startingBeatUIPosition.position, Quaternion.identity, beatUIParent);
        // Set its starting color
        beatSample.SetBeatSampleColor(startingInnerCircleColor, startingOuterCircleColor);
        // Animate the beat beat sync data towards the ending beat ui position
        beatSample.AnimateBeatData(beatSyncData, endingBeatUIPosition.position);
        // Add the beat sample to our currently playing beat sample pool
        playingBeatSamplePool.Add(beatSample);
    }

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
        foreach (BeatSampleUI playingSample in playingBeatSamplePool)
        {
            // If the beat sample matches the beat hit, then update that beat samples UI state
            if (playingSample.PlayingBeatSync == beatHit)
            {
                switch (beatInput)
                {
                    case BeatInputSync.PERFECT:
                        playingSample.SetBeatSampleColor(perfectInnerCircleColor, perfectOuterCircleColor);
                        break;
                    case BeatInputSync.GOOD:
                        playingSample.SetBeatSampleColor(goodInnerCircleColor, goodOuterCircleColor);
                        break;
                    case BeatInputSync.OK:
                        playingSample.SetBeatSampleColor(okInnerCircleColor, okOuterCircleColor);
                        break;
                    case BeatInputSync.MISS:
                        playingSample.SetBeatSampleColor(missedInnerCircleColor, missedOuterCircleColor);
                        break;
                }
                return;
            }
        }
    }
}
