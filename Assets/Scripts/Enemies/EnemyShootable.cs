using System.Collections;
using System.Collections.Generic;
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

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("collides: " + other.tag);
        if (other.tag == "Shoot")
        {
            Destroy(other.gameObject);

            //display blood animation.
            Instantiate(blood, new Vector3(transform.position.x, other.transform.position.y, transform.position.z), Quaternion.identity);

            health.Damage(damage);
            patrolMovement.OnAware();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
