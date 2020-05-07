using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeterUIScript : MonoBehaviour
{
    [SerializeField] RectTransform meterFillTransform;

    [SerializeField] UnityEngine.UI.RawImage meterFillImage;


    [SerializeField] bool overrideSecondsToEmpty = false;
    [Range(0.0f,3.0f)]
    [SerializeField] float secondsToEmpty = 0.0f;

    public void FillMeter(float secondsToFill)
    {
        if(meterFillCoroutine != null)
        {
            StopCoroutine(meterFillCoroutine);
        }

        meterFillCoroutine = StartCoroutine(FillMeterCoroutine(secondsToFill));
    }

    Coroutine meterFillCoroutine = null;

    private IEnumerator FillMeterCoroutine(float secondsToFill)
    {
        Vector3 meterRotation = meterFillTransform.rotation.eulerAngles;
        float lerpAMT = 0.0f;
        float startingTimeStamp = Time.time;

        while(lerpAMT <= 1.0f)
        {
            lerpAMT = (Time.time - startingTimeStamp) / secondsToFill;
            meterRotation.z = Mathf.Lerp(-90.0f, 0.0f, lerpAMT);
            meterFillTransform.eulerAngles = meterRotation;
            yield return null;
        }

        

        startingTimeStamp = Time.time;
        lerpAMT = 0.0f;
        if (overrideSecondsToEmpty)
        {
            secondsToFill = secondsToEmpty;
            if(secondsToFill == 0.0f)
            {
                meterRotation.z = -90.0f;
                meterFillTransform.eulerAngles = meterRotation;
                lerpAMT = 2.0f;
                yield return null;
            }
        }

        while (lerpAMT <= 1.0f)
        {
            lerpAMT = (Time.time - startingTimeStamp) / secondsToFill;
            meterRotation.z = Mathf.Lerp(0.0f, -90.0f, lerpAMT);
            meterFillTransform.eulerAngles = meterRotation;
            yield return null;
        }

        meterFillCoroutine = null;
    }

    public void SetMeterColor(Color color)
    {
        meterFillImage.color = color;
    }
}
