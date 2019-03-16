using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryMenu : MonoBehaviour
{
    public static bool IsGameOver = false;
    public GameObject victoryMenuUI;
    public AudioClip audioClip;

    private void Update()
    {
        if (IsGameOver)
        {
            GameOverMenu.IsGameOver = true;

            if (audioClip != null)
                GameManager.instance.soundManager.SwitchSound(audioClip);
            Cursor.visible = true;
            victoryMenuUI.SetActive(true);
            Time.timeScale = 0f;
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
