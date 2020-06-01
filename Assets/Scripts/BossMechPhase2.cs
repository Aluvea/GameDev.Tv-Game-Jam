using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMechPhase2 : MonoBehaviour
{
    [Header("Game Object References")]
    [SerializeField] BossEyeTarget eyeTargetter;
    [SerializeField] BoxCollider stunnedBoxCollider;
    [SerializeField] CapsuleCollider hitBoxCollider;
    [SerializeField] EnemyHealth healthRef;
    [SerializeField] BossMeshMaterialManager matManager;
    [Header("Roll Build Up Settings")]
    [Tooltip("The rotation power towards the player (while the mech is targetting the player)")]
    [SerializeField] float rotateSpeed = 145;
    [Tooltip("The charging build-up time")]
    [SerializeField] float buildUpTime;
    [Header("Roll Attack Settings")]
    [Tooltip("The rolling charge attack movement speed")]
    [Min(1.0f)]
    [SerializeField] float movementSpeed = 1.0f;
    [Tooltip("The rolling attack rotation speed (ball-like rotation)")]
    [SerializeField] float rollRotationSpeed = 145;
    [Tooltip("The maximum power that the mech rotates towards at player while its charging")]
    [SerializeField] float maxRotatePowerTowardsPlayerSpeed = 25.0f;
    [Tooltip("The minimum power that the mech rotates towards at player while its charging")]
    [SerializeField] float minRotatePowerTowardsPlayerSpeed = 25.0f;
    [Tooltip("The distance the mech needs to travel while charging to rotate at the player at the minimum rotation power")]
    [SerializeField] float distanceForMinRotationPower = 9.0f;
    [Tooltip("The charge damage")]
    [SerializeField] float chargeDamage = 5.0f;
    [Header("Collision Detection Settings")]
    [Tooltip("The layer mask of the player")]
    [SerializeField] LayerMask playerLayerMask;
    [Tooltip("The player collision game object reference (used for the player collision hitbox position and size)")]
    [SerializeField] Transform playerCollisionRef;
    [Tooltip("The layer mask of the walls")]
    [SerializeField] LayerMask wallsLayerMask;
    [Tooltip("The walls collision game object reference (used for the walls collision hitbox position and size)")]
    [SerializeField] Transform wallsCollisionRef;
    [Tooltip("How much the mech should recoil after charging into an object")]
    [Min(1.0f)]
    [SerializeField] float collisionPushBack;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] crashAudioClips;
    [Header("Stun Settings")]
    [Tooltip("How long the mech should be stunned after it hits an object")]
    [Min(3.0f)]
    [SerializeField] float stunDuration = 3.0f;
    [SerializeField] ParticleSystem [] dizzyParticles;
    [Header("Death Game Object References")]
    [SerializeField] SphereCollider bodySphereCollider;
    [SerializeField] CapsuleCollider eyeCollider;
    [SerializeField] AudioClip deathAudioClip;
    [SerializeField] AudioClip explosionAudioClip;
    [SerializeField] ParticleSystem[] explosionParticles;

    [Header("Test Phase Settings")]
    [SerializeField] public bool testPhase2;
    [SerializeField] EnemyHealth[] EnemyHealthToDestroyUponTesting;

    private Transform bodyTransformRef;

    private void Awake()
    {
        bodyTransformRef = transform.Find("Body");
        healthRef.Died += OnMechDied;
    }

    private void OnMechDied(EnemyHealth deadEnemy)
    {
        mechDied = true;
    }

    private bool mechDied = false;

    private void Start()
    {
        if (testPhase2)
        {
            StartCoroutine(TestMech());
        }
    }

    IEnumerator TestMech()
    {
        yield return new WaitForSeconds(1.75f);
        yield return new WaitForEndOfFrame();
        foreach (EnemyHealth health in EnemyHealthToDestroyUponTesting)
        {
            health.TakeDamage(Mathf.Infinity);
            yield return new WaitForSeconds(1.75f);
        }
    }


    public void StartPhase2Battle()
    {
        SetupMechBody();
        StartCoroutine(AttackPlayer());
    }

    Rigidbody rigidBodyRef;

    private void SetupMechBody()
    {
        // Remove this game object from its parent
        transform.parent = null;
        stunnedBoxCollider.gameObject.SetActive(true);
        stunnedBoxCollider.enabled = true;
        hitBoxCollider.gameObject.SetActive(true);
        hitBoxCollider.enabled = true;
        

        // Enable the sphere collider on this game object
        this.transform.GetComponent<SphereCollider>().enabled = false;
        return;
        // Add a rigid body to this game object
        rigidBodyRef = this.transform.gameObject.AddComponent<Rigidbody>();
        // Set this rigidbody reference to is kinematic
        rigidBodyRef.isKinematic = true;
    }

    IEnumerator AttackPlayer()
    {
        // Reset the body rotation to its root
        yield return ResetBodyStareToRoot(1.0f);

        while (mechDied == false)
        {
            matManager.LerpEmissionToColor(Color.red);
            // Rotate the body at player
            yield return RotateAtPlayer();
            // Build up the rotation charge
            yield return BuildUpCharge();
            // Charge
            yield return Charge();
        }
        OnBossDefeated();
    }

    private void OnBossDefeated()
    {
        audioSource.PlayOneShot(deathAudioClip);
        matManager.LerpEmissionToColor(Color.black);
        Debug.LogWarning("Boss Defeated!");
        bodySphereCollider.enabled = true;
        bodySphereCollider.gameObject.AddComponent<Rigidbody>().AddForce(Vector3.up * 8.0f);
        eyeCollider.transform.parent = null;
        eyeCollider.enabled = true;
        eyeCollider.gameObject.AddComponent<Rigidbody>().AddForce(Vector3.up * 8.0f);
        stunnedBoxCollider.gameObject.SetActive(false);
        hitBoxCollider.gameObject.SetActive(false);
        
        for (int i = 0; i < explosionParticles.Length; i++)
        {
            if(i == 0)
            {
                explosionParticles[i].transform.parent.parent = null;
            }
            explosionParticles[i].Play();
        }
        audioSource.PlayOneShot(explosionAudioClip);

        // End the game?
    }
    

    /// <summary>
    /// Simply rotates the mech towards the player, enumeration ends when the mech is facing the player
    /// </summary>
    /// <returns></returns>
    IEnumerator RotateAtPlayer()
    {
        // Rotate towards the player
        Vector3 direction = PlayerController.PlayerCamera.transform.position - transform.position;
        direction.y = transform.position.y;
        while (Vector3.Angle(direction, transform.forward) > Mathf.Epsilon)
        {
            Debug.LogWarning("Rotating towards player");

            direction = PlayerController.PlayerCamera.transform.position - transform.position;
            direction.y = transform.position.y;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction, Vector3.up), rotateSpeed * Time.deltaTime);
            yield return null;
        }
    }

    /// <summary>
    /// Coroutine used to build up the charge attack (rotates like a ball and faces the player)
    /// </summary>
    /// <returns></returns>
    IEnumerator BuildUpCharge()
    {
        // Toggle off teh eye stare
        eyeTargetter.ToggleStare(false);
        // keep staring at the player
        // build up a rotation 
        Vector3 direction = PlayerController.PlayerCamera.transform.position - transform.position;
        direction.y = 0.0f;
        float startTime = Time.time;
        float lerpAMT = 0.0f;
        float rotationSpeed = 0.0f;
        Vector3 localEulerAngle = bodyTransformRef.localEulerAngles;
        while (lerpAMT < 1.0f)
        {
            Debug.LogWarning("Building up charge");
            lerpAMT = (Time.time - startTime) / buildUpTime;
            // Keep staring at the player
            direction = PlayerController.PlayerCamera.transform.position - transform.position;
            direction.y = 0.0f;
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            // Start building up the charging roll
            rotationSpeed = Mathf.Lerp(0.0f, rollRotationSpeed, lerpAMT);
            localEulerAngle.x += rotationSpeed * Time.deltaTime;
            if (localEulerAngle.x >= 360.0f) localEulerAngle.x = localEulerAngle.x % 360.0f;
            bodyTransformRef.localEulerAngles = localEulerAngle;
            yield return null;
        }

    }

    /// <summary>
    /// Coroutine used to charge at the player
    /// </summary>
    /// <returns></returns>
    IEnumerator Charge()
    {
        Vector3 localEulerAngle = bodyTransformRef.localEulerAngles;
        Vector3 direction;
        float rotateAtPlayer = maxRotatePowerTowardsPlayerSpeed;
        Vector3 startingPosition = transform.position;
        float distanceFromStartPosition = 0.0f;
        // While the boss hasn't hit something, keep charging
        while (CollidedAgainstSomething() == false)
        {
            // Keep rotating like a ball
            Debug.LogWarning("Charging");
            localEulerAngle.x += rollRotationSpeed * Time.deltaTime;
            if (localEulerAngle.x >= 360.0f) localEulerAngle.x = localEulerAngle.x % 360.0f;
            bodyTransformRef.localEulerAngles = localEulerAngle;
            direction = PlayerController.PlayerCamera.transform.position;
            direction.y = transform.position.y;
            // Slightly rotate at the player
            direction = direction - transform.position;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction, transform.up), rotateAtPlayer * Time.deltaTime);
            // Charge at the player
            transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, movementSpeed * Time.deltaTime);

            yield return null;
            // Decline the rotation power to the player based on how far the mech has rolled
            if (distanceFromStartPosition >= distanceForMinRotationPower)
            {
                rotateAtPlayer = minRotatePowerTowardsPlayerSpeed;
            }
            else
            {
                // Recalculate the distance from the starting position
                distanceFromStartPosition = Vector3.Distance(transform.position, startingPosition);
                // Interpolate the rotation power based on the disttance traveled (from the max to min rotation power)
                rotateAtPlayer = Mathf.Lerp(maxRotatePowerTowardsPlayerSpeed, minRotatePowerTowardsPlayerSpeed, distanceFromStartPosition / distanceForMinRotationPower);
            }
        }
        matManager.LerpToOriginalColor();
        PlayCrashAudioClip();
        // Simulate a collision recoil (pushback)
        StartCoroutine(PushBack(0.9f));
        ToggleDizzeParticles(true);
        // Reset the body stare to its native rotation
        yield return ResetBodyStareToRoot(1.0f);
        Debug.Log("Stunned!");
        yield return new WaitForSeconds(stunDuration);
        ToggleDizzeParticles(false);
    }

    /// <summary>
    /// Simulates a collision pushback
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    IEnumerator PushBack(float duration)
    {
        // Cache the startime of the pushback
        float startTime = Time.time;
        // Get the pushback direction
        Vector3 pushBackDirection = -transform.forward;
        // If a wall is to the right or left of the mech, then offset the pushback direction in
        // the opposite direction
        if (WallInDirection(transform.right)) pushBackDirection -= transform.right;
        else if (WallInDirection(-transform.right)) pushBackDirection += transform.right;
        // Multiply the pushback amount by the collision pushback modifier amount
        pushBackDirection *= collisionPushBack;
        // Cache the pushback position
        Vector3 pushBackPosition = transform.position + pushBackDirection;
        // If the pushback position is blocked, then do nothing
        if(IsPushBackPositionBlocked(pushBackPosition)) yield break;
        // Otherwise, move towards the pushback position
        while (Time.time <= startTime + duration)
        {
            transform.position = Vector3.MoveTowards(transform.position, pushBackPosition, movementSpeed * Time.deltaTime);
            yield return null;
        }
    }
    /// <summary>
    /// Returns whether or not the given pushback position is blocked by a wall obstacle
    /// </summary>
    /// <param name="desiredPushBackPosition">The desired pushback postion</param>
    /// <returns></returns>
    private bool IsPushBackPositionBlocked(Vector3 desiredPushBackPosition)
    {
        // Get the direction of the pushback postion from the current postion
        Vector3 pushbackDirection = desiredPushBackPosition - transform.position;
        // Create a ray
        Ray pushBackRay = new Ray(transform.position, pushbackDirection);
        // Raycast to the pushback postion
        return Physics.Raycast(pushBackRay, Vector3.Distance(transform.position, desiredPushBackPosition), wallsLayerMask);
    }
    /// <summary>
    /// Returns whether or not a wall is in the given direction from the mech
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    private bool WallInDirection(Vector3 direction)
    {
        Ray ray = new Ray(wallsCollisionRef.position, direction);
        return Physics.Raycast(ray, wallsCollisionRef.lossyScale.x, wallsLayerMask);
    }

    /// <summary>
    /// Resets the body rotation to its resting / native rotation
    /// </summary>
    /// <param name="resetRotationDuration"></param>
    /// <returns></returns>
    IEnumerator ResetBodyStareToRoot(float resetRotationDuration)
    {
        BossEyeTarget eyeTarget = bodyTransformRef.GetComponent<BossEyeTarget>();
        eyeTarget.ToggleStare(false);
        yield return null;
        float startTimestamp = Time.time;
        float duration = resetRotationDuration;
        float lerpAMt = 0.0f;
        Quaternion startingRotation = eyeTarget.transform.localRotation;
        while(lerpAMt < 1.0f)
        {
            Debug.LogWarning("Reseting body to root rotation");
            lerpAMt = (Time.time - startTimestamp) / duration;
            eyeTarget.transform.localRotation = Quaternion.Lerp(startingRotation, Quaternion.Euler(Vector3.zero), lerpAMt);
            yield return null;
        }
        eyeTarget.transform.localEulerAngles = Vector3.zero;
        yield return null;
    }

    bool hitSomething = false;

    /// <summary>
    /// Returns true if the mech collided against something
    /// </summary>
    /// <returns></returns>
    private bool CollidedAgainstSomething()
    {

        // Check if we hit the player with the player collision check settings
        hitSomething = Physics.CheckBox(playerCollisionRef.position, playerCollisionRef.lossyScale / 2.0f, transform.rotation, playerLayerMask.value);
        // If we hit the player, then get the player health component attached
        if (hitSomething)
        {
            Collider[] hitColliders = Physics.OverlapBox(playerCollisionRef.position, playerCollisionRef.lossyScale / 2.0f, transform.rotation, playerLayerMask.value);
            PlayerHealth playerHealth = null;
            foreach (Collider collider in hitColliders)
            {
                playerHealth = collider.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(chargeDamage);
                    return true;
                }
            }
        }
        hitSomething = Physics.CheckBox(wallsCollisionRef.position, wallsCollisionRef.lossyScale / 2.0f, transform.rotation, wallsLayerMask.value);
        if (hitSomething == false) return false;

        // Iterate through all the wall colliders hit
        Collider[] wallColliders = Physics.OverlapBox(wallsCollisionRef.position, wallsCollisionRef.lossyScale / 2.0f, transform.rotation, wallsLayerMask.value);
        PillarScript pillarScript = null;
        foreach (Collider collider in wallColliders)
        {
            pillarScript = collider.GetComponent<PillarScript>();
            if (pillarScript != null)
            {
                pillarScript.DestructPillars();
            }
        }
        // Otherwise, just check if a wall has been hit with the wall collision check settings
        return true;
    }

    private void ToggleDizzeParticles(bool toggled)
    {
        for (int i = 0; i < dizzyParticles.Length; i++)
        {
            if(dizzyParticles[i] != null)
            {
                if (toggled)
                {
                    dizzyParticles[i].Play();
                }
                else
                {
                    dizzyParticles[i].Stop();
                    if(i == 0)
                    {
                        dizzyParticles[i].Clear();
                    }
                }
            }
        }
    }


    private void PlayCrashAudioClip()
    {
        if(audioSource != null && crashAudioClips.Length > 0)
        {
            audioSource.PlayOneShot(crashAudioClips[Random.Range(0, crashAudioClips.Length)]);
        }
    }
}
