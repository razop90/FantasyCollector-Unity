using System.Collections;
using UnityEngine;

public class EnemyShootable : MonoBehaviour
{
    public float damage = 10f;
    public GameObject blood;

    [Header("Shot")]
    public bool canShot = false;
    public float timeBtwShots = 2f;
    public int startShotFromLifeUnder = 80;

    [Header("Bullets Settings")]
    public Transform barrelEnd;
    public int lowLifeIndicator = 50;
    public float delayBullet = 0f; //the delay time between the bullet was shoot, until its actually shoots.
    public GameObject[] bulletsOfHighLife;
    public GameObject[] bulletsOfLowLife;

    private EnemyHealth health;
    private EnemyPatrolMovement patrolMovement;
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        health = GetComponent<EnemyHealth>();
        patrolMovement = GetComponent<EnemyPatrolMovement>();

        timeBtwShots = startTimeBtwShots;
    }

    public void OnShootHit(Vector3 hitPoint)
    {
        //display blood animation.
        Instantiate(blood, hitPoint, Quaternion.identity);

        health.Damage(damage);
        patrolMovement.OnAware();
    }

    public float startTimeBtwShots;
    private void Update()
    {
        if (canShot && health.currentHealth <= startShotFromLifeUnder && patrolMovement.isAware && !health.isDead)
        {
            //shoot from a small distance from the player
            var d = Vector3.Distance(barrelEnd.position, GameManager.instance.location.position);
            if (Vector3.Distance(barrelEnd.position, GameManager.instance.location.position) > 10f)
            {
                if (timeBtwShots <= 0)
                {
                    animator.SetTrigger("attack");

                    StartCoroutine(DeleyBulletLogic(delayBullet));

                    timeBtwShots = startTimeBtwShots;
                }
                else
                {
                    timeBtwShots -= Time.deltaTime;
                }
            }
        }
    }

    private IEnumerator DeleyBulletLogic(float waiting = 0.3f)
    {
        yield return new WaitForSeconds(waiting);
        GameObject[] bullets = bulletsOfHighLife;

        if (health.currentHealth < lowLifeIndicator)
        {
            bullets = bulletsOfLowLife;
        }
        var index = Random.Range(0, bullets.Length);
        Instantiate(bullets[index], barrelEnd.position, barrelEnd.rotation);
    }
}
