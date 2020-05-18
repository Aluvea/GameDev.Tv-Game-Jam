using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BeatSyncIndicator : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textIndicator;
    [SerializeField] CanvasGroup textCanvasGroup;

    [Header("Perfect Settings")]
    [SerializeField] string perfectTextPrompt = "Perfect!";
    [SerializeField] Material perfectTextMaterial;
    [Header("Good Settings")]
    [SerializeField] string goodTextPrompt = "Good!";
    [SerializeField] Material goodTextMaterial;
    [Header("OK Settings")]
    [SerializeField] string OKTextPrompt = "OK!";
    [SerializeField] Material OKTextMaterial;
    [Header("Miss Settings")]
    [SerializeField] string MissTextPrompt = "Miss!";
    [SerializeField] Material missTextMaterial;
    [Header("Bounce Animation Settings")]
    [SerializeField] float bounceHeightMagnitude = 5.0f;
    [SerializeField] float bounceWidthMagnitude = 5.0f;
    [SerializeField] float bounceAnimationDuration = 3.0f;

    Vector3 startingLocalPosition;

    public bool AnimationPlaying
    {
        private set;
        get;
    } = false;

    private void Awake()
    {
        
        textCanvasGroup.alpha = 0.0f;
    }

    public void PlayAnimation(BeatInputSync sync)
    {
        if (AnimationPlaying)
        {
            Debug.LogError("BeatSyncIndicator is already playing an animation!");
            return;
        }

        
        switch (sync)
        {
            case BeatInputSync.PERFECT:
                textIndicator.text = perfectTextPrompt;
                textIndicator.fontSharedMaterial = perfectTextMaterial;
                break;
            case BeatInputSync.GOOD:
                textIndicator.text = goodTextPrompt;
                textIndicator.fontSharedMaterial = goodTextMaterial;
                break;
            case BeatInputSync.OK:
                textIndicator.text = OKTextPrompt;
                textIndicator.fontSharedMaterial = OKTextMaterial;
                break;
            case BeatInputSync.MISS:
                textIndicator.text = MissTextPrompt;
                textIndicator.fontSharedMaterial = missTextMaterial;
                break;
            default:
                break;
        }
        AnimationPlaying = true;
        StartCoroutine(BounceAnimationCoroutine());
    }


    IEnumerator BounceAnimationCoroutine()
    {
        startingLocalPosition = transform.localPosition;
        float endTimeStamp = Time.time + bounceAnimationDuration;

        Vector3 oscillatingVector = Vector3.zero;
        float startTime = Time.time;
        float runtime;
        float oscillation;
        float runtimeFraction;
        textCanvasGroup.alpha = 1.0f;
        while (Time.time < endTimeStamp)
        {
            runtime = (Time.time - startTime);
            runtimeFraction = runtime / bounceAnimationDuration;
            oscillation = Mathf.Sin(runtimeFraction * Mathf.PI);
            oscillatingVector.y = oscillation * bounceHeightMagnitude;
            oscillatingVector.x = bounceWidthMagnitude * runtimeFraction;
            if(runtimeFraction >= 0.5f)
            {
                textCanvasGroup.alpha = oscillation;
            }
            this.transform.localPosition = startingLocalPosition + oscillatingVector;
            yield return null;
        }

        textCanvasGroup.alpha = 0.0f;

        AnimationPlaying = false;
        if(OnAnimationEnded != null)
        {
            OnAnimationEnded.Invoke(this);
        }
    }
    

    public void ChangeBounceSettings(float targetSign)
    {
        // If the target sign is the same as the width sign, then do nothing
        targetSign = Mathf.Sign(targetSign);
        if(targetSign == Mathf.Sign(bounceWidthMagnitude))
        {
            bounceWidthMagnitude *= -1.0f;
        }
    }

    /// <summary>
    /// Event raised when this beat sync indicator is done playing an animation
    /// </summary>
    public event AnimationEnded OnAnimationEnded;

    public delegate void AnimationEnded(BeatSyncIndicator beatPlayer);

}
