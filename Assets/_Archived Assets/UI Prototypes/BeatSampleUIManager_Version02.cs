using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatSampleUIManager_Version02 : BeatSampleUIManager
{
    [SerializeField] MeterUIScript meterScript;

    [SerializeField] Color perfectColor;

    [SerializeField] Color goodColor;

    [SerializeField] Color okColor;

    [SerializeField] Color missedColor;


    public override void DisplayBeat(BeatSyncData beatSyncData)
    {
        meterScript.FillMeter(beatSyncData.BeatTargetTime - Time.time);
    }

    public override void DisplayInputMissedBeat()
    {
        meterScript.SetMeterColor(missedColor);
    }

    public override void UpdateBeatSyncUIState(BeatSyncData beatHit, BeatInputSync beatInput)
    {
        switch (beatInput)
        {
            case BeatInputSync.PERFECT:
                meterScript.SetMeterColor(perfectColor);
                break;
            case BeatInputSync.GOOD:
                meterScript.SetMeterColor(goodColor);
                break;
            case BeatInputSync.OK:
                meterScript.SetMeterColor(okColor);
                break;
            case BeatInputSync.MISS:
                meterScript.SetMeterColor(missedColor);
                break;
        }
    }
}
