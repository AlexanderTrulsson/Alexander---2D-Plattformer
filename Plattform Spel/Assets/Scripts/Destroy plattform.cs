using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyplattform : MonoBehaviour
{
    public GameObject platform; // Assign the platform in the Inspector

    void OnTriggerEnter2D(Collider2D other)
    {
 

        if (other.CompareTag("Player"))
        {
            Destroy(gameObject); // Destroy the strawberry
            Destroy(platform, 0.5f); // Destroy the platform
           
        }
    }
}
