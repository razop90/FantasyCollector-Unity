using UnityEngine;

public class Destructible : MonoBehaviour
{
    public GameObject destroyedVersion;
    public GameObject[] collectables;

    public void OnShootHit(Vector3 hitPoint)
    {
        destroyWithReplacement();
    }

    void OnCollisionEnter(Collision theCollision)
    {
        if (theCollision.gameObject.tag == "Player")
        {
            destroyWithReplacement();
        }
    }

    private void destroyWithReplacement()
    {
        var replacement = Instantiate(destroyedVersion, transform.position, transform.rotation);
        Destroy(gameObject);
        Destroy(replacement, 10);
        spawnCollectable();
    }

    private void spawnCollectable()
    {
        var random = Random.Range(0, collectables.Length);
        Instantiate(collectables[random], new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), Quaternion.identity);
    }
}
