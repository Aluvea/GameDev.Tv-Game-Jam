using System.Collections;
using UnityEngine;

public class BossMech : MonoBehaviour
{
    [SerializeField] float rotateSpeed;

    [Header("Legs Game Object References")]
    [SerializeField] EnemyHealth frontRightLegHealth;
    [SerializeField] EnemyHealth frontLeftLegHealth;
    [SerializeField] EnemyHealth rearRightLegHealth;
    [SerializeField] EnemyHealth rearLeftLegHealth;
    [SerializeField] BossMechDestructorAnimator destructorAnimator;
    
    [SerializeField] BossCannon cannonL;
    [SerializeField] BossCannon cannonR;
    [Header("Attack Settings")]
    [SerializeField] float attackFrequency = 5.0f;
    [SerializeField] float attackBulletDamage = 1.4f;
    [Header("Bullet Barrage Settings")]
    [Tooltip("How many bullets should be shot in each wave of bullet barages")]
    [SerializeField] int bulletBarageCount = 15;
    [Tooltip("How many bullets should hit the player with each wave of bullet barages")]
    [SerializeField] int maxBulletsToHitPlayerInBarage = 4;
    [Min(0.01f)]
    [Tooltip("How quickly the bullets should be shot from each other in a bullet barage wave")]
    [SerializeField] float bulletBarageShotFrequency = 0.08f;
    [Tooltip("The maximum seconds randomized and added to the bullet barage shot frequency (to make it look / feel more authentic)")]
    [Range(0.0f, 5.0f)]
    [SerializeField] float bulletBarageShotFrequencySalt = 0.08f;

    

    private void Start()
    {
        shockWaveParticles.SetShockwaveDamage(shockWaveDamage);
        StartCoroutine(BossMechExecute());
        frontRightLegHealth.Died += OnLegDied;
        frontLeftLegHealth.Died += OnLegDied;
        rearRightLegHealth.Died += OnLegDied;
        rearLeftLegHealth.Died += OnLegDied;
        cannonL.GetComponent<EnemyHealth>().Died += OnCannonDied;
        cannonR.GetComponent<EnemyHealth>().Died += OnCannonDied;
        StartCoroutine(Attack());
    }


    int deadLegCount = 0;


    private void OnLegDied(EnemyHealth legDead)
    {
        deadLegCount++;
        destructorAnimator.LegDied();

        if (deadLegCount == 4)
        {
            OnAllLegsDied();
        }
    }

    private int cannonsDied = 0;

    private void OnCannonDied(EnemyHealth cannonDied)
    {
        cannonsDied++;
        if (cannonsDied == 2) OnAllCannonsDied();
    }

    private bool allLegsDied = false;

    private void OnAllLegsDied()
    {
        GetComponent<AIRoamingController>().SetRoam(false);
        GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        allLegsDied = true;
    }

    private bool allCannonsDied = false;

    private void OnAllCannonsDied()
    {
        GetComponent<AIRoamingController>().SetRoam(false);
        GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        allCannonsDied = true;
    }

    IEnumerator Attack()
    {
        bool shouldShockWaveAttack;
        while (allLegsDied == false || allCannonsDied == false)
        {
            shouldShockWaveAttack = ShouldTargettedShockwave();
            if (shouldShockWaveAttack)
            {
                yield return TargettedShockwaveAttack();
            }
            else
            {
                yield return new WaitForSeconds(allCannonsDied == false ? attackFrequency : 0.0f);
                if (allCannonsDied == false)
                {
                    yield return FireCannons();
                }
                else if (allLegsDied == false)
                {
                    yield return ShockwaveAttack();
                }
            }
        }
        LegsAndCannonsDied();
    }

    private void LegsAndCannonsDied()
    {
        Debug.LogWarning("LEGS AND CANNON DESTROYED!");
        Debug.LogWarning("Entering Phase 2 Attack Mode!");
        StopAllCoroutines();
        phaseTwoRef.StartPhase2Battle();
    }

    public bool EmittingShockwaveAttack
    {
        private set;
        get;
    } = false;
    [Header("Shockwave Settings")]
    [Min(3.0f)]
    [SerializeField] float distanceFromPlayerToAttackWithShockwave = 5.0f;
    [SerializeField] BossMechShockwave shockWaveParticles;
    [SerializeField] float shockWaveDamage = 1.0f;
    [SerializeField] float shockwaveJumpHeight;
    [SerializeField] float shockwaveJumpDuration;
    [SerializeField] float shockwaveWarningTime;
    [SerializeField] float shockwaveWarningLowerHeight;
    [SerializeField] AudioClip[] shockWaveAudioClips;
    [SerializeField] AudioSource shockWaveAudioSource;
    [Header("Targetted Beat Map Shockwave Settings")]
    [Min(0)]
    [SerializeField] int shockWaveAttackPrepBeatMapIndex;
    [Range(1, 5)]
    [SerializeField] int shockWaveAttackPrepBeatMapCount;
    [SerializeField] int shockWaveAttackBeatMapTargetIndex;
    [Header("OPERATION PHASE TWO")]
    [SerializeField] BossMechPhase2 phaseTwoRef;

