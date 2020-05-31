using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoHUD : MonoBehaviour
{
    [SerializeField] GameObject ammoImageRef;
    [SerializeField] TMPro.TextMeshProUGUI ammoText;

    [SerializeField] bool toggleAmmoHUD = false;


    private void Awake()
    {
        ToggleAmmoHUD(toggleAmmoHUD);
    }

    public void ToggleAmmoHUD(bool toggle)
    {
        ammoImageRef.SetActive(toggle);
        ammoText.gameObject.SetActive(toggle);
    }

    public void UpdateAmmoText(int ammoCount)
    {
        ammoText.text = "Ammo x " + ammoCount;
    }


}
