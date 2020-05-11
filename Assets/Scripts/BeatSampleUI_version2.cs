using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatSampleUI_version2 : MonoBehaviour
{
    private CanvasGroup uiCanvasGroup;

    [SerializeField] RectTransform thisBeatSampleRect;
    [SerializeField] UnityEngine.UI.RawImage innerCircle;
    [SerializeField] UnityEngine.UI.RawImage outerCircle;



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
    public float timeToLiveAfterBeatTargetTime = 3.0f;

    [SerializeField]
    float moveSpeedAfterBeatTargetTime = 1.0f;

    private void Awake()
    {
        uiCanvasGroup = GetComponent<CanvasGroup>();
    }

    public void AnimateBeatData(BeatSyncData beatData, float startingZRotation, float targetZRotation, Vector3 endingDirection)
    {
        if (uiCanvasGroup == null) uiCanvasGroup = GetComponent<CanvasGroup>();
        Vector3 eulerAngle = new Vector3(0.0f, 0.0f, startingZRotation);
        this.transform.eulerAngles = eulerAngle;
        if (PlayingBeat)
        {
            Debug.LogError("Already playing a beat!");
            return;
        }

        PlayingBeatSync = beatData;
        uiCanvasGroup.alpha = 0.0f;
        this.gameObject.SetActive(true);
        StartCoroutine(AnimateBeatCoroutine(beatData, eulerAngle, targetZRotation, endingDirection));
    }

    IEnumerator AnimateBeatCoroutine(BeatSyncData beatData, Vector3 currentEulerAngle, float  targetZRotation, Vector3 endingDirection)
    {
        PlayingBeat = true;
        float endBeatAnimationTime = beatData.BeatTargetTime + timeToLiveAfterBeatTargetTime;
        float lerpAMT = 0.0f;
        float startingZRotation = currentEulerAngle.z;
        float startTimestamp = Time.time;
        float animationDuration = beatData.BeatTargetTime - Time.time;
        uiCanvasGroup.alpha = 0.0f;
        while (lerpAMT < 1.0f)
        {
            lerpAMT = (Time.time - startTimestamp) / animationDuration;
            currentEulerAngle.z = Mathf.Lerp(startingZRotation, targetZRotation, lerpAMT);
            transform.eulerAngles = currentEulerAngle;
            uiCanvasGroup.alpha = lerpAMT;

            yield return null;
        }

        uiCanvasGroup.alpha = 1.0f;
        currentEulerAngle.z = targetZRotation;
        transform.eulerAngles = currentEulerAngle;
        float fadeOutTimestamp = Time.time;
        float fadeOutDuration = endBeatAnimationTime - 0.25f - Time.time;

        while (Time.time < endBeatAnimationTime)
        {
            lerpAMT = (Time.time - startTimestamp) / animationDuration;
            transform.Translate(endingDirection * moveSpeedAfterBeatTargetTime * Time.deltaTime, Space.World);
            uiCanvasGroup.alpha = Mathf.Lerp(1.0f, 0.0f, (Time.time - fadeOutTimestamp) / fadeOutDuration);
            yield return null;
        }


        PlayingBeat = false;

        BeatSampleUIManager_Version03.OnBeatSampleUIAnimationEnded(this);

    }


    public virtual void SetBeatSampleColor(Color innerCircleColor, Color outerCircleColor)
    {
        innerCircle.color = innerCircleColor;
        outerCircle.color = outerCircleColor;
    }
}
