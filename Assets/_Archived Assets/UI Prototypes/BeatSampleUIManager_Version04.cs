using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatSampleUIManager_Version04 : BeatSampleUIManager
{
    [SerializeField] BeatSampleUI_version3 beatSamplePrefab;
    
    

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

    private static BeatSampleUIManager_Version04 CurrenBeatSampleUIManager
    {
        set;
        get;
    }

    /// <summary>
    /// Object pool queue of beat sample UIs (ready to play a beat)
    /// </summary>
    Queue<BeatSampleUI_version3> beatSampleObjectPool = new Queue<BeatSampleUI_version3>();

    /// <summary>
    /// List of currently playing beat sample UIs
    /// </summary>
    List<BeatSampleUI_version3> playingBeatSamplePool = new List<BeatSampleUI_version3>();

    private void Awake()
    {
        CurrenBeatSampleUIManager = this;
    }

    /// <summary>
    /// Called to reset a beat sample UI
    /// </summary>
    /// <param name="beatSampleUI"></param>
    public static void OnBeatSampleUIAnimationEnded(BeatSampleUI_version3 beatSampleUI)
    {
        CurrenBeatSampleUIManager.ResetBeatInstance(beatSampleUI);
    }

    private void ResetBeatInstance(BeatSampleUI_version3 beatSample)
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
            BeatSampleUI_version3 dequeuedBeatSample = beatSampleObjectPool.Dequeue();
            dequeuedBeatSample.AnimateBeatData(beatSyncData, endingBeatUIPosition.position, GetAlternatingDirection());
            playingBeatSamplePool.Add(dequeuedBeatSample);
            return;
        }
        // Otherwise, instantiate a beat sample UI
        BeatSampleUI_version3 beatSample = Instantiate(beatSamplePrefab, startingBeatUIPosition.position, Quaternion.identity, beatUIParent);
        // Set its starting color
        beatSample.SetBeatSampleColor(startingInnerCircleColor, startingOuterCircleColor);
        // Animate the beat beat sync data towards the ending beat ui position
        beatSample.AnimateBeatData(beatSyncData, endingBeatUIPosition.position, GetAlternatingDirection());
        // Add the beat sample to our currently playing beat sample pool
        playingBeatSamplePool.Add(beatSample);
    }

    private bool direction = false;

    private Vector3 GetAlternatingDirection()
    {
        if (direction)
        {
            direction = !direction;
            return Vector3.right;
        }
        else
        {
            direction = !direction;
            return Vector3.left;
        }
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
        foreach (BeatSampleUI_version3 playingSample in playingBeatSamplePool)
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
