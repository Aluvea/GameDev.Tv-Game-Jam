using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Beat sample UI for a targetable beat map
/// </summary>
public class TargetableBeatSampleUI : MonoBehaviour
{
    /// <summary>
    /// This beat sample's target UI circle
    /// </summary>
    [SerializeField] UICircle beatSampleTarget;

    /// <summary>
    /// This beat sample's UI circle
    /// </summary>
    [SerializeField] UICircle beatSamplePreview;

    /// <summary>
    /// This beat sample's canvas group
    /// </summary>
    [SerializeField] CanvasGroup sampleCanvasGroup;

    /// <summary>
    /// How far this beat sample radius spans away from the beat sample target circle
    /// </summary>
    [Min(35.0f)]
    [Tooltip("How far away this beat sample radius spans away from the target circle when shown")]
    [SerializeField] float radiusDistanceOffset = 35.0f;

    /// <summary>
    /// How long this beat should be displayed before it's rendered invisible
    /// </summary>
    [SerializeField] float timeToStayAlive = 1.0f;

    [Min(0.0f)]
    [SerializeField] float fadeInDuration = 1.0f;

    private TargetableBeatMapUI targetableBeatMapRef;

    Coroutine beatSampleCoroutine = null;

    private void Awake()
    {
        sampleCanvasGroup.alpha = 0.0f;
    }

    public bool PlayingBeat
    {
        private set;
        get;
    } = false;

    /// <summary>
    /// The beat sync that this beat sample UI is playing
    /// </summary>
    public BeatSyncData PlayingBeatSync
    {
        private set;
        get;
    }
    

    public void AnimateBeatData(BeatSyncData beatData, Color missColor)
    {
        if (sampleCanvasGroup == null) sampleCanvasGroup = GetComponent<CanvasGroup>();

        if (PlayingBeat)
        {
            Debug.LogError("Already playing a beat!");
            return;
        }

        transform.SetAsLastSibling();

        PlayingBeatSync = beatData;
        PlayingBeat = true;
        sampleCanvasGroup.alpha = 0.0f;
        StartCoroutine(AnimateBeatCoroutine(beatData));

        StartCoroutine(SimulateMissedBeat(beatData, beatSamplePreview.CircleColor, missColor));
    }



    IEnumerator AnimateBeatCoroutine(BeatSyncData beatData)
    {
        StartCoroutine(FadeCanvasCoroutine(1.0f, fadeInDuration));
        float startingRadius = beatSampleTarget.radius + radiusDistanceOffset;
        float lerpAMT = 0.0f;
        float startingTimeStamp = Time.time;
        float beatSampleDuration = beatData.BeatTargetTime - Time.time;
        beatSamplePreview.radius = startingRadius;

        while (lerpAMT <= 1.0f)
        {
            yield return new WaitForEndOfFrame();

            startingRadius = beatSampleTarget.radius + radiusDistanceOffset;
            lerpAMT = (Time.time - startingTimeStamp) / beatSampleDuration;
            beatSamplePreview.radius = Mathf.Lerp(startingRadius, beatSampleTarget.radius, lerpAMT);

        }



        StartCoroutine(FadeCanvasCoroutine(0.0f, timeToStayAlive));
        float endTimeStamp = Time.time + timeToStayAlive;

        while (Time.time <= endTimeStamp)
        {
            yield return new WaitForEndOfFrame();

            beatSamplePreview.radius = beatSampleTarget.radius;
        }

        while (fadeInProgress)
        {
            yield return null;
        }

        PlayingBeat = false;
        beatSampleCoroutine = null;

        if(targetableBeatMapRef != null)
        {
            targetableBeatMapRef.EnqueueBeatSample(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    bool fadeInProgress = false;

    IEnumerator FadeCanvasCoroutine(float targetAlpha, float duration)
    {
        fadeInProgress = true;
        float startingTimeStamp = Time.time;

        float lerpAMT = 0.0f;
        float startingAlpha = sampleCanvasGroup.alpha;

        while(lerpAMT <= 1.0f)
        {
            lerpAMT = (Time.time - startingTimeStamp) / duration;
            sampleCanvasGroup.alpha = Mathf.Lerp(startingAlpha, targetAlpha, lerpAMT);
            yield return null;
        }
        fadeInProgress = false;
    }

    /// <summary>
    /// Sets the beat sample color
    /// </summary>
    /// <param name="sampleColor"></param>
    public void SetBeatSampleColor(Color sampleColor)
    {
        beatSamplePreview.UpdateCircleColor(sampleColor);
    }


    public void SetBeatSampleTargetParent(TargetableBeatMapUI beatMapRef)
    {
        targetableBeatMapRef = beatMapRef;
    }

    IEnumerator SimulateMissedBeat(BeatSyncData beatData, Color startingColor, Color missColor)
    {
        float missTime = beatData.BeatTargetTime + BeatSyncReceiver.BeatReceiver.MaxTimeToHitABeat;

        while(Time.time <= missTime)
        {
            yield return null;
        }

        if(this.beatSamplePreview.CircleColor == startingColor)
        {
            SetBeatSampleColor(missColor);
        }
    }
}
