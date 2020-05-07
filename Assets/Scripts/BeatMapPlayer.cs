using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class used to play a beatmap audiotrack and generate beat syncs for it
/// </summary>
public class BeatMapPlayer : MonoBehaviour
{
    [SerializeField] UnityEngine.Audio.AudioMixer audioMixerChannel;
    [SerializeField] string mixerAttenuationParameterName;

    [SerializeField] AudioSource introAudiosource;
    [SerializeField] AudioSource loopingAudiosource;
    [SerializeField] AudioSource outroAudiosource;

    BeatMap currentBeatMap = null;
    private bool loopTrack = false;

    /// <summary>
    /// How early beat map syncs should be generated before they are audible
    /// </summary>
    private double beatMapPreviewTime;

    /// <summary>
    /// Whether or not the BPM should be used on a given track instead of the beat map timestamps
    /// </summary>
    private bool useBPM;

    /// <summary>
    /// Plays a beatmap audio track
    /// </summary>
    /// <param name="beatMapToPlay">The beatmap to play</param>
    /// <param name="scheduledPlaybacktime">The scheduled audiotime when this beatmap should be played (In AudioSettings.dpsTime)</param>
    /// <param name="loopTrack">Whether or not the track should loop (Loops by default)</param>
    public void PlayBeatMapTrack(BeatMap beatMapToPlay, double scheduledPlaybacktime, bool loopTrack = true)
    {
        // Generate the beatmap's audio tracks
        beatMapToPlay.GenerateTracks();
        // Cache the currently playing beatmap
        currentBeatMap = beatMapToPlay;
        // Assign the loop track variable
        this.loopTrack = loopTrack;
        // The looping audiosource track should be set to the loop value here
        loopingAudiosource.loop = loopTrack;
        // Start looping through the beat map
        LoopSong(scheduledPlaybacktime);
    }

    /// <summary>
    /// Loops the current beat map song at the scheduled time
    /// </summary>
    /// <param name="scheduledPlaybacktime"></param>
    void LoopSong(double scheduledPlaybacktime)
    {
        // Assign the respective clips to the given audiosources
        introAudiosource.clip = currentBeatMap.IntroTrack;
        loopingAudiosource.clip = currentBeatMap.LoopableTrack;
        outroAudiosource.clip = currentBeatMap.OutroTrack;

        // If an intro track exists, play it first followed by the loopable track
        if (currentBeatMap.CustomIntroTrackExists)
        {
            // Play the intro at the scheduled playback time
            introAudiosource.PlayScheduled(scheduledPlaybacktime);
            GenerateBeatSyncForScheduledAudioClip(scheduledPlaybacktime, 0.0d, currentBeatMap.IntroTrackDuration);
            // Reassign the scheduled playback time by adding the intro track duration to it
            scheduledPlaybacktime = scheduledPlaybacktime + currentBeatMap.IntroTrackDuration;
        }

        // Play the loop track when assigned to do so
        loopingAudiosource.PlayScheduled(scheduledPlaybacktime);
        // Generate a beat sync for the initial beat loop clip
        GenerateBeatSyncForScheduledAudioClip(scheduledPlaybacktime, currentBeatMap.IntroTrackDuration, currentBeatMap.LoopTrackDuration);
        // Then keep generating beat maps for the consecutive loops
        StartCoroutine(GenerateBeatMapsForLoop(scheduledPlaybacktime + currentBeatMap.LoopTrackDuration));
    }

