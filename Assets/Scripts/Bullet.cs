using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float damage;

    [Range(5.0f, 150.0f)]
    [SerializeField] float bulletSpeed;


    [Min(25.0f)]
    float bulletDistance = 90.0f;
    [SerializeField]
    private LayerMask bulletMask;

    private Vector3 bulletTargetPosition;
    
    /// <summary>
    /// Projects this bullet towards a world position
    /// </summary>
    /// <param name="bulletTargetPosition">The world position this bullet should project towards</param>
    /// <param name="damage">The damage this bullet should deal if it hits a target</param>
    /// <param name="layerMask">The physical layer mask applied to this bullet (what it can hit)</param>
    public void ProjectBullet(Vector3 bulletTargetPosition, float damage, LayerMask layerMask)
    {
        this.damage = damage;
        this.bulletMask = layerMask;
        
        this.bulletTargetPosition = bulletTargetPosition;
        StartCoroutine(BulletProjectionCoroutine());
    }

    /// <summary>
    /// Projects this bullet towards a world position
    /// </summary>
    /// <param name="bulletTargetPosition">The world position this bullet should project towards</param>
    /// <param name="damage">The damage this bullet should deal if it hits a target</param>
    /// <param name="layerMask">The physical layer mask applied to this bullet (what it can hit)</param>
    /// <param name="bulletSpeed">The speed of this bullet projectile particle</param>
    public void ProjectBullet(Vector3 bulletTargetPosition, float damage, LayerMask layerMask, float bulletSpeed)
    {
        this.bulletSpeed = bulletSpeed;
        ProjectBullet(bulletTargetPosition, damage, layerMask);
    }


    IEnumerator BulletProjectionCoroutine()
    {
        // Look at the bullet's target position
        transform.LookAt(bulletTargetPosition);
        // Cast a ray from the bullet's current position towards its destination direction
        Ray bulletRay = new Ray(transform.position, bulletTargetPosition - transform.position);
        // Raycast the bullet projectile
        bool hitSomthing = Physics.Raycast(bulletRay, out RaycastHit bulletHitInfo, bulletDistance, bulletMask);

        // If the raycast hit something, move the bullet towards the given transform's contact position
        if (hitSomthing && bulletHitInfo.transform != null)
        {
            Debug.LogWarning("Hit " + bulletHitInfo.transform.gameObject.name);
            // Cache a reference to the transform of the bullet collision
            Transform transformRef = bulletHitInfo.transform;
            // Identify the offset position relative to the bullet contact point of the target
            Vector3 contactPointOffset = bulletHitInfo.point - bulletHitInfo.transform.position;
            // Cached vector3 for the bullet's current destination (the transform's bullet contact point in world space)
            Vector3 bulletContactPointInWorldSpace = bulletHitInfo.transform.position + contactPointOffset;

            // Keep moving towards the bullet contact point
            while(Vector3.Distance(transform.position, bulletContactPointInWorldSpace) > Mathf.Epsilon && transformRef != null)
            {
                // Get the new bullet contact position of the hit target
                bulletContactPointInWorldSpace = transformRef.position + contactPointOffset;
                // Move towards the bullet contact position
                transform.position = Vector3.MoveTowards(transform.position, bulletContactPointInWorldSpace, bulletSpeed * Time.deltaTime);
                // Look at the bullet contact position
                //transform.LookAt(currentBulletContactPoint);
                yield return null;
            }
            
            // If the transform reference is still around, then check if it can be damaged or if spark VFX should played (for example if a wall is hit)
            if(transformRef != null)
            {
                // TO-DO
                // Check if the target reference can be damaged
                // If not, then emmit spark VFX
                PlayerHealth healthAttached = transformRef.GetComponent<PlayerHealth>();
                if(healthAttached != null)
                {
                    healthAttached.TakeDamage(damage);
                }
            }
            // If the target transform has been destroyed for what ever reason, then proceed to move towards the bullet's destination 
            else if(transformRef == null)
            {
                while (Vector3.Distance(transform.position, bulletTargetPosition) > Mathf.Epsilon)
                {
                    transform.position = Vector3.MoveTowards(transform.position, bulletTargetPosition, bulletSpeed * Time.deltaTime);
                    yield return null;
                }
            }
        }
        // If the raycast didn't hit anything, then simply move the bullet towards the bullet's destination position
        else
        {
            Debug.LogWarning("Bullet Didn't Hit Anything");
            while (Vector3.Distance(transform.position, bulletTargetPosition) > Mathf.Epsilon)
            {
                transform.position = Vector3.MoveTowards(transform.position, bulletTargetPosition, bulletSpeed * Time.deltaTime);
                yield return null;
            }
        }
        

        if(BulletTravelEnded!= null)
        {
            BulletTravelEnded.Invoke(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public System.Action<Bullet> BulletTravelEnded;
}
