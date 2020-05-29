using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMechShockwave : MonoBehaviour
{
    [SerializeField] private float shockwaveDamage;
    [SerializeField] private float shockWaveWidth = 1.0f;
    [SerializeField] private float shockWaveHeight;

    [SerializeField] Transform sphereRef;

    ParticleSystem PSystem;
    private void Awake()
    {
        PSystem = GetComponent<ParticleSystem>();
    }
    public void SetShockwaveDamage(float dmg)
    {
        shockwaveDamage = dmg;
    }

    public void PlayShockwave()
    {
        StartCoroutine(MonitorPlayerHit());
    }


    IEnumerator MonitorPlayerHit()
    {
        PSystem.Play();
        while(PSystem.particleCount  == 0)
        {
            yield return null;
        }

        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[PSystem.particleCount];

        int particleCount = PSystem.GetParticles(particles);
        ParticleSystem.Particle shockParticle = particles[particleCount - 1];
        Vector3 startingPosition = shockParticle.position;
        float radiusDistance;
        Transform groundCheckRef = PlayerController.PlayerCamera.transform.parent.Find("Ground Check Cube");
        PlayerHealth playerHealth = PlayerController.PlayerCamera.transform.parent.GetComponent<PlayerHealth>();
        Vector3 playerPosition;
        float playerDistanceFromStartingPosition;
        Vector3 testDrawEndLinePosition;
        Vector3 currentSize;
        float deltaSize = 0;
        float lastSize = 0;
        float widthToUse = 0;
        while (shockParticle.remainingLifetime > 0 && PSystem.particleCount > 0)
        {
            particleCount = PSystem.GetParticles(particles);
            shockParticle = particles[particleCount - 1];
            currentSize = shockParticle.GetCurrentSize3D(PSystem);
            // Get the radius distance of the shock particle
            radiusDistance = currentSize.x / 2.0f;

            if(lastSize == 0)
            {
                lastSize = radiusDistance;
                deltaSize = radiusDistance;
            }
            else
            {
                deltaSize = radiusDistance - lastSize;
                lastSize = radiusDistance;
            }

            widthToUse = shockWaveWidth < deltaSize ? deltaSize : shockWaveWidth;

            //Debug.LogWarning("Shockwave Size = " + currentSize +"; DeltaSize = "+ deltaSize);
            
            testDrawEndLinePosition = startingPosition;
            testDrawEndLinePosition.x += radiusDistance;
            testDrawEndLinePosition.y += 0.5f;
            //Debug.DrawLine(startingPosition, testDrawEndLinePosition, Color.red, 1.0f);
            playerPosition = groundCheckRef.position;
            playerPosition.y = startingPosition.y;
            playerDistanceFromStartingPosition = Vector3.Distance(playerPosition, startingPosition);
            if (playerDistanceFromStartingPosition <= radiusDistance + widthToUse && playerDistanceFromStartingPosition >= radiusDistance - widthToUse)
            {
                Debug.LogWarning("PLAYER IS IN SHOCKWAVE RADIUS!");
                if(PlayerController.IsPlayerAirborne == false)
                {
                    Debug.LogWarning("PLAYER IS NOT AIRBORNE! TAKING DAMAGE!");
                    playerHealth.TakeDamage(shockwaveDamage);
                    yield break;
                }
                else
                {

                    Debug.LogWarning("PLAYER IS AIRBORNE! NO SHOCKWAVE DAMAGE TO TAKE!");
                }
                
            }

            yield return null;
        }
    }

}
