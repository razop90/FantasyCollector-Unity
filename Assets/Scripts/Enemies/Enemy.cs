using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public bool isAttacking { get; private set; }

    [Header("Regular Attack")]
    public string regularAttackName = "attack";
    public int regularAttackDamage = 5;

    [Header("Special Attack")]
    public string specialAttackName = "attack";
    public int specialAttackDamage = 5;

    [Header("Ultimate Attack")]
    public string ultimateAttackName = "attack";
    public int ultimateAttackDamage = 5;

    private EnemyHealth health;
    private Animator animator;
    private int damage = 0;

    private void Awake()
    {
        health = GetComponent<EnemyHealth>();
        animator = GetComponent<Animator>();
        isAttacking = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!GameManager.instance.health.isDead && !health.isDead)
        {
            if (other.tag == "Player" && canAttack)
            {
                canAttack = false;

                var attackName = regularAttackName;
                damage = regularAttackDamage;

                if (health.currentHealth > 20 && health.currentHealth < 50)
                {
                    attackName = specialAttackName;
                    damage = specialAttackDamage;
                }
                else if (health.currentHealth > 0 && health.currentHealth <= 20)
                {
                    attackName = ultimateAttackName;
                    damage = ultimateAttackDamage;
                }

                animator.SetTrigger(attackName);

                isAttacking = true;

                if (attackClipLength <= 0.1f)
                {
                    AnimatorClipInfo[] clipInfos = animator.GetCurrentAnimatorClipInfo(0);
                    attackClipLength = clipInfos[0].clip.length;
                }
                StartCoroutine(EnemyAnimationIsDoneAttackYield(attackClipLength));
                StartCoroutine(EnemyAttackYield(attackClipLength / 5));
            }
        }
    }

    private bool canAttack = true;
    private float attackClipLength = 0f;
    private IEnumerator EnemyAttackYield(float waiting = 0.3f)
    {
        yield return new WaitForSeconds(waiting);
        GameManager.instance.health.TakeDamage(damage);
    }

    private IEnumerator EnemyAnimationIsDoneAttackYield(float waiting = 0.3f)
    {
        yield return new WaitForSeconds(waiting);
        canAttack = true;
        isAttacking = false;
    }
}
