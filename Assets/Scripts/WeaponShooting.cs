using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponShooting : MonoBehaviour
{

    [SerializeField] Camera FPCamera;
    [SerializeField] float range = 100f;

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
        RaycastHit hit;
        Physics.Raycast(FPCamera.transform.position, FPCamera.transform.forward, out hit, range);
        Debug.Log("I hit the " + hit.transform.name + "!");
    }
}
