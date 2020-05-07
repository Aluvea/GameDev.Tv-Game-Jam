using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponShooting : MonoBehaviour
{

    [SerializeField] Camera FPCamera;
    [SerializeField] float range = 100f;
    // Damage will be dictated by the accuracy of landing the hit on beat as determined in a different script by ForeverAFK
    [SerializeField] float damage = 30f;
    [SerializeField] ParticleSystem muzzleFlash;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (CanShoot())
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
    }

    bool CanShoot()
    {
        return true;
    }

    private void Shoot()
    {
        PlayMuzzleFlash();
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
            Debug.Log("I hit the " + hit.transform.name + "!");
            // TO DO: Add some hit effect for visual players

            EnemyHealth target = hit.transform.GetComponent<EnemyHealth>();
            // Call a method on enemy health that decreases the enemy health
            if (target == null) return;
            target.TakeDamage(damage);
        }

        else
        {
            return;
        }
    }
}
