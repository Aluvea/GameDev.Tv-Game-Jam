using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEyeTarget : MonoBehaviour
{

    [SerializeField] bool testStare = false;

    [SerializeField] bool freezeXRotation;
    [SerializeField] bool freezeYRotation;
    [SerializeField] bool freezeZRotation;


    Vector3 originalLocalEuler;

    private void Start()
    {
        originalLocalEuler = transform.localEulerAngles;
        if (testStare)
        {
            ToggleStare(testStare);
        }
    }

    private bool staring = false;

    public void ToggleStare(bool toggle)
    {
        if(toggle != staring)
        {
            staring = toggle;
            StartCoroutine(StareAtPlayer());
        }
    }
    
    private IEnumerator StareAtPlayer()
    {
        while (staring)
        {
            transform.forward = GetPositionToLookAt();
            yield return null;
        }
    }

    Vector3 DirectionToLookAt;

    private Vector3 GetPositionToLookAt()
    {
        DirectionToLookAt = PlayerController.PlayerCamera.transform.position - transform.position;
        DirectionToLookAt.Normalize();
        if (freezeXRotation)
        {
            DirectionToLookAt.x = transform.forward.x;
        }
        if (freezeYRotation)
        {
            DirectionToLookAt.y = transform.forward.y;
        }
        if (freezeZRotation)
        {
            DirectionToLookAt.z = transform.forward.z;
        }

        return DirectionToLookAt;
    }


    
}
