using Assets.Scripts;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Scene { Menu = 0, Global = 1, Level1 = 2, Level2 = 3, Level3 = 4 }
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Global")]
    public Text goalText;
    public AudioClip goalSound;
    public Fading fading;
    public SoundManager soundManager;

    [Header("Player")]
    public Transform location;
    public PlayerHealth health;
    public AimBehaviourBasic aim;
    public BasicBehaviour playerBehaviour;
    public Shooting shooting;
    public LevelCollectable levelCollectable;
    public Animator playerAnimator;

    [Header("Boss Items Indication")]
    public GameObject level1Item;
    public GameObject level2Item;
    public GameObject level3Item1;
    public GameObject level3Item2;

    public bool playerIsPickingAnItem { get; set; }
    public bool isGameOperational { get; set; }
    private Animator animator;
    private AudioSource audioSource;
    private Scene currentScene = Scene.Level1;
    private bool gameStarted = false;

    private void Awake()
    {
        if (!gameStarted)
        {
            instance = this;
            gameStarted = true;
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();

            SceneManager.LoadSceneAsync((int)currentScene, LoadSceneMode.Additive);

            FloatingTextHandler.Initialize();

            isGameOperational = true;
            playerIsPickingAnItem = false;
            VictoryMenu.IsGameOver = false;
            PauseMenu.GameIsPaused = false;
            GameOverMenu.IsGameOver = false;
        }
    }

    private void Update()
    {
        isGameOperational = VictoryMenu.IsGameOver || PauseMenu.GameIsPaused || GameOverMenu.IsGameOver || playerIsPickingAnItem ? false : true;
    }

    public void UpdateGoal(string text)
    {
        goalText.text = text;
        audioSource.PlayOneShot(goalSound);
    }

    public void SwitchScenes(Scene scene, LoadSceneMode mode = LoadSceneMode.Additive)
    {
        if (currentScene != scene)
        {
            SwitchScenes((int)scene, mode);
        }
    }

    private void SwitchScenes(int toScene, LoadSceneMode mode = LoadSceneMode.Additive)
    {
        instance.UnloadScene((int)currentScene);
        currentScene = (Scene)toScene;
        aim.disableAim = false;

        fading.FadeToScene(toScene, mode);

        #region Reset Game

        health.SetHealth(100f);
        shooting.maxAmmo.SetAmount(100);
        shooting.availableAmmo.SetAmount(100);
        levelCollectable.SetCollectableValue(false);

        if (toScene != (int)Scene.Level1 && toScene != (int)Scene.Level2 && toScene != (int)Scene.Level3)
        {
            level1Item.SetActive(false);
            level2Item.SetActive(false);
            level3Item1.SetActive(false);
            level3Item2.SetActive(false);
        }

        if (aim.aim)
        {
            StartCoroutine(aim.ToggleAimOff());
        }

        var ammos = GameObject.FindGameObjectsWithTag("Collectable Ammo");
        foreach (GameObject ammo in ammos)
        {
            Destroy(ammo);
        }

        var healths = GameObject.FindGameObjectsWithTag("Collectable Health");
        foreach (GameObject health in healths)
        {
            Destroy(health);
        }

        var bombs = GameObject.FindGameObjectsWithTag("Bombs");
        foreach (GameObject bomb in bombs)
        {
            Destroy(bomb);
        }

        var destroyables = GameObject.FindGameObjectsWithTag("Destroyable");
        foreach (GameObject destroyable in destroyables)
        {
            Destroy(destroyable);
        }

        #endregion
    }

    public void UnloadScene(int scene)
    {
        StartCoroutine(Unload(scene));
    }

    IEnumerator Unload(int scene)
    {
        yield return null;
        SceneManager.UnloadSceneAsync(scene);
    }
}


public abstract class LevelManager : MonoBehaviour
{
}
