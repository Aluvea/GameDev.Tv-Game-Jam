using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathfExtensions;

public class BossMechLegIK : MonoBehaviour
{
    [SerializeField] DitzelGames.FastIK.FastIKFabric mechLegReference;
    [SerializeField] Transform mechRootReference;

    [Header("Leg Movement Settings")]
    [SerializeField] float legMovementDuration = 1.0f;
    [SerializeField] float legMovementHeight = 1.0f;
    [SerializeField] float forwardOffset = 0.0f;
    [SerializeField] float directionInfluenceMagnitude = 0.0f;
    [Header("Audio Clip Settings")]
    [SerializeField] AudioSource legStompAudioSource;
    [SerializeField] AudioClip [] legStompAudioClips;

    /// <summary>
    /// The native resting local positin of this leg to its root body
    /// </summary>
    private Vector3 restingLegLocalPosition;

    private Vector3 lastOffSetPosition;

    private void Awake()
    {
        // Set this transform position to the leg's reference position
        transform.position = mechLegReference.transform.position;
        // Get the native offset postiion from the body to the 
        restingLegLocalPosition = mechRootReference.transform.InverseTransformPoint(mechLegReference.transform.position);
        restingLegLocalPosition.z += forwardOffset;
        lastOffSetPosition = restingLegLocalPosition;
    }
    

    public bool IsMoving
    {
        private set;
        get;
    } = false;


    public void StartMoveLegCoroutine()
    {
        IsMoving = true;
        StartCoroutine(MoveLegToTargetPositionCoroutine());
    }

    IEnumerator MoveLegToTargetPositionCoroutine()
    {
        IsMoving = true;
        Vector3 targetPosition;
        float lerpAMT = 0.0f;
        float startimeTimestamp;
        // Cache the initial starting position relative to the root
        Vector3 initialStartLegMovementPosition = transform.position;
        Vector3 updatedRestingTargetPosition = mechRootReference.TransformPoint(restingLegLocalPosition);
        Vector3 directionInfluence = updatedRestingTargetPosition - transform.position;

        directionInfluence.Normalize();

        transform.forward = directionInfluence;

        // Set the lerp amount to 0
        lerpAMT = 0.0f;
        // Cache the starting timestamp
        startimeTimestamp = Time.time;
        // While the lerp amount is less than or equal to 1.0f, move the leg towards the target postion
        while (lerpAMT <= 1.0f)
        {
            // Lerp amount is the runtime of the movement / the assign movement duration
            lerpAMT = (Time.time - startimeTimestamp) / legMovementDuration;
            updatedRestingTargetPosition = mechRootReference.TransformPoint(restingLegLocalPosition);

            directionInfluence = updatedRestingTargetPosition - initialStartLegMovementPosition;
            directionInfluence.Normalize();
            updatedRestingTargetPosition += directionInfluence * directionInfluenceMagnitude;

            // If the lerp amount is more than or equal to 1.0f, set it to 1.0f
            if (lerpAMT >= 1.0f)
            {
                PlayLegStompAudioClip();
                lerpAMT = 1.0f;
            } 
            // Reevaluate the target position from the root, interpolate the initial starting position towards the resting position with the lerp amount
            //targetPosition = mechRootReference.transform.TransformPoint(Vector3.Lerp(initialStartLegMovementPosition, restingLegLocalPosition, lerpAMT));
            targetPosition = Vector3.Lerp(initialStartLegMovementPosition, updatedRestingTargetPosition, lerpAMT);
            // Oscillate the leg height by the lerp amount
            targetPosition.y += MathfExt.Oscillate(0.0f, legMovementHeight, lerpAMT);
            // Set this leg's postion to the target position
            transform.position = targetPosition;
            yield return null;
            
            // If the lerp amount is 1.0f, then break out of the loop
            if (lerpAMT == 1.0f) break;
        }
        

        transform.position = updatedRestingTargetPosition;

        lastOffSetPosition = mechRootReference.InverseTransformPoint(updatedRestingTargetPosition);

        IsMoving = false;
    }

    public float DistanceFromRestingPosition
    {
        get
        {
            return Vector3.Distance(mechRootReference.transform.InverseTransformPoint(transform.position), lastOffSetPosition);
        }
    }


    private void PlayLegStompAudioClip()
    {
        if(legStompAudioSource != null && legStompAudioClips.Length > 0)
        {
            if(mechLegReference.enabled)
            legStompAudioSource.PlayOneShot(legStompAudioClips[Random.Range(0, legStompAudioClips.Length)]);
        }
    }

    


}
