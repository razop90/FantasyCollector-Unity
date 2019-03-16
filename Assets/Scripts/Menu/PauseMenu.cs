using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public KeyCode pauseKey = KeyCode.Escape;
    public GameObject pauseMenuUI;

    void Update()
    {
        if (Input.GetKeyDown(pauseKey) && !GameOverMenu.IsGameOver)
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        Cursor.visible = false;

        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void LoadMenu()
    {
        GameManager.instance.SwitchScenes(Scene.Menu, LoadSceneMode.Single);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void QuitGame()
    {
        Debug.Log("On Quit");
        Application.Quit();
    }

    private void Pause()
    {
        Cursor.visible = true;

        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }
}
