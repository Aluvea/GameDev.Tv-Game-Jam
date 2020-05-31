using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoManager : MonoBehaviour
{
    [Header("Reload Animation References")]
    [SerializeField] Animator reloadAnimator;
    [SerializeField] string reloadAnimationTriggerName = "reload";
    [Header("Ammo Settings")]
    [Min(10)]
    [SerializeField] int maxClipAmmoCount = 10;
    [Min(0.5f)]
    [SerializeField] float perfectReloadSpeed = 1.0f;
    [Min(0.5f)]
    [SerializeField] float goodReloadSpeed = 0.78f;
    [Min(0.5f)]
    [SerializeField] float okReloadSpeed = 0.60f;
    [Min(0.5f)]
    [SerializeField] float missReloadSpeed = 0.5f;
    private AmmoHUD ammoHud;
    private int currentClipCount;

    /// <summary>
    /// Whether or not the player can shoot (this is false if the player is reloading or has no ammo loaded in the clip)
    /// </summary>
    public bool CanShoot
    {
        get
        {
            if (currentClipCount <= 0) return false;
            if (reloading) return false;
            return true;
        }
    }


    private void Awake()
    {
        ammoHud = FindObjectOfType<AmmoHUD>();
        ammoHud.ToggleAmmoHUD(true);
        currentClipCount = maxClipAmmoCount;
    }

    private bool reloading = false;

    /// <summary>
    /// Method called to reload
    /// </summary>
    public void Reload()
    {
        // If a reload animation is already playing then do nothing
        if (reloading) return;
        // Otherwise, start reloading
        reloading = true;
        // Play the reload animation
        //...
        // Request an input action
        BeatSyncReceiver.BeatReceiver.RequestInputAction(out BeatInputSync inputSync);
        float reloadSpeed = 1.0f;
        switch (inputSync)
        {
            case BeatInputSync.PERFECT:
                reloadSpeed = perfectReloadSpeed;
                break;
            case BeatInputSync.GOOD:
                reloadSpeed = goodReloadSpeed;
                break;
            case BeatInputSync.OK:
                reloadSpeed = okReloadSpeed;
                break;
            case BeatInputSync.MISS:
                reloadSpeed = missReloadSpeed;
                break;
            default:
                break;
        }
        // Adjust the reload animation speed based on the input sync
        reloadAnimator.speed = reloadSpeed;
        reloadAnimator.SetTrigger(reloadAnimationTriggerName);
        // Callback reload animation finished!!!!
    }

    /// <summary>
    /// Method called when the reload animation finishes
    /// </summary>
    public void ReloadAnimationFinished()
    {
        // Reset the current clip count to the max clip ammo count
        currentClipCount = maxClipAmmoCount;
        // Update the ammo HUD
        ammoHud.UpdateAmmoText(currentClipCount);
        // set reloading to false
        reloading = false;
    }

    /// <summary>
    /// Method called to dispense ammo
    /// </summary>
    public void DispenseAmmo()
    {
        // Decrement the current ammo  count
        currentClipCount--;
        // If the current ammo clip is less than or equal to 0, set it to 0 and set CanShoot to false (No ammo in clip)
        if (currentClipCount <= 0)
        {
            currentClipCount = 0;
        }
        // Update the ammo text HUD
        ammoHud.UpdateAmmoText(currentClipCount);
    }
}
