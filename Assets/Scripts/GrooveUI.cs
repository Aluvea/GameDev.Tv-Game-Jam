﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrooveUI : MonoBehaviour
{
    [SerializeField] Animator grooveAnimator;
    [SerializeField] CanvasGroup grooveCanvasGroup;
    [SerializeField] string grooveParameterName = "visible";


    private void Start()
    {
        grooveCanvasGroup.alpha = 0.0f;
        if(GrooveController.GrooveControllerSingleton != null)
        {
            GrooveController.GrooveControllerSingleton.GrooveToggleChanged += ToggleGrooveUI;
            ToggleGrooveUI(GrooveController.GrooveControllerSingleton.GrooveToggled);
        }
    }

    private void OnDestroy()
    {
        if (GrooveController.GrooveControllerSingleton != null)
        {
            GrooveController.GrooveControllerSingleton.GrooveToggleChanged -= ToggleGrooveUI;
        }
    }



    /// <summary>
    /// Method called to toggle the groove UI
    /// </summary>
    /// <param name="toggle"></param>
    public void ToggleGrooveUI(bool toggle)
    {
        grooveAnimator.SetBool(grooveParameterName, toggle);
    }

}
