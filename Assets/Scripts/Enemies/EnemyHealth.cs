using Assets.Scripts;
using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public event Action<Transform> OnDeath;

    public float startingHealth = 100;
    public float currentHealth;
    public Image healthBarImage;
    public Canvas healthBarContainer;
    public AudioClip deathClip;
    public string deathTrigger = "Die";
    public bool isDead { get; protected set; }


    private Enemy enemy;
    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
        enemy = GetComponent<Enemy>();
        currentHealth = startingHealth;
        isDead = false;
    }

    public void Damage(float amount)
    {
        if (!isDead)
        {
            //decrease damage
            currentHealth -= amount;
            healthBarImage.fillAmount = currentHealth / startingHealth;

            //display the damage amunt on screen
            FloatingTextHandler.CreateFloatingText(transform, "-" + amount.ToString(), Color.yellow);

            //// Play the hurt sound effect.
            // playerAudio.Play();
        }

        if (currentHealth <= 0f && !isDead)
        {
            Death();
        }
    }

    void Death()
    {
        isDead = true;
        healthBarContainer.enabled = false;
        anim.SetTrigger(deathTrigger);

        Destroy(gameObject, 2f);

        if (OnDeath != null)
            OnDeath.Invoke(transform);

        //// Set the death flag so this function won't be called again.
        //isDead = true;

        //// Turn off any remaining shooting effects.
        //playerShooting.DisableEffects();

        //// Tell the animator that the player is dead.
        //anim.SetTrigger("Die");

        //// Set the audiosource to play the death clip and play it (this will stop the hurt sound from playing).
        //playerAudio.clip = deathClip;
        //playerAudio.Play();

        //// Turn off the movement and shooting scripts.
        //playerMovement.enabled = false;
        //playerShooting.enabled = false;
    }
}