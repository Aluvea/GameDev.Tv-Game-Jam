using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Beat map player used to transition between tracks
/// </summary>
public class BeatMapPlayerManager : MonoBehaviour
{
    [Tooltip("AudioSource A")]
    [SerializeField] BeatMapPlayer trackA;
    [SerializeField] BeatMapPlayer trackB;

    [Tooltip("How many seconds a beat is previewed to the player before it's audible")]
    [Range(0.0f, 10.0f)]
    [SerializeField] double beatMapPreviewTime = 3.0f;
    [Tooltip("How much time in seconds an audio track shoud take before transitioning into the next song")]
    [SerializeField] float transitionFadeDuration = 3.0f;
    [Header("BPM Settings")]
    [Tooltip("Whether or not BeatMapPlayers should use BPM lists instead of beatmap timestamps")]
    [SerializeField] bool beatMapPlayersShouldPlayBPMList = false;
    [Tooltip("Whether or not the BPM should be used as the beat preview time")]
    [SerializeField] bool useBPMAsPreviewTime = false;
    [Header("Testing Parameters")]
    [SerializeField] bool debugPlayer = false;
    [SerializeField] BeatMap beatMapToTest;


    private void Awake()
    {

        if (useBPMAsPreviewTime)
        {
            beatMapPreviewTime = 60.0d / beatMapToTest.BeatsPerMinute;
        }

        trackA.SetBeatMapPlayer(this);
        trackB.SetBeatMapPlayer(this);
    }

    private void Start()
    {
        if (debugPlayer)
        {
            trackA.PlayBeatMapTrack(beatMapToTest, AudioSettings.dspTime + beatMapPreviewTime + beatMapPreviewTime, true);
        }
    }
    
    /// <summary>
    /// Amount of seconds that a beat previewed to the palyer before it's audible (warning time)
    /// </summary>
    public double BeatMapPreviewTime
    {
        get { return beatMapPreviewTime; }
    }

    /// <summary>
    /// Use beat per minute
    /// </summary>
    public bool UseBeatPerMinute
    {
        get
        {
            return beatMapPlayersShouldPlayBPMList;
        }
    }

}




