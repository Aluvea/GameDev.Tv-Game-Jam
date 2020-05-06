using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatSampleUIManager_Version01 : BeatSampleUIManager
{
    [SerializeField] BeatSampleUI beatSamplePrefab;
    [SerializeField] Color perfectColor;
    [SerializeField] Color goodColor;
    [SerializeField] Color okColor;
    [SerializeField] Transform beatUIParent;
    [SerializeField] Transform startingBeatUIPosition;
    [SerializeField] Transform endingBeatUIPosition;

    private static BeatSampleUIManager_Version01 CurrenBeatSampleUIManager
    {
        set;
        get;
    }

    List<BeatSampleUI> beatSampleObjectPool = new List<BeatSampleUI>();

    private void Awake()
    {
        CurrenBeatSampleUIManager = this;
    }

    public static void OnBeatSampleUIAnimationEnded(BeatSampleUI beatSampleUI)
    {
        CurrenBeatSampleUIManager.ResetBeatInstance(beatSampleUI);
    }

    private void ResetBeatInstance(BeatSampleUI beatSample)
    {
        beatSample.gameObject.SetActive(false);
        beatSample.transform.position = startingBeatUIPosition.position;
    }
    

    public override void DisplayBeat(BeatSyncData beatSyncData)
    {
        foreach (BeatSampleUI pooledSample in beatSampleObjectPool)
        {
            if(pooledSample.PlayingBeat == false)
            {
                pooledSample.AnimateBeatData(beatSyncData, endingBeatUIPosition.position);
                return;
            }
        }

        BeatSampleUI beatSample = Instantiate(beatSamplePrefab, startingBeatUIPosition.position, Quaternion.identity, beatUIParent);
        beatSample.AnimateBeatData(beatSyncData, endingBeatUIPosition.position);
        beatSampleObjectPool.Add(beatSample);
    }
}
