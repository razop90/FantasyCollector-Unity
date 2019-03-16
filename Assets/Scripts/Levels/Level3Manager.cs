using System.Collections.Generic;
using UnityEngine;

public class Level3Manager : LevelManager
{
    [Header("Sound")]
    public AudioClip background;
    [Header("Player")]
    public Transform playerSpawnTransform;

    [Header("Treasure Boxes")]
    public TreasureBox airTotemBox;
    public TreasureBox waterTotemBox;

    public bool isTotemsCollected { get; private set; }
    private Dictionary<int, string> goals;

    void Start()
    {
        GameManager.instance.soundManager.SwitchSound(background);
        GameManager.instance.location.position = playerSpawnTransform.position;
        GameManager.instance.aim.disableAim = true;

        goals = new Dictionary<int, string>
        {
            { 1, "Find the rest of totems" },
            { 2, "Talk to Yuki" }
        };

        setGoal(1);
        Cursor.visible = false;
    }

    void Update()
    {
        if (!GameManager.instance.level3Item1.activeInHierarchy && airTotemBox.isCollected)
            GameManager.instance.level3Item1.SetActive(true);
        if (!GameManager.instance.level3Item2.activeInHierarchy && waterTotemBox.isCollected)
            GameManager.instance.level3Item2.SetActive(true);

        //stage 1:: ind the rest of totems.
        if (airTotemBox.isCollected && waterTotemBox.isCollected)
        {
            //stage 2:: Talk to Yuki.
            setGoal(2);
            isTotemsCollected = true;
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
