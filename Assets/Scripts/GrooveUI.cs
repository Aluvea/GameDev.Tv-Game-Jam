using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrooveUI : MonoBehaviour
{
    [SerializeField] Animator grooveAnimator;
    [SerializeField] CanvasGroup grooveCanvasGroup;
    [SerializeField] string grooveParameterName = "visible";


    private bool lastToggle = false;


    private void Start()
    {
        grooveCanvasGroup.alpha = 0.0f;
    }


    
    /// <summary>
    /// Method called to toggle the groove UI
    /// </summary>
    /// <param name="toggle"></param>
    public void ToggleGroove(bool toggle)
    {
        if (lastToggle == toggle) return;
        grooveAnimator.SetBool(grooveParameterName, toggle);
        lastToggle = toggle;
    }

}
