using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechDamageHandler : EnemyDamageHandler
{
    public bool AttackingPlayer
    {
        private set;
        get;
    } = false;

    public override void OnDamageTaken()
    {
        AttackPlayer();
    }

    public void AttackPlayer()
    {
        if (AttackingPlayer == false)
        {
            AttackingPlayer = true;
            GetComponent<MechAttackScript>().StartAttackingTarget(PlayerController.PlayerCamera.transform);
            AIRoamingController AIController = GetComponent<AIRoamingController>();
            if (AIController != null) AIController.SetRoamMode(AIRoamingController.RoamMode.RandomizedPathing);
        }
    }
}
