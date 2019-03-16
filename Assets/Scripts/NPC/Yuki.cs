using System.Collections;
using UnityEngine;

public class Yuki : MonoBehaviour
{
    public Level3Manager levelManager;
    public GameObject talkingText;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (levelManager.isTotemsCollected && other.tag == "Player")
        {
            talkingText.SetActive(true);

            animator.SetTrigger("happy");
            AnimatorClipInfo[] clipInfos = animator.GetCurrentAnimatorClipInfo(0);
            StartCoroutine(EndGame(clipInfos[0].clip.length + 5f));
        }
    }
    private IEnumerator EndGame(float waiting = 0.3f)
    {
        yield return new WaitForSeconds(waiting);
        VictoryMenu.IsGameOver = true;
    }
}
