using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemy;                // The enemy prefab to be spawned.
    public GameObject enemyBoss;            // The enemy boss prefab to be spawned.
    public bool spawnOnTimer = false;
    public float spawnTime = 3f;            // How long between each spawn.
    public int startSpawnEnemiesCount = 10;
    public Transform[] spawnPoints;         // An array of the spawn points this enemy can spawn from.
    public GameObject[] collectables;
    [Header("Boss Sector")]
    public Transform bossSpawnTransform;
    public GameObject specialItem;
    public bool summonEnemiesWhenBossSpawn = false;
    public int maxEnemiesPerTime = 0;

    public bool bossWasSpawn { get; private set; }
    public bool bossIsDead { get; private set; }

    void Start()
    {
        bossWasSpawn = false;
        bossIsDead = false;

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
            var instance = Instantiate(enemyBoss, bossSpawnTransform.position, bossSpawnTransform.rotation);
            instance.GetComponent<EnemyHealth>().OnDeath += OnBossDeath;
        }
        else if (summonEnemiesWhenBossSpawn && !bossIsDead)
        {
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemies.Length >= maxEnemiesPerTime)
                return;

            for (int i = 0; i < maxEnemiesPerTime - enemies.Length; i++)
            {
                Spawn();
            }
        }
    }

    void Spawn()
    {
        // If the player has no health left
        if (GameManager.instance.health.currentHealth <= 0f)
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
        var random = Random.Range(0, collectables.Length);
        Instantiate(collectables[random], new Vector3(location.position.x, location.position.y + 1f, location.position.z), Quaternion.identity);
    }

    public void OnBossDeath(Transform location)
    {
        bossIsDead = true;
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemies.Length; i++)
        {
            Destroy(enemies[i]);
        }

        Instantiate(specialItem, new Vector3(location.position.x, location.position.y + 1f, location.position.z), Quaternion.identity);
    }
}
