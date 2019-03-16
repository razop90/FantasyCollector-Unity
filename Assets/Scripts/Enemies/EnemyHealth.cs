using Assets.Scripts;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public event Action<Transform> OnDeath;

    public float startingHealth = 100;
    public float currentHealth;
    public Image healthBarImage;
    public Canvas healthBarContainer;
    public string deathTrigger = "Die";
    public bool isDead { get; protected set; }

    [Header("Sound")]
    public AudioClip awakeClip;
    public AudioClip[] clips;
    public AudioClip deathClip;
    public AudioClip hurtClip;

    private AudioSource audio;
    private Enemy enemy;
    private Animator anim;

    void Awake()
    {
        audio = GetComponent<AudioSource>();
        if (awakeClip != null)
        {
            audio.Stop();
            audio.PlayOneShot(awakeClip);
        }

        anim = GetComponent<Animator>();
        enemy = GetComponent<Enemy>();
        currentHealth = startingHealth;
        isDead = false;
    }

    private int lastPlayedClip = -1;
    private bool playingInProcess = false;
    private void Update()
    {
        //Play sound.
        if (clips != null && clips.Length > 0 && !playingInProcess)
        {
            if (!audio.isPlaying)
            {
                playingInProcess = true;
                StartCoroutine(PlayRandomClip());
            }
        }
    }

    private IEnumerator PlayRandomClip()
    {
        var index = UnityEngine.Random.Range(0, clips.Length);

        while (index == lastPlayedClip)
        {
            index = UnityEngine.Random.Range(0, clips.Length);
        }

        lastPlayedClip = index;

        yield return new WaitForSeconds(clips[index].length / 4);

        if (!audio.isPlaying)
            audio.PlayOneShot(clips[index]);
        playingInProcess = false;
    }

    public void Damage(float amount)
    {
        if (!isDead)
        {
            //decrease damage
            currentHealth -= amount;
            healthBarImage.fillAmount = currentHealth / startingHealth;

            //play hurt clip
            if (hurtClip != null && !audio.isPlaying && !playingInProcess)
            {
                audio.Stop();
                audio.PlayOneShot(hurtClip);
            }

            //display the damage amunt on screen
            FloatingTextHandler.CreateFloatingText(transform, "-" + amount.ToString(), Color.yellow);
        }

        if (currentHealth <= 0f && !isDead)
        {
            Death();
        }
    }

    void Death()
    {
        GetComponent<CapsuleCollider>().enabled = false;
        isDead = true;
        healthBarContainer.enabled = false;
        anim.SetTrigger(deathTrigger);

        var destroyTime = deathClip != null ? deathClip.length + 0.02f : 2f;
        Destroy(gameObject, destroyTime);

        if (OnDeath != null)
            OnDeath.Invoke(transform);

        if (deathClip != null)
        {
            audio.Stop();
            audio.PlayOneShot(deathClip);
        }
    }
}