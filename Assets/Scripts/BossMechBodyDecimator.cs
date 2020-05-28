﻿using System.Collections;
using System.Collections.Generic;
using Animations;
using UnityEngine;

public class BossMechBodyDecimator : Animations.AnimationController, Animations.IPlayDeathAnimation
{
    [SerializeField] DitzelGames.FastIK.FastIKFabric mechLegIK;
    [SerializeField] BossEyeTarget eyeTarget;
    [SerializeField] BossCannon bossCannon;


    [SerializeField] GameObject [] gameObjectsThatRequireRigidBodies;
    [SerializeField] Transform objectToDeroot;
    [SerializeField] AudioClip[] clips;
    [SerializeField] AudioSource audiouSource;

    [SerializeField] bool testDecimator;

    public bool Decimated
    {
        private set;
        get;
    } = false;

    // Start is called before the first frame update
    void Start()
    {
        if (testDecimator)
        {
            DecimateBodyPart();
        }
    }

    public void DecimateBodyPart()
    {

        if(mechLegIK != null)
        {
            mechLegIK.ChainLength = 0;
            mechLegIK.enabled = false;
        }
        if(bossCannon != null)
        {
            Destroy(bossCannon);
        }
        if(eyeTarget != null)
        {
            Destroy(eyeTarget);
        }

        objectToDeroot.parent = null;

        for (int i = 0; i < gameObjectsThatRequireRigidBodies.Length; i++)
        {
            gameObjectsThatRequireRigidBodies[i].AddComponent<Rigidbody>().AddForce(Vector3.up * 0.1f);
        }

        Decimated = true;
        PlayDecimationClip();
    }
    

    public void PlayDeathAnimation()
    {
        DecimateBodyPart();
    }

    private void PlayDecimationClip()
    {
        if(audiouSource != null && clips.Length > 0)
        {
            audiouSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);
        }
    }
}
