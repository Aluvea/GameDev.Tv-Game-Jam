using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMech : MonoBehaviour
{
    [SerializeField] float rotateSpeed;

    [Header("Legs Game Object References")]
    [SerializeField] EnemyHealth frontRightLegHealth;
    [SerializeField] EnemyHealth frontLeftLegHealth;
    [SerializeField] EnemyHealth rearRightLegHealth;
    [SerializeField] EnemyHealth rearLeftLegHealth;
    [SerializeField] Transform frontRightLegRoot;
    [SerializeField] Transform frontLeftLegRoot;
    [SerializeField] Transform rearRightLegRoot;
    [SerializeField] Transform rearLeftLegRoot;
    [SerializeField] Transform bodyReference;

    private void Start()
    {
        StartCoroutine(BossMechExecute());
        return;
        frontRightLegHealth.Died += OnLegDied;
        frontLeftLegHealth.Died += OnLegDied;
        rearRightLegHealth.Died += OnLegDied;
        rearLeftLegHealth.Died += OnLegDied;
    }

    private void OnLegDied(EnemyHealth legDead)
    {
        int deadLegCount = 0;
        if (frontRightLegHealth == null || frontRightLegHealth == legDead) deadLegCount++;
        if (frontLeftLegHealth == null || frontLeftLegHealth == legDead) deadLegCount++;
        if (rearRightLegRoot == null || rearRightLegRoot == legDead) deadLegCount++;
        if (rearLeftLegHealth == null || rearLeftLegHealth == legDead) deadLegCount++;

        if(deadLegCount == 4)
        {
            OnAllLegsDied();
        }
        else
        {

        }
    }

    private void OnAllLegsDied()
    {

    }


    IEnumerator BossMechExecute()
    {
        while (true)
        {
            Vector3 direction = PlayerController.PlayerCamera.transform.position - transform.position;
            direction.y = transform.position.y;

            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction, transform.up), rotateSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
