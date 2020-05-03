using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Beat Map", menuName ="Beat Map")]
public class BeatMap : ScriptableObject
{
    [Header("Track Data")]
    [Tooltip("The audio track")]
    [SerializeField] AudioClip musicTrack;
    [Tooltip("The audio track beat timestamps in seconds")]
    [SerializeField] List<float> beatTimestamps;
    [Tooltip("The audio track's beats per minute")]
    [SerializeField] float BPM;
    [Header("Loop Settings")]
    [Tooltip("Whether or not this audio track should use a custom starting timestamp to loop")]
    [SerializeField] bool useCustomStartLoopTimestamp = false;
    [Tooltip("The timestamp in seconds of the song where it should begin looping")]
    [SerializeField] float customLoopableStartTimestamp = 0.0f;
    [Tooltip("Whether or not this audio track should use a custom ending timestamp to loop")]
    [SerializeField] bool useCustomEndLoopTimestamp = false;
    [Tooltip("The timestamp of the song where it should end looping")]
    [SerializeField] float customLoopableEndTimestamp = 0.0f;

    /// <summary>
    /// The music track
    /// </summary>
    public AudioClip MusicTrack
    {
        get { return musicTrack; }
    }

    /// <summary>
    /// Returns the beat map timestamps
    /// </summary>
    public List<float> BeatTimeStamps
    {
        get { return beatTimestamps; }
    }

    /// <summary>
    /// Returns the beats per minute
    /// </summary>
    public float BeatsPerMinute
    {
        get
        {
            return BPM;
        }
    }

    /// <summary>
    /// The music track's duration
    /// </summary>
    public float TrackDuration
    {
        get
        {
            return MusicTrack.length;
        }
    }

    /// <summary>
    /// The custom loopable starting timestamp 
    /// </summary>
    public float CustomLoopableStartTimestamp
    {
        get
        {
            if (customLoopableStartTimestamp < 0)
            {
                return 0;
            }
            else
            {
                return customLoopableStartTimestamp;
            }
        }
    }

    /// <summary>
    /// Returns whether or not this audio track has a custom loopable begining
    /// </summary>
    public bool UseCustomLoopStartTimestamp
    {
        get { return useCustomStartLoopTimestamp; }
    }

    /// <summary>
    /// Returns whether or not this audio track has a custom loopable end
    /// </summary>
    public bool UseCustomLoopEndTimestamp
    {
        get { return useCustomEndLoopTimestamp; }
    }

    /// <summary>
    /// The custom loopable ending timestamp 
    /// </summary>
    public float CustomLoopableEndTimestamp
    {
        get
        {
            if(customLoopableEndTimestamp > musicTrack.length)
            {
                return musicTrack.length;
            }
            else
            {
                return customLoopableEndTimestamp;
            }

        }
    }

}
