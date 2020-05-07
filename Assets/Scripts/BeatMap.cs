using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Beat Map", menuName ="Beat Map")]
public class BeatMap : ScriptableObject
{
    [Header("Track Data")]
    [Tooltip("The audio track")]
    [SerializeField] AudioClip musicTrack;

    [Header("Loop / Trimming Settings")]
    [Tooltip("Whether or not the song's loop should be trimmed from an intro clip")]
    [SerializeField] bool trimIntroFromLoop;
    [Tooltip("The timestamp of the end of the intro / beginning of the loop")]
    [SerializeField] double introEndTimestamp = 0;
    [Tooltip("Whether or not the song's loop should be trimmed from an outro clip")]
    [SerializeField] bool trimOutroFromLoop;
    [Tooltip("The timestamp of the start of the outro / end of the loop")]
    [SerializeField] double outroStartTimestamp = 0;
    [Tooltip("Whether or not the trimmed audio clips should be saved to the disc (Intro / Loop / Outro). We should use pre-trimmed audio subclips prior to releasing our game instead of relying on trimming audio clips in during runtime. Leave this unchecked while we're in development though.")]
    [SerializeField] bool saveTrimmedAudioClipsToDisc = false;
    [Header("Beat Timestamps")]
    [Tooltip("The audio track beat timestamps in seconds")]
    [SerializeField] List<double> beatTimestamps;

    [Header("BPM Setting Data")]
    [Tooltip("The audio track's beats per minute (BPM)")]
    [SerializeField] double BPM = 60.0d;
    [Tooltip("The first beat timestamp ")]
    [SerializeField] double firstBeatTimestamp = 0.0d;
    [Tooltip("Whether or not the BPM beatmap should be limited to a last beat timestamp")]
    [SerializeField] bool useLastBeatTimestamp = false;
    [Tooltip("The last beat timestamp")]
    [SerializeField] double lastBeatTimestamp;
    

    /// <summary>
    /// The intro music track
    /// </summary>
    public AudioClip IntroTrack
    {
        private set;
        get;
    } = null;

    /// <summary>
    /// The loopable music track
    /// </summary>
    public AudioClip LoopableTrack
    {
        private set;
        get;
    } = null;

    /// <summary>
    /// The outro music track
    /// </summary>
    public AudioClip OutroTrack
    {
        private set;
        get;
    } = null;

    /// <summary>
    /// Returns the beat map timestamps (from a list of preset timestamps)
    /// </summary>
    public List<double> BeatTimeStamps
    {
        get { return beatTimestamps; }
    }

    /// <summary>
    /// Returns the timestamps from BPM settings
    /// </summary>
    public List<double> BPMBeatTimestamps
    {
        get
        {
            if (BPM <= 0) throw new System.Exception("BPM BEAT TIMESTAMPS ATTEMPTED TO BE ACCESSED BUT BPMS ARE SET TO 0 IN BEATMAP FOR TRACK " + musicTrack.name);
            // create a list of double timestamps
            List<double> bpmBeats = new List<double>();
            // Cache a variable for the last recorded timestamp
            double lastRecordedTimestamp = firstBeatTimestamp;
            // Add the first beat timestamp
            bpmBeats.Add(lastRecordedTimestamp);
            // Set a timestamp to record beats up to
            double recordUpToTimestamp = GetAudioClipDuration(musicTrack);
            // If we're supposed to use the last beat time stamp, and 
            // the last beat timestamp is less than the duration of the actual music track
            // then set the record up to timestamp to the last beat timestamp
            if(useLastBeatTimestamp && recordUpToTimestamp > lastBeatTimestamp + 0.01d)
            {
                recordUpToTimestamp = lastBeatTimestamp + 0.01d;
            }

            // Get the beat interval (60 seconds / BPM)
            double beatInterval = 60.0d / BPM;

            // While the last recorded time stamp is less than the recordUptoTimestamp,
            // Keep adding the beat interval to the timestamp
            while (lastRecordedTimestamp < recordUpToTimestamp)
            {
                lastRecordedTimestamp += beatInterval;
                bpmBeats.Add(lastRecordedTimestamp);
            }

            return bpmBeats;
        }
    }

    /// <summary>
    /// The beats per minute of this song
    /// </summary>
    public double BeatsPerMinute
    {
        get
        {
            return BPM;
        }
    }

    /// <summary>
    /// The loopable music track's duration
    /// </summary>
    public double LoopTrackDuration
    {
        get
        {
            return GetAudioClipDuration(LoopableTrack);
        }
    }

    /// <summary>
    /// The intro music track's duration
    /// </summary>
    public double IntroTrackDuration
    {
        get
        {
            if (trimIntroFromLoop == false) return 0;
            return GetAudioClipDuration(IntroTrack);
        }
    }

    /// <summary>
    /// The outro music tracks duration
    /// </summary>
    public double OutroTrackDuration
    {
        get
        {
            if (trimOutroFromLoop == false) return 0;
            return GetAudioClipDuration(OutroTrack);
        }
    }

    /// <summary>
    /// Returns whether or not this audio track has a custom intro track
    /// </summary>
    public bool CustomIntroTrackExists
    {
        get { return IntroTrack != null; }
    }

