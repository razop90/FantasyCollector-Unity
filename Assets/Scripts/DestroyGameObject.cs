using UnityEngine;

public class DestroyGameObject : MonoBehaviour
{
    public float destroyAfter = 6f;

    private void Awake()
    {
        Destroy(gameObject, destroyAfter);
    }
}
