using System.Collections.Generic;
using UnityEngine;

public class Level2Manager : LevelManager
{
    [Header("Managers")]
    public EnemyManager enemyManager;

    [Header("Destroyable Items")]
    public float spawnTime = 10f;
    public int maxDestroyableItems = 2;
    public Transform spawnTransform;
    public GameObject[] collectables;

    [Header("Ending Area")]
    public Entrance portalEntranceColider;
    public GameObject portal;

    [Header("Player")]
    public Transform playerSpawnTransform;

    [Header("Sounds")]
    public AudioClip background;
    public AudioClip bossBackground;

    private Dictionary<int, string> goals;
    private int lastGoal = 0;
    private bool itemWascollected = false;

    private void Start()
    {
        InvokeRepeating("SpawnDestroyables", spawnTime, spawnTime);
        GameManager.instance.soundManager.SwitchSound(background);

        GameManager.instance.location.position = playerSpawnTransform.position;

        goals = new Dictionary<int, string>
        {
            { 1, "Defeat the protectors of Acnologia" },
            { 2, "Defeat Acnologia - the Earth King" },
            { 3, "Collect the Earth Totem" },
            { 4, "Find the portal and enter" }
        };

        Cursor.visible = false;
    }

    private void Update()
    {
        //stage 1:: Kill all enemies.
        if (!enemyManager.bossWasSpawn)
        {
            setGoal(1);
        }
        //stage 2:: Kill the boss.
        else if (!GameManager.instance.levelCollectable.isBossItemCollected)
        {
            if (lastGoal < 2)
                GameManager.instance.soundManager.SwitchSound(bossBackground);

            setGoal(2);

            var boss = GameObject.FindWithTag("EnemyBoss");

            //stage 3:: Collect the earh totem.
            if (boss != null)
            {
                var health = boss.GetComponent<EnemyHealth>();
                if (health.isDead)
                {
                    GameManager.instance.soundManager.SwitchSound(background);

                    setGoal(3);

                    var bombs = GameObject.FindGameObjectsWithTag("Bombs");
                    foreach (GameObject bomb in bombs)
                    {
                        Destroy(bomb);
                    }
                }
            }
        }
        else
        {
            if (!itemWascollected && GameManager.instance.level2Item.activeInHierarchy)
                itemWascollected = true;

            //stage 4:: Find the portal and enter.
            if (lastGoal < 4)
            {
                setGoal(4);
                GameManager.instance.level2Item.SetActive(true);
                portal.SetActive(true);
            }
            else if (itemWascollected && portalEntranceColider.isEntered)
            {
                //going to the next level.
                GameManager.instance.SwitchScenes(Scene.Level3);
            }
        }
    }

    private void setGoal(int stage)
    {
        if (lastGoal != stage && lastGoal < stage && goals.ContainsKey(stage))
        {
            lastGoal = stage;
            GameManager.instance.UpdateGoal(goals[stage]);
        }
    }

    private void SpawnDestroyables()
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
}
