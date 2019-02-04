using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class Level1Manager : MonoBehaviour
{
    public GameObject endingArena;
    public EnemyManager enemyManager;
    public float spawnTime = 10f;
    public int maxDestroyableItems = 2;
    public Transform spawnTransform;
    public GameObject[] collectables;

    private Dictionary<int, string> goals;
    //private bool loaded = false;

    void Start()
    {
        InvokeRepeating("SpawnDestroyables", spawnTime, spawnTime);

        goals = new Dictionary<int, string>
        {
            { 1, "Defeat all enemies" },
            { 2, "Defeat the boss" },
            { 3, "Go to the next level" }
        };

        Cursor.visible = false;
        FloatingTextHandler.Initialize();
    }

    void SpawnDestroyables()
    {
        //check if there is ammo that hasn't picked yet
        var ammos = GameObject.FindGameObjectsWithTag("Collectable Ammo");
        if (ammos != null && ammos.Length > 0)
        {
            foreach (var ammo in ammos)
            {
                var picked = ammo.GetComponent<AmmoCollectable>().picked;
                if (!picked)
                    return;
            }
        }
        //check if there is health that hasn't picked yet
        var healths = GameObject.FindGameObjectsWithTag("Collectable Health");
        if (healths != null && healths.Length > 0)
        {
            foreach (var health in healths)
            {
                var picked = health.GetComponent<HealthCollectable>().picked;
                if (!picked)
                    return;
            }
        }

        var destroyables = GameObject.FindGameObjectsWithTag("Destroyable");
        if (destroyables != null && destroyables.Length >= maxDestroyableItems)
            return;

        var random = Random.Range(0, collectables.Length);
        var position = new Vector3(spawnTransform.position.x + Random.Range(0, spawnTransform.lossyScale.x / 2), spawnTransform.position.y, spawnTransform.position.z + Random.Range(0, spawnTransform.lossyScale.z / 2));
        Instantiate(collectables[random], position, Quaternion.identity);
    }

    void Update()
    {
        //stage 1:: Kill all enemies.
        if (!enemyManager.bossWasSpawn)
        {
            setGoal(1);
        }
        //stage 2:: Kill the boss.
        else
        {
            setGoal(2);

            var boss = GameObject.FindWithTag("EnemyBoss");

            //stage 3:: Go to the next level.
            if (boss != null && !endingArena.activeInHierarchy)
            {
                var health = boss.GetComponent<EnemyHealth>();
                if (health.isDead)
                {
                    setGoal(3);
                    endingArena.SetActive(true);
                }
            }

            ////load next level.
            //if (!loaded)
            //{
            //    loaded = true;
            //    GameManager.instance.SwitchScenes(2);
            //}
        }
    }

    private int lastGoal = 0;
    private void setGoal(int stage)
    {
        if (lastGoal != stage && lastGoal < stage && goals.ContainsKey(stage))
        {
            lastGoal = stage;
            GameManager.instance.UpdateGoal(goals[stage]);
        }
    }
}
