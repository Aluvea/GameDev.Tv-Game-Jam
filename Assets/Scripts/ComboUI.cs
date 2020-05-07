using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboUI : MonoBehaviour
{

    [SerializeField] UnityEngine.UI.Text comboUI;

    /// <summary>
    /// Updates the combo count displayed
    /// </summary>
    /// <param name="count"></param>
    public void UpdateComboCount(int count)
    {
        comboUI.text = "Combo: " + count.ToString();
    }
}
