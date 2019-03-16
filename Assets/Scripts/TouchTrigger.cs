using UnityEngine;

public class TouchTrigger : MonoBehaviour
{
    public Transform respawnPoint;

    private void Awake()
    {
        GameManager.instance.location.position = respawnPoint.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameManager.instance.health.SetHealth(0f);
        }
    }
}
