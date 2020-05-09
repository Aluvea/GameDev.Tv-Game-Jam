using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatSampleUIV2 : BeatSampleUI
{
    [SerializeField] UnityEngine.UI.Image lineImage;
    [SerializeField] bool useOuterCircleAsColor;

    public override void SetBeatSampleColor(Color innerCircleColor, Color outerCircleColor)
    {
        if (useOuterCircleAsColor)
        {
            lineImage.color = outerCircleColor;
        }
        else
        {
            lineImage.color = innerCircleColor;
        }
    }
}