    /// <summary>
    /// Generates seamless beat maps for the beat map loop
    /// </summary>
    /// <param name="loopEndTimestamp"></param>
    /// <returns></returns>
    IEnumerator GenerateBeatMapsForLoop(double loopEndTimestamp)
    {
        // Wait for the beatmap preview seconds BEFORE the initial loop ends
        double waitUntilDSPtime = loopEndTimestamp - beatMapPreviewTime - 1;
        // Wait until the loop is almost done (end of the loop timestamp - beatmap preview time - 1)
        while(AudioSettings.dspTime < waitUntilDSPtime)
        {
            yield return null;
        }

        bool loopedOnce = false;
        Coroutine beatPlayCoroutine = null;
        // If the song is still looping, keep creating a beatmap syncs for the given loop
        while (loopTrack)
        {
            // Generate beat syncs for the scheduled audioclip
            beatPlayCoroutine = StartCoroutine(GenerateBeatSyncForScheduledAudioClipCoroutine(loopEndTimestamp, currentBeatMap.IntroTrackDuration, currentBeatMap.LoopTrackDuration));
            // Wait until the loop ends - the beatmap preview time - 1
            waitUntilDSPtime = loopEndTimestamp - beatMapPreviewTime - 1;
            // Wait until the loop is almost done before checking to create another beat map
            while (AudioSettings.dspTime < waitUntilDSPtime)
            {
                yield return null;
            }
            // Increment the loop timestamp by the loop track duration
            loopEndTimestamp += currentBeatMap.LoopTrackDuration;
            loopedOnce = true;
        }

        // If a custom outro track exists, then play it
        if(currentBeatMap.CustomOutroTrackExists)
        {
            // If our loop was played at least once, decrement the loopend timestamp by the loop track duration
            if (loopedOnce) {
                loopEndTimestamp -= currentBeatMap.LoopTrackDuration;
                StopCoroutine(beatPlayCoroutine);
            } 
            // Play the outro at the end of the currently playing loop
            outroAudiosource.PlayScheduled(loopEndTimestamp);
            // Generate a beatmap for the outro
            GenerateBeatSyncForScheduledAudioClip(loopEndTimestamp, currentBeatMap.IntroTrackDuration + currentBeatMap.LoopTrackDuration, currentBeatMap.IntroTrackDuration + currentBeatMap.LoopTrackDuration + currentBeatMap.OutroTrackDuration);
        }
    }



    /// <summary>
    /// Coroutine called to generate beat sync data for a scheduled audioclip
    /// </summary>
    /// <param name="clipStartRef">The audiosettings DPS time when the audio clip is scheduled to start</param>
    /// <param name="clipStartOffsetTimestamp">The audioclip starting timestamp offset</param>
    /// <param name="clipLength">The length of the audio clip played</param>
    /// <returns></returns>
    IEnumerator GenerateBeatSyncForScheduledAudioClipCoroutine(double clipStartRef, double clipStartOffsetTimestamp, double clipLength)
    {
        // Create a list of beat sync time stamps
        List<BeatSyncQueueData> beatSynceQueueDataList = new List<BeatSyncQueueData>();

        // Get the starting timestamp of the audio clip
        double minTimeStamp = clipStartOffsetTimestamp;
        // Get the ending timestamp of the audio clip
        double maxTimeStamp = clipStartOffsetTimestamp + clipLength;

        List<double> beatMapListToUse = useBPM ? currentBeatMap.BPMBeatTimestamps : currentBeatMap.BeatTimeStamps;

        // Iterate through our list of beat map timestamps
        for (int i = 0; i < beatMapListToUse.Count; i++)
        {
            double beatTimestamp = beatMapListToUse[i];

            // If the timestamp is within the range of our audio clip, then add the beat map timestamp
            // to our beat sync timestamps
            if (beatTimestamp >= clipStartOffsetTimestamp && beatTimestamp < maxTimeStamp)
            {
                // Create the beat timestamp to add
                // This will be the start time of the audio clip playback + the beat timestamp - the clip start offset time 
                double beatStampToAdd = clipStartRef + beatTimestamp - clipStartOffsetTimestamp;
                BeatSyncQueueData beatToQueue = new BeatSyncQueueData();
                beatToQueue.beatTimestampAudioDSPTime = beatStampToAdd;
                beatToQueue.beatTimestamp = beatTimestamp;
                beatToQueue.beatTimestampIndex = i;
                // Add the beat sync timestamp to our list
                beatSynceQueueDataList.Add(beatToQueue);
            }
        }
        /* Original Code before implementing BeatQueueSyncData structs
        // Iterate through our list of beat map timestamps
        foreach (double beatTimestamp in currentBeatMap.BeatTimeStamps)
        {
            // If the timestamp is within the range of our audio clip, then add the beat map timestamp
            // to our beat sync timestamps
            if(beatTimestamp >= clipStartOffsetTimestamp && beatTimestamp < maxTimeStamp)
            {
                // Create the beat timestamp to add
                // This will be the start time of the audio clip playback + the beat timestamp - the clip start offset time 
                double beatStampToAdd = clipStartRef + beatTimestamp - clipStartOffsetTimestamp;
                // Add the beat sync timestamp to our list
                beatSyncTimeStamps.Add(beatStampToAdd);
            }
        }
        */


        // Iterate through each of the beat sync time stamps
        foreach (BeatSyncQueueData beatSyncQueue in beatSynceQueueDataList)
        {
            // While the audio time is less than the beat sync time stamp, wait
            while(AudioSettings.dspTime < beatSyncQueue.beatTimestampAudioDSPTime - beatMapPreviewTime)
            {
                yield return null;
            }

            BeatSyncReceiver.BeatReceiver.QueueBeat(new BeatSyncData(beatSyncQueue.beatTimestampAudioDSPTime, beatSyncQueue.beatTimestamp, beatSyncQueue.beatTimestampIndex, useBPM));
        }

    }

