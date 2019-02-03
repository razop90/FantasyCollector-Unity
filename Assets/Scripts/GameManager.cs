using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Text goalText;
    public AudioClip goalSound;
    public PlayerHealth playerHealth;
    public Transform playerLocation;

    private AudioSource audioSource;
    private int currentScene = 0;
    private bool gameStarted = false;
    private void Awake()
    {
        if (!gameStarted)
        {
            instance = this;
            audioSource = GetComponent<AudioSource>();

            currentScene = 1;
            SceneManager.LoadSceneAsync(currentScene, LoadSceneMode.Additive);

            gameStarted = true;
        }
    }

    public void UpdateGoal(string text)
    {
        goalText.text = text;
        audioSource.PlayOneShot(goalSound);
    }

    public void SwitchScenes(int toScene)
    {
        if (currentScene != toScene)
        {
            playerHealth.SetHealth(100f);

            instance.UnloadScene(currentScene);
            currentScene = toScene;
            SceneManager.LoadSceneAsync(toScene, LoadSceneMode.Additive);
        }
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
