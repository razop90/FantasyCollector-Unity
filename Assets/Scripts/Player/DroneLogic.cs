using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneLogic : MonoBehaviour
{
    // User Inputs
    public float degreesPerSecond = 15.0f;
    public float amplitude = 0.5f;
    public float frequency = 1f;
    public Transform playerTransform;
    // Position Storage Variables
    float startYPosition = 0f;
    float tempPos = 0f;

    // Use this for initialization
    void Start()
    {
        // Store the starting position & rotation of the object
        startYPosition = transform.position.y;
    }


    // Update is called once per frame
    void Update()
    {
        // Spin object around Y-Axis
        //transform.Rotate(new Vector3(0f, Time.deltaTime * degreesPerSecond, 0f), Space.World);

        // Float up/down with a Sin()
        //tempPos = transform.position.y;
        //tempPos += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

        //transform.position = new Vector3(transform.position.x, tempPos, transform.position.z);
    }
}