    IEnumerator ShockwaveAttack()
    {
        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        CharacterMovementController movementController = GetComponent<CharacterMovementController>();
        Debug.LogWarning("Shockwave attack!");

        if (agent.enabled == false)
        {
            yield return null;
            agent.enabled = true;
        }

        // Walk towards the player
        // Get close enough
        while (Vector3.Distance(transform.position, PlayerController.PlayerCamera.transform.position) > distanceFromPlayerToAttackWithShockwave && allLegsDied == false)
        {
            agent.SetDestination(PlayerController.PlayerCamera.transform.position);
            movementController.MoveCharacter(CharacterMovementController.AgentVelocityToVector2DInput(agent));
            yield return null;
            if (ShouldTargettedShockwave()) yield break;
        }

        
        // Jump and emmit shockwave attack
        EmittingShockwaveAttack = true;
        
        agent.enabled = false;
        movementController.MoveCharacter(Vector2.zero);
        movementController.enabled = false;
        yield return null;
        Debug.LogWarning("JUMPING SHOCKWAVE ATTACK!");

        float jumpDuration = shockwaveWarningTime;
        
        float jumpTimeStart = Time.time;
        float lerpAMT = 0.0f;
        Vector3 jumpVector = transform.localPosition;
        float originalYPosition = jumpVector.y;
        float jumpHeight = jumpVector.y - shockwaveWarningLowerHeight;
        if(deadLegCount < 3)
        {
            while (lerpAMT < 1.0f)
            {
                lerpAMT = (Time.time - jumpTimeStart) / jumpDuration;
                if (lerpAMT >= 1.0f) lerpAMT = 1.0f;
                jumpVector.y = MathfExtensions.MathfExt.Oscillate(originalYPosition, jumpHeight, lerpAMT);
                transform.localPosition = jumpVector;
                yield return null;
            }
        }
        
        jumpVector.y = originalYPosition;
        transform.localPosition = jumpVector;
        jumpDuration = shockwaveJumpDuration;
        jumpHeight = shockwaveJumpHeight;
        jumpTimeStart = Time.time;
        lerpAMT = 0.0f;
        while (lerpAMT < 1.0f)
        {
            lerpAMT = (Time.time - jumpTimeStart) / jumpDuration;
            if (lerpAMT >= 1.0f) lerpAMT = 1.0f;
            jumpVector.y = MathfExtensions.MathfExt.Oscillate(originalYPosition, jumpHeight, lerpAMT);
            transform.localPosition = jumpVector;
            yield return null;
        }
        jumpVector.y = originalYPosition;
        transform.localPosition = jumpVector;
        shockWaveParticles.PlayShockwave();
        PlayShockwaveAudioClip();
        yield return null;
        EmittingShockwaveAttack = false;
        Debug.LogWarning("EmittingShockwaveAttack = FALSE");
        movementController.MoveCharacter(Vector2.zero);
        if(allLegsDied == false) movementController.enabled = true;
    }


