using System.Collections;
using UnityEngine;

public class FllowBullet : MonoBehaviour
{
    public float speed = 100;
    public int damage = 5;
    public float destroyTime = 120f;
    public AudioClip destroyClip;
    public GameObject bulletModel;

    [Header("Effect On Blast")]
    public GameObject effect;
    public float effectLength = 0;

    private Vector3 target;
    private AudioSource audio;
    void Start()
    {
        audio = GetComponent<AudioSource>();
        target = new Vector3(GameManager.instance.location.position.x, GameManager.instance.location.position.y + 1f, GameManager.instance.location.position.z);
        Destroy(gameObject, destroyTime);
    }

    private bool reachedDestination = false;
    void Update()
    {
        if (!reachedDestination)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

            if (transform.position.x == target.x && transform.position.y == target.y && transform.position.z == target.z)
            {
                reachedDestination = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            DestroyBullet();
            GameManager.instance.health.TakeDamage(damage);
        }
    }

    private void DestroyBullet()
    {
        audio.Stop();
        audio.PlayOneShot(destroyClip);
        bulletModel.SetActive(false);
        if (effect != null)
            Instantiate(effect, transform.position, Quaternion.identity);

        float destroyAfter = effectLength > destroyClip.length ? effectLength : destroyClip.length;
        StartCoroutine(DestroyTimer(destroyAfter + 0.2f));
    }

    private IEnumerator DestroyTimer(float waiting = 0.3f)
    {
        yield return new WaitForSeconds(waiting);
        Destroy(gameObject);
    }
}
