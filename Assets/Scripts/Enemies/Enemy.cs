using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int damage = 5;
    public float waitingBetweenAttacks = 0.3f;
    public string attackName;

    Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public bool isAttacking()
    {
        return animator.GetBool(attackName);
    }
}
