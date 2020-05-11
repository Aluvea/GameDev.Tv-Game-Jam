using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboUI : MonoBehaviour
{

    [SerializeField] TMPro.TextMeshProUGUI comboUI;

    /// <summary>
    /// Updates the combo count displayed
    /// </summary>
    /// <param name="count"></param>
    public void UpdateComboCount(int count)
    {
        comboUI.text = "Combo: " + count.ToString();
    }
}