    IEnumerator TargettedShockwaveAttack()
    {
        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        CharacterMovementController movementController = GetComponent<CharacterMovementController>();
        Debug.LogWarning("Shockwave attack!");

        if(allCannonsDied == false)
        {
            GetComponent<AIRoamingController>().SetRoam(false);
        }
        if (agent.enabled == false)
        {
            yield return null;
            agent.enabled = true;
        }

        // Walk towards the player
        // Get close enough
        while (allLegsDied == false && BeatSyncReceiver.BeatReceiver.LastBeatQueued.BeatMapTimestampIndex != shockWaveAttackBeatMapTargetIndex)
        {
            agent.SetDestination(PlayerController.PlayerCamera.transform.position);
            movementController.MoveCharacter(CharacterMovementController.AgentVelocityToVector2DInput(agent));
            yield return null;
        }


        float targetTimestamp = BeatSyncReceiver.BeatReceiver.LastBeatQueued.BeatTargetTime;
        float startAttackTimestamp = BeatSyncReceiver.BeatReceiver.LastBeatQueued.BeatTargetTime - shockwaveJumpDuration - shockwaveWarningTime;

        while(Time.time < startAttackTimestamp)
        {
            agent.SetDestination(PlayerController.PlayerCamera.transform.position);
            movementController.MoveCharacter(CharacterMovementController.AgentVelocityToVector2DInput(agent));
            yield return null;
        }

        // Jump and emmit shockwave attack
        EmittingShockwaveAttack = true;

        agent.enabled = false;
        movementController.MoveCharacter(Vector2.zero);
        movementController.enabled = false;
        yield return null;
        Debug.LogWarning("JUMPING SHOCKWAVE ATTACK!");

        float jumpDuration = shockwaveWarningTime;

        float jumpTimeStart = Time.time;
        float lerpAMT = 0.0f;
        Vector3 jumpVector = transform.localPosition;
        float originalYPosition = jumpVector.y;
        float jumpHeight = jumpVector.y - shockwaveWarningLowerHeight;
        if (deadLegCount < 3)
        {
            while (lerpAMT < 1.0f)
            {
                lerpAMT = (Time.time - jumpTimeStart) / jumpDuration;
                if (lerpAMT >= 1.0f) lerpAMT = 1.0f;
                jumpVector.y = MathfExtensions.MathfExt.Oscillate(originalYPosition, jumpHeight, lerpAMT);
                transform.localPosition = jumpVector;
                yield return null;
            }
        }
        else
        {
            
            while (Time.time < targetTimestamp - jumpDuration)
            {
                agent.SetDestination(PlayerController.PlayerCamera.transform.position);
                movementController.MoveCharacter(CharacterMovementController.AgentVelocityToVector2DInput(agent));
                yield return null;
            }
        }

        jumpVector.y = originalYPosition;
        transform.localPosition = jumpVector;
        jumpDuration = targetTimestamp - Time.time;
        jumpHeight = shockwaveJumpHeight;
        jumpTimeStart = Time.time;
        lerpAMT = 0.0f;
        while (lerpAMT < 1.0f)
        {
            lerpAMT = (Time.time - jumpTimeStart) / jumpDuration;
            if (lerpAMT >= 1.0f) lerpAMT = 1.0f;
            jumpVector.y = MathfExtensions.MathfExt.Oscillate(originalYPosition, jumpHeight, lerpAMT);
            transform.localPosition = jumpVector;
            yield return null;
        }
        jumpVector.y = originalYPosition;
        transform.localPosition = jumpVector;
        shockWaveParticles.PlayShockwave();
        PlayShockwaveAudioClip();
        yield return null;
        EmittingShockwaveAttack = false;
        Debug.LogWarning("EmittingShockwaveAttack = FALSE");
        movementController.MoveCharacter(Vector2.zero);
        if (allLegsDied == false) movementController.enabled = true;
        if (allCannonsDied == false)
        {
            agent.enabled = true;
            GetComponent<AIRoamingController>().SetRoam(true);
        }
    }

    private void PlayShockwaveAudioClip()
    {
        if(shockWaveAudioSource != null && shockWaveAudioClips.Length > 0)
        {
            shockWaveAudioSource.PlayOneShot(shockWaveAudioClips[Random.Range(0, shockWaveAudioClips.Length)]);
        }
    }

    IEnumerator FireCannons()
    {
        // Play some machine gun reving sound?
        int shotsFired = 0;
        int shotsToHitPlayer = maxBulletsToHitPlayerInBarage;
        BossCannon cannonUsed;
        bool shouldHit = false;
        while (shotsFired < bulletBarageCount && (cannonL != null || cannonR != null))
        {
            cannonUsed = GetCanon();
            if (shotsToHitPlayer == 0) { shouldHit = false; }
            else
            {
                shouldHit = Random.Range(0.0f, 1.0f) < 0.5f;
                if(shouldHit) shotsToHitPlayer--;
            }

            if (cannonUsed != null) cannonUsed.ShootPlayer(attackBulletDamage, shouldHit);
            shotsFired++;
            yield return new WaitForSeconds(bulletBarageShotFrequency + Random.Range(0.0f, bulletBarageShotFrequencySalt));

            if (ShouldTargettedShockwave())
            {
                yield break;
            }
        }

    }

    BossCannon lastCannonShot = null;

    BossCannon GetCanon()
    {
        if (cannonL == null) return cannonR;
        if (cannonR == null) return cannonL;
        lastCannonShot = lastCannonShot == cannonR ? cannonL : cannonR;
        return lastCannonShot;
    }


    IEnumerator BossMechExecute()
    {
        while (true)
        {
            if (EmittingShockwaveAttack)
            {
                yield return null;
                continue;
            }
            Vector3 direction = PlayerController.PlayerCamera.transform.position - transform.position;
            direction.y = transform.position.y;

            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction, Vector3.up), rotateSpeed * Time.deltaTime);
            yield return null;
        }
    }


    private bool ShouldTargettedShockwave()
    {
        // If all legs have died, then return false
        if (allLegsDied) return false;
        // If the last beat queued is null, then return false
        if (BeatSyncReceiver.BeatReceiver.LastBeatQueued == null) return false;
        // If the last beat queued is between the shockwave attack prep range, then return true
        if(BeatSyncReceiver.BeatReceiver.LastBeatQueued.BeatMapTimestampIndex >= shockWaveAttackPrepBeatMapIndex && BeatSyncReceiver.BeatReceiver.LastBeatQueued.BeatMapTimestampIndex <= shockWaveAttackPrepBeatMapIndex + shockWaveAttackPrepBeatMapCount)
        {
            Debug.Log("Should target shockwave!");
            return true;
        }
        else
        {
            return false;
        }
    }

    
}
