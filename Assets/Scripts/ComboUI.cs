using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboUI : MonoBehaviour
{
    [Tooltip("The text where the combo count is acutally displayed")]
    [SerializeField] TMPro.TextMeshProUGUI comboCountText;

    [Tooltip("The canvas group where the combo animation is played")]
    [SerializeField] CanvasGroup animationCanvasGroup;

    [Tooltip("The minimum count of combos required for animations to appear")]
    [Range(5,300)]
    [SerializeField] int comboCountRequiredForAnimations = 5;

    [Tooltip("The consecutive count of combos required for full flames")]
    [Range(5,300)]
    [SerializeField] int consecutiveCombosRequiredForFullAnimations = 5;

    private void Awake()
    {
        animationCanvasGroup.alpha = 0.0f;
    }

    private void Start()
    {
        if(BeatSyncReceiver.BeatReceiver != null)
        {
            BeatSyncReceiver.BeatReceiver.OnComboCountChanged += UpdateComboCount;
            comboCountText.text = BeatSyncReceiver.BeatReceiver.ComboCount.ToString();
        }
    }

    private void OnDestroy()
    {
        if (BeatSyncReceiver.BeatReceiver != null)
        {
            BeatSyncReceiver.BeatReceiver.OnComboCountChanged -= UpdateComboCount;
        }
    }


    /// <summary>
    /// Updates the combo count displayed
    /// </summary>
    /// <param name="count"></param>
    public void UpdateComboCount(int count)
    {
        comboCountText.text = count.ToString();
        UpdateComboAnimation(count);
    }

    /// <summary>
    /// Coroutine references for changing the animation's transparency
    /// </summary>
    Coroutine animationTransparencyChangeCoroutine = null;

    /// <summary>
    /// The animation's transparency target (0 - 1)
    /// </summary>
    private float animationTargetTransparency = 0.0f;

    /// <summary>
    /// Updates the combo animation based on the player's combo count
    /// </summary>
    /// <param name="count"></param>
    private void UpdateComboAnimation(int count)
    {
        /// If the combo count is more than 0 but less than the combo count required for animations, then do nothing
        if(count > 0 && count < comboCountRequiredForAnimations)
        {
            return;
        }
        // If the combo count is 0, then try to set the animation transparency to 0
        if(count == 0)
        {
            // if the animation target transparency is already 0, then do nothing (escape from the function)
            if (animationTargetTransparency == 0.0f) return;
            // Otherwise, set the target trasnparency to 0
            animationTargetTransparency = 0.0f;
        }
        // If the count is more than or equal to the combo count required for animations,
        // Then set the animation transparency
        else if(count >= comboCountRequiredForAnimations)
        {
            // If the animation transparency is already 1 or more, then do nothing
            if (animationTargetTransparency >= 1.0f) return;
            // The animation target transparency will be:
            // count of combos hit during animations / consecutive count of combos required for full animation transparency
            animationTargetTransparency = (float)(count - comboCountRequiredForAnimations) / (float)consecutiveCombosRequiredForFullAnimations;
        }
        // If the animation alpha change coroutine is not null, then stop it
        if(animationTransparencyChangeCoroutine != null)
        {
            StopCoroutine(animationTransparencyChangeCoroutine);
        }
        // Change the animation transparency via a coroutine, assign it to the coroutine reference
        animationTransparencyChangeCoroutine = StartCoroutine(ChangeAnimationTransparency(animationTargetTransparency));
    }

    /// <summary>
    /// Coroutine used to change the animation group's transparency
    /// </summary>
    /// <param name="targetAlpha">The animation's target transparency (0 -1)</param>
    /// <returns></returns>
    IEnumerator ChangeAnimationTransparency(float targetAlpha)
    {
        // Clamp the target alpha between 0 and 1
        targetAlpha = Mathf.Clamp01(targetAlpha);
        // Cache the current alpha amount
        float startingAlpha = animationCanvasGroup.alpha;
        // Cache the starting time
        float startingTime = Time.time;
        // Cache the animation time duration
        float animationDuration = 1.5f;
        // Cache the lerp amount (0 - 1)
        float lerpAMT = 0.0f;
        // While the lerp amount is less than 1, transition the lerp amount to 1
        while(lerpAMT < 1.0f)
        {
            // Lerp amount will be the current animation duration / the total animation duration
            lerpAMT = (Time.time - startingTime) / animationDuration;
            lerpAMT = Mathf.Clamp01(lerpAMT);
            // Lerp the transparency to the target alpha
            animationCanvasGroup.alpha = Mathf.Lerp(startingAlpha, targetAlpha, lerpAMT);
            yield return null;
        }
        // Set the canvas group alpha to the target alpha assigned
        animationCanvasGroup.alpha = targetAlpha;
    }
}
