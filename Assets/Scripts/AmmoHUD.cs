using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoHUD : MonoBehaviour
{
    [SerializeField] GameObject ammoImageRef;
    [SerializeField] TMPro.TextMeshProUGUI ammoText;
    [SerializeField] TMPro.TextMeshProUGUI comboBonusText;
    [Min(5.0f)]
    [SerializeField] float comboBonusDisplayDuration = 10.0f;
    [SerializeField] bool toggleAmmoHUD = false;

    

    private void Awake()
    {
        ToggleAmmoHUD(toggleAmmoHUD);
    }

    public void ToggleAmmoHUD(bool toggle)
    {
        ammoImageRef.SetActive(toggle);
        ammoText.gameObject.SetActive(toggle);
        comboBonusText.gameObject.SetActive(toggle);
    }

    public void UpdateAmmoText(int ammoCount)
    {
        ammoText.text = "Ammo x " + ammoCount;
    }

    Coroutine comboBonusCoroutine = null;
    public void PlayComboBonusAnimation(int comboBonus)
    {
        comboBonusText.text = "Combo            Bonus         +           " + comboBonus;
        if (comboBonusCoroutine != null) StopCoroutine(comboBonusCoroutine);
        comboBonusCoroutine =  StartCoroutine(ShowComboBonusText());
    }

    IEnumerator ShowComboBonusText()
    {
        comboBonusText.GetComponent<Animator>().SetBool("visible", true);
        yield return new WaitForSeconds(comboBonusDisplayDuration);
        comboBonusText.GetComponent<Animator>().SetBool("visible", false);
        comboBonusCoroutine = null;
    }




}
