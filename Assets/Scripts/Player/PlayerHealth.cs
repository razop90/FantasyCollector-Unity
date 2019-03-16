using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Assets.Scripts;

public class PlayerHealth : MonoBehaviour
{
    public float startingHealth = 100;                            // The amount of health the player starts the game with.
    public float currentHealth;                                   // The current health the player has.
    public Image healthBarImage;                                 // Reference to the UI's health bar.
    public Image damageImage;                                   // Reference to an image to flash on the screen on being hurt.
    public AudioClip deathClip;                                 // The audio clip to play when the player dies.
    public float flashSpeed = 5f;                               // The speed the damageImage will fade at.
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f);     // The colour the damageImage is set to, to flash.

    private Animator anim;                                              // Reference to the Animator component.
    private AudioSource playerAudio;                                    // Reference to the AudioSource component.
    public bool isDead { get; protected set; }                          // Whether the player is dead.
    private bool damaged;                                               // True when the player gets damaged.

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Collectable Health")
        {
            var health = other.GetComponent<HealthCollectable>();

            if (!health.picked)
            {
                var total = currentHealth + health.amount;
                int pickedAmount = health.amount;

                if (currentHealth == startingHealth) //full, dont need to pick
                {
                    //FloatingTextHandler.CreateFloatingText(transform.position, "Health Is Full", Color.blue, 25);
                    return;
                }
                else if (total > startingHealth) //not full, but need a bit of
                {
                    pickedAmount = Mathf.RoundToInt(startingHealth - currentHealth);
                    currentHealth = startingHealth;
                }
                else //need full amount
                {
                    currentHealth += health.amount;
                }

                SetHealth(currentHealth);
                health.Pick(pickedAmount);
            }
        }
    }

    //bool flag = true;
    //IEnumerator EnemyAttackYield(float waiting = 0.3f)
    //{
    //    yield return new WaitForSeconds(waiting);
    //    flag = true;
    //}

    void Awake()
    {
        // Setting up the references.
        anim = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();
        //playerMovement = GetComponent<PlayerMovement>();
        //playerShooting = GetComponentInChildren<PlayerShooting>();

        // Set the initial health of the player.
        currentHealth = startingHealth;
    }

    private static int count = 0;
    void Update()
    {
        // If the player has just been damaged...
        if (damaged || Input.GetKeyDown(KeyCode.Tab))
        {
            // ... set the colour of the damageImage to the flash colour.
            damageImage.color = flashColour;
        }
        // Otherwise...
        else
        {
            // ... transition the colour back to clear.
            damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }

        // Reset the damaged flag.
        damaged = false;
        count++;
    }

    public void TakeDamage(int amount)
    {
        damaged = true;

        SetHealth(currentHealth - amount);

        //// Play the hurt sound effect.
        //playerAudio.Play();

        if (currentHealth <= 0 && !isDead)
        {
            Death();
        }
    }

    public void SetHealth(float amount)
    {
        currentHealth = amount;
        healthBarImage.fillAmount = currentHealth / startingHealth;

        if (currentHealth <= 0 && !isDead)
        {
            Death();
        }
    }

    void Death()
    {
        isDead = true;
        anim.SetTrigger("die");
    }
}