    /// <summary>
    /// Returns whether or not this audio track has a custom loopable end
    /// </summary>
    public bool CustomOutroTrackExists
    {
        get { return OutroTrack != null; }
    }

    /// <summary>
    /// Generates intro, loopable, and outro tracks based on the settings of this beatmap
    /// </summary>
    public void GenerateTracks()
    {
        // If no intro and outro should be used, then simply assign the music track to the loopable track
        if(trimIntroFromLoop == false && trimOutroFromLoop == false)
        {
            LoopableTrack = musicTrack;
        }
        // If an intro should be used and no outro should be used,
        // Then do respective track trimming
        else if (trimIntroFromLoop == true && trimOutroFromLoop == false)
        {
            // Get the introduction subclip from the beginning of the track to the intro end
            IntroTrack = CreateSubClip(musicTrack, "intro", 0.0f, (float) introEndTimestamp);
            // Get the loopable subclip from the intro end to the end of the song
            LoopableTrack = CreateSubClip(musicTrack, "trueLoop", (float)introEndTimestamp, (float) GetAudioClipDuration(musicTrack));
            
            if (saveTrimmedAudioClipsToDisc)
            {
                // Save the intro subclip
                SaveWave.Save(musicTrack.name + "- Intro Track", IntroTrack);
                // Save the loopable subclip
                SaveWave.Save(musicTrack.name + " - Loop Track", LoopableTrack);
            }
        }
        // If an intro and outro should be used, 
        // Then do respective track trimming
        else if (trimIntroFromLoop == true && trimOutroFromLoop == true)
        {

            // Get the introduction subclip from the beginning of the track to the intro end
            IntroTrack = CreateSubClip(musicTrack, "intro", 0.0f, (float)introEndTimestamp);
            // Get the loopable subclip from the intro end to the outro start of the song
            LoopableTrack = CreateSubClip(musicTrack, "trueLoop", (float)introEndTimestamp, (float)outroStartTimestamp);
            // Get the outro subclip from the from the outro start to the end of the song
            OutroTrack = CreateSubClip(musicTrack, "outro", (float)outroStartTimestamp, (float)GetAudioClipDuration(musicTrack));
            if (saveTrimmedAudioClipsToDisc)
            {
                // Save the intro subclip
                SaveWave.Save(musicTrack.name + "- Intro Track", IntroTrack);
                // Save the loopable subclip
                SaveWave.Save(musicTrack.name + " - Loop Track", LoopableTrack);
                // Save the outro subclip
                SaveWave.Save(musicTrack.name + " - Outro Track", OutroTrack);
            }
        }
        else if(trimIntroFromLoop == false && trimOutroFromLoop == true)
        {
            // Get the loopable subclip from the start of the song to the outro start of the song
            LoopableTrack = CreateSubClip(musicTrack, "trueLoop", 0, (float)outroStartTimestamp);
            // Get the outro subclip from the from the outro start to the end of the song
            OutroTrack = CreateSubClip(musicTrack, "outro", (float)outroStartTimestamp, (float)GetAudioClipDuration(musicTrack));

            if (saveTrimmedAudioClipsToDisc)
            {
                // Save the loopable subclip
                SaveWave.Save(musicTrack.name + " - Loop Track", LoopableTrack);
                // Save the outro subclip
                SaveWave.Save(musicTrack.name + " - Outro Track", OutroTrack);
            }
        }

    }

    /// <summary>
    /// Returns the precise dureation of an audio clip
    /// </summary>
    /// <param name="clip">The audio clip</param>
    /// <returns></returns>
    private static double GetAudioClipDuration(AudioClip clip)
    {
        if (clip == null) return 0;
        return (double) clip.samples / clip.frequency;
    }

    /**
 * Creates a sub clip from an audio clip based off of the start time
 * and the stop time. The new clip will have the same frequency as
 * the original.
 */
    private AudioClip CreateSubClip(AudioClip clip, string clipName, float start, float stop)
    {
        // Reference
        // http://answers.unity.com/answers/1383912/view.html
        /* Create a new audio clip */
        // Get the frequency of the audio clip
        int frequency = clip.frequency;
        // Get the time length of our audio clip
        float timeLength = stop - start;
        // Get the count of samples by multiplying the frequency and the time length
        int samplesLength = (int)(frequency * timeLength);
        // Create a new audio clip for our subclip
        AudioClip newClip = AudioClip.Create(clip.name + clipName, samplesLength, clip.channels, frequency, false);
        /* Create a temporary buffer for the samples */ 
        float[] data = new float[samplesLength * clip.channels];
        /* Get the data from the original clip */
        clip.GetData(data, (int)(frequency * start));
        /* Transfer the data to the new clip */
        newClip.SetData(data, 0);
        DebugAudioClipData(clip);
        DebugAudioClipData(newClip);
        newClip.LoadAudioData();
        /* Return the sub clip */
        return newClip;
    }

    private void DebugAudioClipData(AudioClip clip)
    {
        Debug.Log("clip: " + clip.name + "; Frequency = " + clip.frequency + "; samples = " + clip.samples + "; Length = "+clip.length);
    }
}
