using System.Collections;
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
    [SerializeField] ParticleSystem []destructionParticlesToEnable;
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
            GameObject bodyPart = gameObjectsThatRequireRigidBodies[i];
            if(bodyPart != null) bodyPart.AddComponent<Rigidbody>().AddForce(Vector3.up * 0.1f);
        }

        Decimated = true;
        PlayDecimationClip();
        PlayParticleSystem();
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
    private void PlayParticleSystem()
    {
        if(destructionParticlesToEnable.Length > 0)
        {
            foreach (ParticleSystem particle in destructionParticlesToEnable)
            {
                if (particle != null) particle.Play(true);
            }
        }
    }
}
