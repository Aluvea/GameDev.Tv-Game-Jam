using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrooveController : MonoBehaviour
{
    [SerializeField] bool startGameWithGrooveModeOn = true;
    [Header("Unity Input Settings")]
    [SerializeField] string grooveInputButtonName = "Fire2";
    
    /// <summary>
    /// Whether or not Groove mode is toggled on
    /// </summary>
    public bool GrooveToggled
    {
        private set;
        get;
    } = true;

    /// <summary>
    /// The Groove Controller singleton
    /// </summary>
    public static GrooveController GrooveControllerSingleton
    {
        private set;
        get;
    }

    /// <summary>
    /// Event raised when the groove mode is changed
    /// </summary>
    public event GrooveChanged GrooveToggleChanged;

    private void Awake()
    {
        GrooveControllerSingleton = this;
        GrooveToggled = !startGameWithGrooveModeOn;
        UpdateGrooveMode(startGameWithGrooveModeOn);
    }

    // Update is called once per frame
    void Update()
    {
        // If the user presses the groove button, then toggle on the groove mode
        if (Input.GetButtonDown(grooveInputButtonName))
        {
            UpdateGrooveMode(true);
        }
    }

    private void UpdateGrooveMode(bool mode)
    {
        if (GrooveToggled == mode) return;
        GrooveToggled = mode;
        if(GrooveToggleChanged != null)
        {
            GrooveToggleChanged.Invoke(GrooveToggled);
        }

        Debug.LogWarning("Groove is " + mode.ToString());
    }

    public void ToggleOffGrooveMode()
    {
        UpdateGrooveMode(false);
    }

    public delegate void GrooveChanged(bool toggled);
}



