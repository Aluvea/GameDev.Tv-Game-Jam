using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    float dissolveEffectDuration = 3.0f;

    [Tooltip("The renderer references that should have their material changed to a dissolve material")]
    [SerializeField] Renderer [] renderers;
    [Tooltip("The dissolve material reference")]
    [SerializeField] Material dissolveMaterial;
    [Tooltip("The dissolve parameter name on the dissolve material")]
    [SerializeField] string dissolveParameterName = "_Cutoff";
    private float startInSeconds;
    private GameObject gameObjectToDestroyWhenDoneDissolving;

    /// <summary>
    /// Plays the dissolve animation
    /// </summary>
    /// <param name="waitSecondsToStart">How many seconds should be awaited before the dissolve starts</param>
    /// <param name="dissolveDuration">Dissolve animation duration (in seconds)</param>
    /// <param name="objectToDestroyWhenDoneDissolving">The game object that should be destroyed when the dissolve animation is done (Should be the root game object of this dissolve animation)</param>
    public void PlayDissolveAnimation(float waitSecondsToStart, float dissolveDuration, GameObject objectToDestroyWhenDoneDissolving)
    {
        if (waitSecondsToStart < 0.0f) waitSecondsToStart = 0.0f;
        if (dissolveDuration < 0.0f) dissolveDuration = 0.0f;
        this.dissolveEffectDuration = dissolveDuration;
        this.gameObjectToDestroyWhenDoneDissolving = objectToDestroyWhenDoneDissolving;
        this.startInSeconds = waitSecondsToStart;
        StartCoroutine(DissolveCoroutine());
    }

    private IEnumerator DissolveCoroutine()
    {
        if(startInSeconds > 0)
        {
            yield return new WaitForSeconds(startInSeconds);
        }

        Material uniqueMaterial = new Material(dissolveMaterial);
        uniqueMaterial.SetFloat(dissolveParameterName, 0.0f);
        
        foreach (Renderer renderer in renderers)
        {
            renderer.material = uniqueMaterial;
        }

        yield return null;

        float startingTime = Time.time;
        float lerpAMT = 0.0f;
        while(lerpAMT < 1.0f)
        {
            lerpAMT = (Time.time - startingTime) / dissolveEffectDuration;
            uniqueMaterial.SetFloat(dissolveParameterName, lerpAMT);
            yield return null;
            if (lerpAMT > 1.0f) lerpAMT = 1.0f;
        }

        uniqueMaterial.SetFloat(dissolveParameterName, lerpAMT);
        yield return null;
        Destroy(gameObjectToDestroyWhenDoneDissolving);
    }
}
