using UnityEngine;

public class EnemyBossPatrolMovement : EnemyPatrolMovement
{
    public string specialAttackName = "specialAttack";
    public string FinalAttackName = "finalAttack";

    public override void OnStart()
    {
        base.OnStart();
        health = GetComponent<EnemyHealth>();
    }

    public override void OnUpdate()
    {
        if (isAware)
        {
            //way far from the player - search for him.
            if (Vector3.Distance(player.transform.position, transform.position) > viewDistance)
            {
                isAware = false;
                return;
            }

            //very close to the player - attack.
            if (agent.remainingDistance < 4f)
            {
                if (health.currentHealth <= 50)
                    animator.SetTrigger(FinalAttackName);
                else
                    animator.SetTrigger(specialAttackName);
            }

            //go for the player.
            agent.SetDestination(player.transform.position);
        }
        else
        {
            SearchPlayer();
            Wander();
        }
    }
}
