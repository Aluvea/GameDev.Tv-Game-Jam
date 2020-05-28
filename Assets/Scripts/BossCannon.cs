using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCannon : MonoBehaviour
{
    [SerializeField] Bullet bulletPrefab;
    [SerializeField] EnemyHealth canonHealth;
    [SerializeField] Transform gunMuzzle1Reference;
    [SerializeField] Transform gunMuzzle2Reference;
    [SerializeField] LayerMask projectileLayerMask;
    [SerializeField] AudioSource gunAudioSource;
    [SerializeField] AudioClip [] gunShotClip;
    private Transform currentGunMuzzleUsed;

    private void Awake()
    {
        if (Random.Range(0.0f, 1.0f) <= 0.5)
        {
            currentGunMuzzleUsed = gunMuzzle1Reference;
        }
        else
        {
            currentGunMuzzleUsed = gunMuzzle2Reference;
        }
        canonHealth.Died += OnCannonDestroyed;
    }

    public bool CanonDestroyed
    {
        private set;
        get;
    } = false;

    private void OnCannonDestroyed(EnemyHealth deadEnemy)
    {
        CanonDestroyed = true;
    }

    public void ShootPlayer(float hitDamage, bool shouldHitTarget)
    {
        // Get the bullet's target position
        Vector3 bulletTargetPosition = GetBulletTargetPosition(PlayerController.PlayerCamera.transform, shouldHitTarget);
        // Shoot a bullet projectile towards at the bullet target position determined
        ShootBulletProjectile(bulletTargetPosition, hitDamage);
        PlayGunShot();
    }

    Queue<Bullet> bulletObjectPool = new Queue<Bullet>();
    List<Bullet> instantiatedBullets = new List<Bullet>();

    /// <summary>
    /// Shoots a bullet projectile at a given target position
    /// </summary>
    /// <param name="targetPosition">The bullet projectile target position</param>
    /// <param name="hitDamage">The damage applied on hit</param>
    void ShootBulletProjectile(Vector3 targetPosition, float hitDamage)
    {
        // If the bullet object pool is available, then dequeue a bullet projectile
        if (bulletObjectPool.Count > 0)
        {
            Bullet dequeuedBullet = bulletObjectPool.Dequeue();
            // Reset its position to the gun muzzle position
            dequeuedBullet.transform.position = currentGunMuzzleUsed.position;
            // Enable the game object
            dequeuedBullet.gameObject.SetActive(true);
            // Project the bullet at the target position
            dequeuedBullet.ProjectBullet(targetPosition, hitDamage, projectileLayerMask);
        }
        else
        {
            // Create a bullet projectile
            Bullet bulletCreated = Instantiate(bulletPrefab, currentGunMuzzleUsed.position, Quaternion.identity);
            // Shoot it at the target position
            bulletCreated.ProjectBullet(targetPosition, hitDamage, projectileLayerMask);
            // Add the bullet to our list of instantiated bullets
            instantiatedBullets.Add(bulletCreated);
            // Subscribe to the bullet's ending travel event
            bulletCreated.BulletTravelEnded += OnBulletTravelEnded;
        }
        currentGunMuzzleUsed = currentGunMuzzleUsed == gunMuzzle1Reference ? gunMuzzle2Reference : gunMuzzle1Reference;
    }


    /// <summary>
    /// Retuns a bullet projectile destination position based on a target and whether or not the target should be hit
    /// </summary>
    /// <param name="target"></param>
    /// <param name="shouldHit"></param>
    /// <returns></returns>
    private Vector3 GetBulletTargetPosition(Transform target, bool shouldHit)
    {
        // If the target should be missed, then offset the bullet target position towards on the x and y axis
        if (shouldHit == false)
        {
            float randomXAxisMissOffset = Random.Range(0.0f, 2f);
            float randomYAxisMissOffset = 2.0f - randomXAxisMissOffset;
            return target.position + (currentGunMuzzleUsed.right * randomXAxisMissOffset) + (randomYAxisMissOffset * currentGunMuzzleUsed.up);
        }
        // If the target should be hit, then offset the bullet target position down a little bit
        // It's annoying for bullets to be flying into the player's line of sight / camera
        else
        {
            return target.position - new Vector3(0.0f, 0.3f, 0.0f);
        }
    }

    /// <summary>
    /// Callback method raised when a bullet is done travelling towards its destination
    /// </summary>
    /// <param name="bullet"></param>
    void OnBulletTravelEnded(Bullet bullet)
    {
        // Disable the game object
        bullet.gameObject.SetActive(false);
        // Enqueue the bullet into our object pool
        bulletObjectPool.Enqueue(bullet);
    }

    private void OnDestroy()
    {
        // Unsubscribe to our list of instantiated bullets
        foreach (Bullet bulletCreated in instantiatedBullets)
        {
            if (bulletCreated != null) bulletCreated.BulletTravelEnded -= OnBulletTravelEnded;
        }

        // Destroy the bullets in our object pool
        while (bulletObjectPool.Count > 0)
        {
            Bullet dequeuedBullet = bulletObjectPool.Dequeue();
            if (dequeuedBullet != null)
            {
                if (dequeuedBullet.gameObject != null) Destroy(dequeuedBullet.gameObject);
            }
        }
    }

    private void PlayGunShot()
    {
        if(gunAudioSource != null)
        {
            if(gunShotClip.Length > 0)
            {
                gunAudioSource.PlayOneShot(gunShotClip[Random.Range(0, gunShotClip.Length)]);
            }
        }
    }
}
