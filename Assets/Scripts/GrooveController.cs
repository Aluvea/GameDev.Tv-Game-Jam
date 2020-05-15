using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrooveController : MonoBehaviour
{
    [Header("Groove Game Object Settings / References")]
    [SerializeField] GrooveUI grooveUIController;
    [SerializeField] bool startGameWithGrooveModeOn = true;
    [Header("Unity Input Settings")]
    [SerializeField] string grooveInputButtonName = "Fire2";

    public bool GrooveToggled
    {
        private set;
        get;
    } = false;

    private void Awake()
    {
        GrooveToggled = !startGameWithGrooveModeOn;
        UpdateGrooveMode(startGameWithGrooveModeOn);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown(grooveInputButtonName))
        {
            UpdateGrooveMode(true);
        }
    }

    private void UpdateGrooveMode(bool mode)
    {
        if (GrooveToggled == mode) return;
        GrooveToggled = mode;
        grooveUIController.ToggleGrooveUI(GrooveToggled);
        Debug.LogWarning("Groove is " + mode.ToString());
    }

    public void ToggleOffGrooveMode()
    {
        UpdateGrooveMode(false);
    }
}
