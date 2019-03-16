using System.Collections;
using UnityEngine;

public class TreasureBox : MonoBehaviour
{
    public bool isOpened { get; private set; }
    public bool isCollected { get; private set; }
    public AudioClip openedSound;
    public AudioClip collectedSound;
    public GameObject openedEffect;
    public GameObject drop;
    public float dropAfter = 1f;

    private Animator animator;
    private AudioSource audioSource;

    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        isOpened = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !isOpened)
        {
            audioSource.PlayOneShot(openedSound);

            StartCoroutine(CreateDrop(dropAfter));
            isOpened = true;
            animator.SetTrigger("openBox");
        }
    }

    private IEnumerator CreateDrop(float waiting = 0.3f)
    {
        yield return new WaitForSeconds(waiting);
        openedEffect.SetActive(true);
        var newDrop = Instantiate(drop, new Vector3(transform.position.x, transform.position.y + 1.2f, transform.position.z), Quaternion.identity);
        newDrop.GetComponent<TreasureBoxDrop>().OnDropCollected += (tag) =>
        {
            audioSource.PlayOneShot(collectedSound);
            isCollected = true;

            StartCoroutine(RemoveBoxEffect());          
        };
    }

    private IEnumerator RemoveBoxEffect(float waiting = 0.5f)
    {
        yield return new WaitForSeconds(waiting);
        openedEffect.SetActive(false);
    }
}
