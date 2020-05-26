using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour
{
    [Tooltip("How much health this health pack should give the player")]
    [SerializeField] float healthBonus = 5.0f;
    [Tooltip("Whether or not this health pack can be picked up when the player has full health")]
    [SerializeField] bool canBePickedUpOnFullHealth = false;
    [Header("Pick Up Sound FX")]
    [SerializeField] AudioClip healthPackAudioClip;
    [Range(0.0f,1.0f)]
    [SerializeField] float clipVolume;

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
        if(playerHealth != null)
        {
            if(canBePickedUpOnFullHealth == false && playerHealth.AtFullHealth)
            {
                return;
            }
            playerHealth.GiftHealth(healthBonus);
            if(healthPackAudioClip != null)
            {
                AudioSource.PlayClipAtPoint(healthPackAudioClip, PlayerController.PlayerCamera.transform.position, clipVolume);
            }
           
            Destroy(gameObject);
        }
    }
}
