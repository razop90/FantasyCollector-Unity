using UnityEngine;

public class EnemyManager : MonoBehaviour
{
   // public PlayerHealth playerHealth;       // Reference to the player's heatlh.
    public GameObject enemy;                // The enemy prefab to be spawned.
    public GameObject enemyBoss;            // The enemy boss prefab to be spawned.
    public float spawnTime = 3f;            // How long between each spawn.
    public Transform[] spawnPoints;         // An array of the spawn points this enemy can spawn from.
    public int startSpawnEnemiesCount = 10;
    public bool spawnOnTimer = false;

    public GameObject healthPrefab;
    public GameObject ammoPrefab;

    public bool bossWasSpawn { get; private set; }

    void Start()
    {
        bossWasSpawn = false;

        // Call the Spawn function after a delay of the spawnTime and then continue to call after the same amount of time.
        if (spawnOnTimer)
            InvokeRepeating("Spawn", spawnTime, spawnTime);

        for (int i = 0; i < startSpawnEnemiesCount; i++)
        {
            Spawn();
        }
    }

    private void Update()
    {
        if (!bossWasSpawn)
        {
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var enemy in enemies)
            {
                var health = enemy.GetComponent<EnemyHealth>();
                if (!health.isDead)
                    return;
            }

            bossWasSpawn = true;
            Instantiate(enemyBoss, spawnPoints[0].position, spawnPoints[0].rotation);
        }
    }

    void Spawn()
    {
        // If the player has no health left
        if (GameManager.instance.playerHealth.currentHealth <= 0f)
        {
            return;
        }

        int spawnPointIndex = Random.Range(0, spawnPoints.Length);

        // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation
        var instance = Instantiate(enemy, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
        instance.GetComponent<EnemyHealth>().OnDeath += OnEnemyDeath;
    }

    public void OnEnemyDeath(Transform location)
    {
        var summon = Random.Range(0, 7);
        var item = healthPrefab;
        if (summon > 3)
        {
            item = ammoPrefab;
        }

        Instantiate(item, new Vector3(location.position.x, location.position.y + 1f, location.position.z), Quaternion.identity);
    }
}
