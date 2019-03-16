using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public enum EnemyMovement { Idle, Walk, Run }
public class EnemyPatrolMovement : MonoBehaviour
{
    public float fov = 180f; //angle of view
    public float viewDistance = 20f;
    public float wanderRadius = 10f;
    public bool isAware { get; protected set; }

    [Header("Agent")]
    public float walkSpeed = 3.5f;
    public float runSpeed = 7f;
    public float walkRotationSpeed = 80f;
    public float runRotationSpeed = 80f;

    [Header("Animation")]
    public string wakenAnimName = null;

    protected GameObject player;
    protected Vector3 wanderPoint;
    protected NavMeshAgent agent;
    protected Animator animator;
    protected EnemyHealth health;
    protected Enemy enemy;
    protected bool destroyed = false; //this enemy was already destroyed.
    protected bool enemyIsAwake = false;

    void Start()
    {
        OnStart();
    }

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

        //Checking if the awaken animation is over in order to enable walking.
        if (!enemyIsAwake)
        {
            AnimatorClipInfo[] clipInfos = animator.GetCurrentAnimatorClipInfo(0);
            if (clipInfos[0].clip.name != wakenAnimName)
            {
                enemyIsAwake = true;
            }

            return;
        }

        //if the player is dead - dont do anything
        if (GameManager.instance.health.isDead)
        {
            agent.isStopped = true;
            SetSpeed(EnemyMovement.Idle);
        }
        else
        {
            OnUpdate();
        }
    }

    public virtual void OnStart()
    {
        player = GameObject.FindWithTag("Player");
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<EnemyHealth>();
        enemy = GetComponent<Enemy>();
        wanderPoint = RandomWanderPoint();

        if (wakenAnimName == null)
            enemyIsAwake = true;

        isAware = false;
    }

    public virtual void OnUpdate()
    {
        if (!enemy.isAttacking)
        {
            if (isAware)
            {
                //checking if the path to the player is reachable.
                NavMeshPath navpath = new NavMeshPath();
                NavMesh.CalculatePath(transform.position, player.transform.position, -1, navpath);
                if (navpath.status == NavMeshPathStatus.PathPartial || navpath.status == NavMeshPathStatus.PathInvalid)
                {
                    SetSpeed(EnemyMovement.Walk);
                    SearchPlayer();
                    Wander();
                    isAware = false;
                    return;
                }

                SetSpeed(EnemyMovement.Run);

                //go for the player.
                agent.SetDestination(player.transform.position);

                //way far from the player - search for him.
                if (Vector3.Distance(player.transform.position, transform.position) > viewDistance)
                {
                    isAware = false;
                    return;
                }
            }
            else
            {
                SetSpeed(EnemyMovement.Walk);
                SearchPlayer();
                Wander();
            }
        }
        else
        {
            SetSpeed(EnemyMovement.Idle);
            agent.ResetPath();
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
                        NavMeshPath navpath = new NavMeshPath();
                        NavMesh.CalculatePath(transform.position, player.transform.position, -1, navpath);
                        if (navpath.status == NavMeshPathStatus.PathPartial || navpath.status == NavMeshPathStatus.PathInvalid)
                        {
                            //unreachable path.
                        }
                        else
                        {
                            OnAware();
                        }
                    }
                }
            }
        }
    }

    public void OnAware()
    {
        isAware = true;
        SetSpeed(EnemyMovement.Run);
    }

    private void SetSpeed(EnemyMovement movement)
    {
        int animSpeed = 0;
        float agentSpeed = 0f;
        float rotationSpeed = walkRotationSpeed;

        switch (movement)
        {
            case EnemyMovement.Walk:
                animSpeed = 2;
                agentSpeed = walkSpeed;
                rotationSpeed = walkRotationSpeed;
                break;
            case EnemyMovement.Run:
                animSpeed = 3;
                agentSpeed = runSpeed;
                rotationSpeed = runRotationSpeed;
                break;
        }

        agent.speed = agentSpeed;
        agent.angularSpeed = rotationSpeed;

        animator.SetFloat("speed", animSpeed);
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
            SetSpeed(EnemyMovement.Walk);
        }
        else
        {
            if (Mathf.Abs(distanceFromTarget - lastDistanceFromTarget) < 0.01f)
            {
                findAnotherTarget = true;
                SetSpeed(EnemyMovement.Idle);
            }
            else
            {
                SetSpeed(EnemyMovement.Walk);
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
