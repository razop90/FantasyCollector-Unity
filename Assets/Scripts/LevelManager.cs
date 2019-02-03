using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public GameObject endingArena;
    public EnemyManager enemyManager;
    public Text goalText;
    public AudioClip goalSound;

    private AudioSource audioSource;
    private Dictionary<int, string> goals;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        goals = new Dictionary<int, string>
        {
            { 1, "Defeat all enemies" },
            { 2, "Defeat the boss" },
            { 3, "Go to the next level" }
        };

        Cursor.visible = false;
        FloatingTextHandler.Initialize();
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
        }
    }

    private int lastGoal = 0;
    private void setGoal(int stage)
    {
        if (lastGoal != stage && lastGoal < stage && goals.ContainsKey(stage))
        {
            lastGoal = stage;
            goalText.text = goals[stage];
            audioSource.PlayOneShot(goalSound);
        }
    }
}
