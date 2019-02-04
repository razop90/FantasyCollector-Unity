using UnityEngine;

public class EnemyShootable : MonoBehaviour
{
    public float damage = 10f;
    public GameObject blood;

    private EnemyHealth health;
    private EnemyPatrolMovement patrolMovement;

    void Start()
    {
        health = GetComponent<EnemyHealth>();
        patrolMovement = GetComponent<EnemyPatrolMovement>();
    }

    public void OnShootHit(Vector3 hitPoint)
    {
        //display blood animation.
        Instantiate(blood, hitPoint, Quaternion.identity);

        health.Damage(damage);
        patrolMovement.OnAware();
    }
}