    private struct BeatSyncQueueData
    {
        public double beatTimestamp;
        public int beatTimestampIndex;
        public double beatTimestampAudioDSPTime;
    }

    /// <summary>
    /// Cretes a coroutine to generate beat sync data for a scheduled audioclip
    /// </summary>
    /// <param name="scheduledClipPlaybackTime"></param>
    /// <param name="clipStartOffset"></param>
    /// <param name="clipLength"></param>
    private void GenerateBeatSyncForScheduledAudioClip(double scheduledClipPlaybackTime, double clipStartOffset, double clipLength)
    {
        StartCoroutine(GenerateBeatSyncForScheduledAudioClipCoroutine(scheduledClipPlaybackTime, clipStartOffset, clipLength));
    }


    
    /// <summary>
    /// Method called to fade out this song's playback
    /// </summary>
    /// <param name="seconds">The duration of the audio volume fade-out</param>
    public void FadeOutSongPlayback(float seconds)
    {
        StopAllCoroutines();
        StartCoroutine(FadeSongPlayBackInSeconds(seconds));
    }

    

    /// <summary>
    /// Method called to stop looping this beat map audiotrack
    /// </summary>
    public void StopLoopingBeatmap()
    {
        this.loopTrack = false;
        loopingAudiosource.loop = false;
    }

    /// <summary>
    /// Coroutine used to fade out the song playback in a given amount of seconds
    /// </summary>
    /// <param name="seconds">The audio volume fade-out duration in seconds</param>
    /// <returns></returns>
    IEnumerator FadeSongPlayBackInSeconds(float seconds)
    {
        
        // Cache a lerp amount
        float lerpAMT = 0.0f;
        // Cache the mixer starting attenuation value
        float mixerStartingValue;
        // Get the attenuation value
        audioMixerChannel.GetFloat(mixerAttenuationParameterName, out mixerStartingValue);
        // Cache a mixer value to store
        float mixerValue = mixerStartingValue;
        yield return null;
        // Cache the timestamp this song is stopping
        float stopPlaybackTimestamp = Time.time;

        // While we're interpolating the volume, keep lerping the value
        while (lerpAMT <= 1.0f)
        {
            // Lerp the value between its starting value and the -80 value
            // over the amount of seconds (duration) required
            lerpAMT = (Time.time - stopPlaybackTimestamp) / seconds;
            mixerValue = Mathf.Lerp(mixerStartingValue, -80.0f, lerpAMT);
            // Set the exposed attenuation parameter to the new mixer value
            audioMixerChannel.SetFloat(mixerAttenuationParameterName, mixerValue);
            yield return null;
        }
        // Stop the transition audio sources and nullify their audioclips
        introAudiosource.Stop();
        loopingAudiosource.Stop();
        outroAudiosource.Stop();
        introAudiosource.clip = null;
        loopingAudiosource.clip = null;
        outroAudiosource.clip = null;
        yield return new WaitForSeconds(0.1f);
        // Reset the audio mixer volume to 0.0f
        audioMixerChannel.SetFloat(mixerAttenuationParameterName, 0.0f);
    }

    

    /// <summary>
    /// Sets the beatmap preview time with a given beatmap player
    /// </summary>
    /// <param name="player"></param>
    public void SetBeatMapPlayer(BeatMapPlayerManager player)
    {
        this.beatMapPreviewTime = player.BeatMapPreviewTime;
        this.useBPM = player.UseBeatPerMinute;
    }

}
