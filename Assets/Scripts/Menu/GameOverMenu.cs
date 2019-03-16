using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public static bool IsGameOver = false;
    public GameObject pauseMenuUI;
    public AudioClip audioClip;

    private void Update()
    {
        if (GameManager.instance != null && GameManager.instance.health.isDead && !IsGameOver)
        {
            GameManager.instance.soundManager.SwitchSound(audioClip);
            Cursor.visible = true;
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            IsGameOver = true;
        }
    }

    public void LoadMenu()
    {
        GameManager.instance.SwitchScenes(Scene.Menu, LoadSceneMode.Single);
        Time.timeScale = 1f;
        IsGameOver = false;
    }

    public void QuitGame()
    {
        Debug.Log("On Quit");
        Application.Quit();
    }
}
