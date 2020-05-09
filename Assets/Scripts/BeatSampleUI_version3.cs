using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatSampleUI_version3 : MonoBehaviour
{
    private CanvasGroup uiCanvasGroup;

    [SerializeField] UnityEngine.UI.RawImage innerCircle;
    [SerializeField] UnityEngine.UI.RawImage outerCircle;
    [SerializeField] float moveShiftSpeed = 15.0f;



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

    [Range(0.0f,5.0f)]
    public float timeToLiveAfterBeatTargetTime = 0.0f;
    


    private void Awake()
    {
        uiCanvasGroup = GetComponent<CanvasGroup>();
    }
    
    public void AnimateBeatData(BeatSyncData beatData, Vector3 destination, Vector3 directionToGoAfterDesitionation)
    {
        if (uiCanvasGroup == null) uiCanvasGroup = GetComponent<CanvasGroup>();

        if (PlayingBeat)
        {
            Debug.LogError("Already playing a beat!");
            return;
        }

        PlayingBeatSync = beatData;
        uiCanvasGroup.alpha = 0.0f;
        this.gameObject.SetActive(true);
        StartCoroutine(AnimateBeatCoroutine(beatData, destination, directionToGoAfterDesitionation));
    }

    IEnumerator AnimateBeatCoroutine(BeatSyncData beatData, Vector3 destination, Vector3 directionToGoAfterDesitionation)
    {
        PlayingBeat = true;
        float endBeatAnimationTime = beatData.BeatTargetTime + timeToLiveAfterBeatTargetTime;
        float lerpAMT = 0.0f;

        float startTimestamp = Time.time;
        float animationDuration = beatData.BeatTargetTime - Time.time;
        uiCanvasGroup.alpha = 0.0f;
        Vector3 startingPosition = transform.position;
        while(lerpAMT <= 1.0f)
        {
            lerpAMT = (Time.time - startTimestamp) / animationDuration;
            transform.position = Vector3.Lerp(startingPosition, destination, lerpAMT);

            uiCanvasGroup.alpha = lerpAMT;

            yield return null;
        }

        Debug.Log("UI Beat!");

        float fadeOutTimestamp = Time.time;
        float fadeOutDuration = endBeatAnimationTime - 0.25f - Time.time ;

        while (Time.time < endBeatAnimationTime)
        {
            lerpAMT = (Time.time - startTimestamp) / animationDuration;
            transform.Translate(directionToGoAfterDesitionation * Time.deltaTime * moveShiftSpeed);
            uiCanvasGroup.alpha = Mathf.Lerp(1.0f, 0.0f, (Time.time - fadeOutTimestamp) /fadeOutDuration);
            yield return null;
        }


        PlayingBeat = false;

        BeatSampleUIManager_Version04.OnBeatSampleUIAnimationEnded(this);
        
    }


    public virtual void SetBeatSampleColor(Color innerCircleColor, Color outerCircleColor)
    {
        innerCircle.color = innerCircleColor;
        outerCircle.color = outerCircleColor;
    }


}
