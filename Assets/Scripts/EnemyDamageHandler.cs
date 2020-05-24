using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base Class Used for Enemies To Handle When They Receive Damage
/// </summary>
public abstract class EnemyDamageHandler : MonoBehaviour
{
    /// <summary>
    /// Method to override when an enemy takes damage
    /// </summary>
    public abstract void OnDamageTaken();
}
