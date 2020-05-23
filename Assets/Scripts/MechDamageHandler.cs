using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechDamageHandler : EnemyDamageHandler
{
    bool attackingPlayer = false;

    public override void OnDamageTaken()
    {
        if(attackingPlayer == false)
        {
            attackingPlayer = true;
            GetComponent<MechAttackScript>().StartAttackingTarget(PlayerController.PlayerCamera.transform);
        }
    }
}
