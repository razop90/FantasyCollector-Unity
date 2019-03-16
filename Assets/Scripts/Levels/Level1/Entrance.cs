using UnityEngine;

public class Entrance : MonoBehaviour
{
    public bool isEntered { get; private set; }

    private void Awake()
    {
        isEntered = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            isEntered = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            isEntered = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            isEntered = false;
        }
    }
}
