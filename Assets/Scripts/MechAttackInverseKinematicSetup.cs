using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DitzelGames.FastIK;

public class MechAttackInverseKinematicSetup : MonoBehaviour
{
    [SerializeField] FastIKFabric gunPointerIKReference;
    [SerializeField] FastIKFabric stareIKReference;
    [SerializeField] FastIKFabric leftHandIKReference;
    [SerializeField] FastIKFabric rightHandIKReference;



    private Transform targetRef;


    /// <summary>
    /// Sets up the mech's inverse kinematic target animation
    /// </summary>
    /// <param name="target"></param>
    public void SetupIKTargetAnimation(Transform target)
    {
        targetRef = target;

        rightHandIKReference.enabled = false;
        gunPointerIKReference.enabled = false;
        stareIKReference.enabled = false;
        leftHandIKReference.enabled = false;

        gunPointerIKReference.ChainLength = 1;
        stareIKReference.ChainLength = 1;
        leftHandIKReference.ChainLength = 4;
        rightHandIKReference.ChainLength = 4;

        gunPointerIKReference.Target = targetRef;
        stareIKReference.Target = targetRef;

        gunPointerIKReference.enabled = true;
        rightHandIKReference.enabled = true;
        leftHandIKReference.enabled = true;
        stareIKReference.enabled = true;
    }

    /// <summary>
    /// Stops the inverse kinematic target animation
    /// </summary>
    public void StopInverseKinematics()
    {
        gunPointerIKReference.enabled = false;
        stareIKReference.enabled = false;
        leftHandIKReference.enabled = false;
        rightHandIKReference.enabled = false;
    }
}
