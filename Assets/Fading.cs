using UnityEngine;
using UnityEngine.SceneManagement;

public class Fading : MonoBehaviour
{
    private int lastScene;
    private LoadSceneMode mode;

    public void FadeToScene(int index, LoadSceneMode mode = LoadSceneMode.Additive)
    {
        this.lastScene = index;
        this.mode = mode;
        GetComponent<Animator>().SetTrigger("FadeOut");
    }

    public void OnFadeOutComplete()
    {
        GetComponent<Animator>().SetTrigger("FadeIn");
        SceneManager.LoadSceneAsync(lastScene, mode);
    }
}
