using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatSampleUI_version4 : MonoBehaviour
{

    private CanvasGroup uiCanvasGroup;

    [SerializeField] UICircle circle;

    [SerializeField] float startingRadius;
    [SerializeField] float endingRadius;

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

    [Range(0.0f, 5.0f)]
    public float timeToLiveAfterBeatTargetTime = 0.0f;



    private void Awake()
    {
        uiCanvasGroup = GetComponent<CanvasGroup>();
    }

    public void AnimateBeatData(BeatSyncData beatData)
    {
        if (uiCanvasGroup == null) uiCanvasGroup = GetComponent<CanvasGroup>();

        if (PlayingBeat)
        {
            Debug.LogError("Already playing a beat!");
            return;
        }

        transform.SetAsLastSibling();

        PlayingBeatSync = beatData;
        uiCanvasGroup.alpha = 0.0f;
        this.gameObject.SetActive(true);
        StartCoroutine(AnimateBeatCoroutine(beatData));
    }

    IEnumerator AnimateBeatCoroutine(BeatSyncData beatData)
    {
        PlayingBeat = true;
        float endBeatAnimationTime = beatData.BeatTargetTime + timeToLiveAfterBeatTargetTime;
        float lerpAMT = 0.0f;

        float startTimestamp = Time.time;
        float animationDuration = beatData.BeatTargetTime - Time.time;
        uiCanvasGroup.alpha = 0.0f;

        circle.UpdateCircleSize(startingRadius);

        yield return null;

        while (lerpAMT < 1.0f)
        {
            lerpAMT = (Time.time - startTimestamp) / animationDuration;
            circle.UpdateCircleSize(Mathf.Lerp(startingRadius, endingRadius, lerpAMT));
            uiCanvasGroup.alpha = lerpAMT;

            yield return null;
        }

        circle.UpdateCircleSize(endingRadius);

        Debug.Log("UI Beat!");

        float fadeOutTimestamp = Time.time;
        float fadeOutDuration = endBeatAnimationTime - 0.25f - Time.time;

        while (Time.time < endBeatAnimationTime)
        {
            lerpAMT = (Time.time - startTimestamp) / animationDuration;
            uiCanvasGroup.alpha = Mathf.Lerp(1.0f, 0.0f, (Time.time - fadeOutTimestamp) / fadeOutDuration);
            yield return null;
        }


        PlayingBeat = false;

        BeatSampleUIManager_Version05.OnBeatSampleUIAnimationEnded(this);

    }


    public void SetBeatSampleColor(Color sampleColor)
    {
        circle.UpdateCircleColor(sampleColor);
    }
}
