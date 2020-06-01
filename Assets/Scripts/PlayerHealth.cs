using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] HealthUI healthUI;
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float currentHealth;
    [SerializeField] GameOverUI gameOverUI;
    [SerializeField] WeaponShooting weaponShooting;

    [SerializeField] Canvas damgeScreen;
    [SerializeField] Image bloodOverlay;
    [SerializeField] Animator bloodAnimator;

    [SerializeField] AudioSource playerAudioSource;
    [SerializeField] AudioClip[] clips;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {

        if (currentHealth > 0)
        {
            currentHealth -= damage;
            bloodAnimator.SetTrigger("damaged");
            PlayRandomClipFromArray(clips);

            healthUI.UpdateHealthUIMeter(currentHealth, maxHealth);
        }

        else if (currentHealth <= 0)
        {
            Destroy(weaponShooting);
            Destroy(GetComponent<PlayerController>());

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Time.timeScale = 0;
            GetComponent<AudioSource>().Play();

            gameOverUI.HandleDeath();
        }
    }

    public void GiftHealth(float health)
    {
        currentHealth += health;

        if (currentHealth > maxHealth) currentHealth = maxHealth;
        healthUI.UpdateHealthUIMeter(currentHealth, maxHealth);


    }

    /// <summary>
    /// Whether or not the player has full health
    /// </summary>
    public bool AtFullHealth
    {
        get
        {
            return maxHealth == currentHealth;
        }
    }
        private void PlayRandomClipFromArray(AudioClip[] clips)
    {
        if (playerAudioSource != null && clips != null)
        {
            if (clips.Length > 0)
            {
                playerAudioSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);
            }
        }
    }

}
