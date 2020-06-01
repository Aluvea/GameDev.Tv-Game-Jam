using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponShooting : MonoBehaviour
{

    [SerializeField] Camera FPCamera;
    [SerializeField] float range = 100f;
    private float damage;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] GameObject hitEffect;
    [SerializeField] LayerMask shootLayerMask;
    [SerializeField] AudioSource mAudioSource;
    [SerializeField] AudioClip[] clips;
    [Tooltip("Whether or not shooting always applies to the player's combo, even when an enemy target isn't hit from a bullet")]
    [SerializeField] bool shootingAlwaysAppliesToCombo = false;
    [SerializeField] AmmoManager ammoManager;

    // Start is called before the first frame update
    void Start()
    {
        mAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            // If the ammo manager isn't null, then check if we can shoot
            if(ammoManager != null)
            {
                // If we can't shoot, then just reload
                if (ammoManager.CanShoot == false)
                {
                    ammoManager.Reload();
                }
                // Otherwise, shoot a bullet
                else
                {
                    Shoot();
                }
            }
            // If the ammo manager is null, then just shoot
            else
            {
                
                Shoot();
            }
            
            
        }
    }

    private void Shoot()
    {
        ProcessRayCast();
    }

    private void PlayMuzzleFlash()
    {
        muzzleFlash.Play();
    }

    private void ProcessRayCast()
    {
        RaycastHit hit;
        if (Physics.Raycast(FPCamera.transform.position, FPCamera.transform.forward, out hit, range, shootLayerMask.value))
        {
            EnemyHealth target = hit.transform.GetComponent<EnemyHealth>();

            if (target == null)
            {
                if (shootingAlwaysAppliesToCombo) BeatSyncReceiver.BeatReceiver.RequestInputAction();
                CreateHitImpact(hit);
                PlayMuzzleFlash();
                PlayRandomClipFromArray(clips);
            }
            else if (BeatSyncReceiver.BeatReceiver.RequestInputAction(out BeatInputSync playerSync))
            {
                if (playerSync == BeatInputSync.PERFECT)
                {
                    damage = 3f;
                }
                else if (playerSync == BeatInputSync.GOOD)
                {
                    damage = 2f;
                }
                else if (playerSync == BeatInputSync.OK)
                {
                    damage = 1f;
                }
                else
                {
                    damage = 0f;
                }
                target.TakeDamage(damage);
                Debug.Log("Damage is " + damage + " on " + target.gameObject.name);
                CreateHitImpact(hit);
                PlayMuzzleFlash();
                PlayRandomClipFromArray(clips);
            }
            // If the ammo manager isn't null, then dispense ammo from the magazine clip
            if(ammoManager != null) ammoManager.DispenseAmmo();

        }
        else
        {
            return;
        }
    }

    private void CreateHitImpact(RaycastHit hit)
    {
        
        GameObject impact = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(impact, 1);
    }
    private void PlayRandomClipFromArray(AudioClip[] clips)
    {
        if (mAudioSource != null && clips != null)
        {
            if (clips.Length > 0)
            {
                mAudioSource.PlayOneShot(clips[UnityEngine.Random.Range(0, clips.Length)]);
            }
        }
    }
}
