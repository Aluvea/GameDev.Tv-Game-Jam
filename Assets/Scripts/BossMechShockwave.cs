using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMechShockwave : MonoBehaviour
{
    [SerializeField] private float shockwaveDamage;

    private void Awake()
    {
        PSystem = GetComponent<ParticleSystem>();
        CollisionEvents = new List<ParticleCollisionEvent>();
    }
    public void SetShockwaveDamage(float dmg)
    {
        shockwaveDamage = dmg;
        
    }

    private ParticleSystem PSystem;
    private List<ParticleCollisionEvent> CollisionEvents;

    private void OnParticleCollision(GameObject other)
    {
        Debug.LogWarning("OnParticleCollision in ShockWave!");

        int collisionCount = ParticlePhysicsExtensions.GetCollisionEvents(PSystem, other, CollisionEvents);



        Debug.Log("Collided with " + CollisionEvents.Count + " objects");
        for (int i = 0; i < CollisionEvents.Count; i++)
        {
            Debug.Log("Collided with " + CollisionEvents[i].colliderComponent.gameObject.name);

            PlayerHealth playerHealth = CollisionEvents[i].colliderComponent.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                Debug.LogWarning("Shock wave hit player1");
                if (PlayerController.IsPlayerAirborne == false)
                {
                    Debug.LogWarning("Damaging player!");
                    playerHealth.TakeDamage(shockwaveDamage);
                }
                else
                {
                    Debug.LogWarning("Player is airborne.");
                }
            }
        }

        
        
    }
}
