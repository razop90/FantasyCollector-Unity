using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public string newGameSceneName;
    public Camera menuCamera;
    public GameObject StartGameOptionsPanel;
    public AudioClip buttonClick;

    private AudioSource audioSource;
    private Animator anim;

    void Start()
    {
        Time.timeScale = 1f;
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public void openStartGameOptions()
    {
        StartGameOptionsPanel.SetActive(true);

        anim.Play("buttonTweenAnims_on");
        playClickSound();
    }

    public void newGame()
    {
        playClickSound();
        SceneManager.LoadSceneAsync((int)Scene.Global, LoadSceneMode.Single);
    }

    public void backToMain()
    {
        //simply play anim for CLOSING main options panel
        anim.Play("buttonTweenAnims_off");
        playClickSound();
    }


    public void Quit()
    {
        playClickSound();
        Application.Quit();
    }

    public void playHoverClip()
    {

    }

    private void playClickSound()
    {
        audioSource.PlayOneShot(buttonClick);
    }
}
