using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboRecordManager
{
    private const string PLAYER_PREFS_COMBO_BONUS_KEY = "COMBO_BONUS";

    /// <summary>
    /// The cached combo bonus retrieved (leave this at -1)
    /// </summary>
    private static int cachedComboBonus = -1;

    /// <summary>
    /// The player's combo bonus record
    /// </summary>
    public static int ComboBonusRecord
    {
        get
        {
            UpdateCachedRecordComboBonus();
            return cachedComboBonus;
        }
    }

    /// <summary>
    /// Called to notifiy the combo record manager of a new combo count. If the combo is greater than the player's previous record, then the player's best combo record will be set to this number 
    /// </summary>
    /// <param name="comboBonus"></param>
    public static void NewComboBonus(int comboBonus)
    {
        UpdateCachedRecordComboBonus();
        if(cachedComboBonus < comboBonus)
        {
            PlayerPrefs.SetInt(PLAYER_PREFS_COMBO_BONUS_KEY, comboBonus);
            cachedComboBonus = comboBonus;
        }
    }

    /// <summary>
    /// Updates the cached combo bonus reference (if required)
    /// </summary>
    private static void UpdateCachedRecordComboBonus()
    {
        // If the cached combo bonus is less than 0, then retrieve teh combo bonus player pref
        // Default is 0 if a record doesn't exist
        if (cachedComboBonus < 0)
        {
            cachedComboBonus = PlayerPrefs.GetInt(PLAYER_PREFS_COMBO_BONUS_KEY, 0);
        }
    }
}
