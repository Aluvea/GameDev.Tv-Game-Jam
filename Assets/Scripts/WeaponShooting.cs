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
    private AudioSource mAudioSource;

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
            Shoot();
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
        if (Physics.Raycast(FPCamera.transform.position, FPCamera.transform.forward, out hit, range))
        {
            EnemyHealth target = hit.transform.GetComponent<EnemyHealth>();

            if (target == null)
            {
                CreateHitImpact(hit);
                PlayMuzzleFlash();
                mAudioSource.Play();
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
                mAudioSource.Play();
            }
            
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
}
