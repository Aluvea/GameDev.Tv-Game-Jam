using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BossMechLegsManager : MonoBehaviour
{
    [SerializeField] float frontLegsMovementThreshold;

    [SerializeField] BossMechLegIK frontLeftLeg;
    [SerializeField] BossMechLegIK frontRightLeg;

    [SerializeField] float backLegsMovementThreshold;

    [SerializeField] BossMechLegIK backLeftLeg;
    [SerializeField] BossMechLegIK backRightLeg;

    [SerializeField] float legCheckFrequency;

    [SerializeField] BossMech bossMechManager;

    private void Start()
    {
        StartCoroutine(ManageLegs());
    }
    

    IEnumerator ManageLegs()
    {
        while (true)
        {
            yield return new WaitForSeconds(legCheckFrequency);
            if (bossMechManager.EmittingShockwaveAttack) continue;
            yield return MoveLegSet(frontLeftLeg, frontLegsMovementThreshold, backRightLeg, backLegsMovementThreshold);
            yield return MoveLegSet(frontRightLeg, frontLegsMovementThreshold, backLeftLeg, backLegsMovementThreshold);
        }
    }

    IEnumerator MoveLegSet(BossMechLegIK leg1, float leg1MoveThreshold, BossMechLegIK leg2, float leg2MoveThreshold)
    {
        float leg1Distance = leg1.DistanceFromRestingPosition;
        float leg2Distance = leg2.DistanceFromRestingPosition;
        if(leg1Distance >= leg1MoveThreshold)
        {
            if(leg1.IsMoving == false)
            {
                leg1.StartMoveLegCoroutine();
            }
        }
        if(leg2Distance >= leg2MoveThreshold)
        {
            if(leg2.IsMoving == false)
            {
                leg2.StartMoveLegCoroutine();
            }
        }

        yield return null;

        while(leg1.IsMoving && leg2.IsMoving)
        {
            yield return null;
        }
    }

    /// <summary>
    /// Returns whether any legs are currently moving
    /// </summary>
    public bool AreAnyLegsMoving
    {
        get
        {
            return frontLeftLeg.IsMoving && frontRightLeg.IsMoving && backLeftLeg.IsMoving && backRightLeg.IsMoving;
        }
    }
    

}
