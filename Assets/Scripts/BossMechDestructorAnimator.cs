using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMechDestructorAnimator : MonoBehaviour
{

    [SerializeField] Transform bodyReference;
    [SerializeField] BossMechBodyDecimator frontRightLegRoot;
    [SerializeField] BossMechBodyDecimator frontLeftLegRoot;
    [SerializeField] BossMechBodyDecimator rearRightLegRoot;
    [SerializeField] BossMechBodyDecimator rearLeftLegRoot;
    [Min(1.0f)]
    [SerializeField] float lastLegOffsetModifier = 1.0f;
    [SerializeField] BossEyeTarget bodyTarget;
    private float distanceFromGround;
    private int deadLegCount = 0;

    private float legDistance;

    private void Awake()
    {
        distanceFromGround = bodyReference.localPosition.y;
        legDistance = transform.InverseTransformPoint(frontLeftLegRoot.transform.position).magnitude;
    }

    [SerializeField] float moveDuration = 1.0f;
    
    public void LegDied()
    {
        deadLegCount++;
        AnimateBody();
    }

    IEnumerator MoveBodyToLocalPosition(Vector3 localPosition)
    {
        float startTime = Time.time;
        Vector3 startingPosition = bodyReference.transform.localPosition;

        float lerpAMT = 0.0f;
        while(lerpAMT < 1.0f)
        {
            lerpAMT = (Time.time - startTime) / moveDuration;
            bodyReference.transform.localPosition = Vector3.Lerp(startingPosition, localPosition, lerpAMT);
            MoveLegRoots();
            yield return null;

        }

        bodyReference.transform.localPosition = localPosition;
        MoveLegRoots();
        yield return null;
    }

    private void AnimateBody()
    {
        if (deadLegCount >= 4)
        {
            bodyTarget.ToggleStare(false);
            return;
        }


        Vector3 animatedVector = Vector3.zero;
        if(deadLegCount < 3)
        {
            animatedVector.y = distanceFromGround - ((distanceFromGround /3)  * deadLegCount);
        }
        if(deadLegCount == 3)
        {
            animatedVector += GetLocalVectorOfRemainingLeg();
        }

        StartCoroutine(MoveBodyToLocalPosition(animatedVector));
    }

    private Vector3 GetLocalVectorOfRemainingLeg()
    {
        Vector3 toReturn = Vector3.zero;

        if (frontRightLegRoot.Decimated == false || frontLeftLegRoot.Decimated == false)
        {

            return toReturn = Vector3.zero;
        }
        if (rearRightLegRoot.Decimated == false)
        {
            toReturn = Vector3.left;
        } 
        if (rearLeftLegRoot.Decimated == false)
        {
            toReturn = Vector3.right;
        }
        toReturn *= lastLegOffsetModifier * legDistance;
        bodyTarget.ToggleStare(true);
        return toReturn;
    }
    
    private void MoveLegRoots()
    {
        MaintainLegDistance(frontRightLegRoot);
        MaintainLegDistance(frontLeftLegRoot);
        MaintainLegDistance(rearRightLegRoot);
        MaintainLegDistance(rearLeftLegRoot);
    }

    private void MaintainLegDistance(BossMechBodyDecimator legRef)
    {
        if (legRef.Decimated) return;

        Vector3 inversedPoint = transform.InverseTransformPoint(legRef.transform.position);
        float magnitudeOffset = legDistance / inversedPoint.magnitude;
        inversedPoint = inversedPoint * (magnitudeOffset);
        legRef.transform.position = transform.TransformPoint(inversedPoint);

    }
}
