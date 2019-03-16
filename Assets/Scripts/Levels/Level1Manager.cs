using System.Collections.Generic;
using UnityEngine;

public class Level1Manager : LevelManager
{
    [Header("Managers")]
    public EnemyManager enemyManager;

    [Header("Destroyable Items")]
    public float spawnTime = 10f;
    public int maxDestroyableItems = 2;
    public Transform spawnTransform;
    public GameObject[] destroyables;

    [Header("Ending Area")]
    public KeyCode activateItemKey;
    public Entrance areaEntranceColider;
    public Entrance portalEntranceColider;
    public GameObject endingArena;
    public GameObject lightCone;
    public GameObject portal;

    [Header("Sounds")]
    public AudioClip background;
    public AudioClip bossBackground;

    private Dictionary<int, string> goals;
    private int lastGoal = 0;
    private bool portalHasOpened = false;
    private bool itemWascollected = false;

    void Start()
    {
        InvokeRepeating("SpawnDestroyables", spawnTime, spawnTime);
        GameManager.instance.soundManager.SwitchSound(background);

        goals = new Dictionary<int, string>
        {
            { 1, "Defeat the minions of Shuro Chi" },
            { 2, "Defeat Shuro Chi - the Fire Wizard" },
            { 3, "Collect the Fire Totem" },
            { 4, "Find a way out" },
            { 5, "Go to floating area" },
            { 6, "Open the portal by pressing " + activateItemKey },
            { 7, "Go into the portal" }
        };

        Cursor.visible = false;       
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

        var random = Random.Range(0, this.destroyables.Length);
        var position = new Vector3(spawnTransform.position.x + Random.Range(0, spawnTransform.lossyScale.x / 2), spawnTransform.position.y, spawnTransform.position.z + Random.Range(0, spawnTransform.lossyScale.z / 2));
        Instantiate(this.destroyables[random], position, Quaternion.identity);
    }

    void Update()
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

            //stage 3:: Collect the fire totem.
            if (boss != null && !endingArena.activeInHierarchy)
            {
                var health = boss.GetComponent<EnemyHealth>();
                if (health.isDead)
                {
                    GameManager.instance.soundManager.SwitchSound(background);

                    setGoal(3);
                    endingArena.SetActive(true);

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
            if (GameManager.instance.level1Item.activeInHierarchy)
                itemWascollected = true;

            //stage 4:: Go to the next level.
            if (lastGoal < 4)
            {
                setGoal(4);
                GameManager.instance.level1Item.SetActive(true);
            }
            else if (itemWascollected)
            {
                //stage 6:: Open the portal by pressing Q.
                if (areaEntranceColider.isEntered)
                {
                    if (!portalHasOpened)
                    {
                        setGoal(6);
                    }

                    if (Input.GetKeyDown(activateItemKey) || portalHasOpened)
                    {
                        portalHasOpened = true;
                        lightCone.SetActive(false);
                        portal.SetActive(true);

                        //stage 6:: Go into the portal.
                        setGoal(7);
                        if (portalEntranceColider.isEntered)
                        {
                            GameManager.instance.SwitchScenes(Scene.Level2);
                        }
                    }
                }
                else if (lastGoal > 5)
                {
                    //stage 5:: Go back to floating area.
                    forceGoal(5);
                }
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

    private void forceGoal(int stage)
    {
        lastGoal = stage;
        GameManager.instance.UpdateGoal(goals[stage]);
    }
}
