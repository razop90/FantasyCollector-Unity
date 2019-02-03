using UnityEngine;

public class TouchTrigger : MonoBehaviour
{
    public Transform player;
    public Transform respawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (player != null && other.tag == "Player")
        {
            player.transform.position = respawnPoint.transform.position;
        }
    }
}
