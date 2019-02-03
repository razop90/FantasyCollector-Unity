using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyPatrolMovement : MonoBehaviour
{
    public float fov = 180f;
    public float viewDistance = 20f;
    public float wanderRadius = 10f;

    protected GameObject player;
    protected bool isAware = false;
    protected Vector3 wanderPoint;
    protected NavMeshAgent agent;
    protected Animator animator;
    protected EnemyHealth health;

    void Start()
    {
        OnStart();
    }

    private bool destroyed = false;
    void Update()
    {
        if (health.isDead)
        {
            if (!destroyed)
            {
                destroyed = true;
                Destroy(this, 0);
            }

            agent.isStopped = true;
            return;
        }

        OnUpdate();
    }

    public virtual void OnStart()
    {
        player = GameObject.FindWithTag("Player");
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<EnemyHealth>();
        wanderPoint = RandomWanderPoint();
    }
   
    public virtual void OnUpdate()
    {       
        if (isAware)
        {  
            //go for the player.
            agent.SetDestination(player.transform.position);

            //way far from the player - search for him.
            if (Vector3.Distance(player.transform.position, transform.position) > viewDistance)
            {
                isAware = false;
                return;
            }

            //very close to the player - attack.
            if (agent.remainingDistance < 2f)
            {
                animator.SetTrigger("attack");
            }        
        }
        else
        {
            SearchPlayer();
            Wander();
        }
    }

    protected void SearchPlayer()
    {
        if (Vector3.Angle(Vector3.forward, transform.InverseTransformPoint(player.transform.position)) < fov / 2f)
        {
            if (Vector3.Distance(player.transform.position, transform.position) < viewDistance)
            {
                RaycastHit hit;
                if (Physics.Linecast(transform.position, player.transform.position, out hit, -1))
                {
                    if (hit.transform.CompareTag("Player"))
                    {
                        OnAware();
                    }
                }
            }
        }
    }

    public void OnAware()
    {
        isAware = true;
    }

    float lastDistanceFromTarget = 0;
    bool findAnotherTarget = false;
    protected void Wander()
    {
        var distanceFromTarget = Vector3.Distance(transform.position, wanderPoint);
        if (findAnotherTarget || Vector3.Distance(transform.position, wanderPoint) < 0.5f)
        {
            findAnotherTarget = false;
            wanderPoint = RandomWanderPoint();
            animator.SetFloat("speed", 2);
        }
        else
        {
            if (lastDistanceFromTarget == distanceFromTarget)
            {
                findAnotherTarget = true;
                animator.SetFloat("speed", 0);
            }
            else
            {
                animator.SetFloat("speed", 2);
            }

            agent.SetDestination(wanderPoint);
            lastDistanceFromTarget = distanceFromTarget;
        }
    }

    protected Vector3 RandomWanderPoint()
    {
        Vector3 randomPoint = (Random.insideUnitSphere * wanderRadius) + transform.position;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomPoint, out navHit, wanderRadius, -1);
        return new Vector3(navHit.position.x, transform.position.y, navHit.position.z);
    }
}
