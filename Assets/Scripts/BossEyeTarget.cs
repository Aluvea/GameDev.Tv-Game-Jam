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

    private void Awake()
    {
        originalLocalEuler = transform.localEulerAngles;
    }

    private void Start()
    {
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
            FixRotation();
            yield return null;
        }
    }

    Vector3 DirectionToLookAt;

    private Vector3 GetPositionToLookAt()
    {
        DirectionToLookAt = PlayerController.PlayerCamera.transform.position - transform.position;
        DirectionToLookAt.Normalize();

        return DirectionToLookAt;
    }

    private void FixRotation()
    {
        DirectionToLookAt = transform.localEulerAngles;

        if (freezeXRotation)
        {
            DirectionToLookAt.x = originalLocalEuler.x;
        }
        if (freezeYRotation)
        {
            DirectionToLookAt.y = originalLocalEuler.y;
        }
        if (freezeZRotation)
        {
            DirectionToLookAt.z = originalLocalEuler.z;
        }
        transform.localEulerAngles = DirectionToLookAt;
    }


    
}
