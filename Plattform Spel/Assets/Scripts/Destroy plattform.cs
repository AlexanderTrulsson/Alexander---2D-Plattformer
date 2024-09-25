using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPlatform : MonoBehaviour
{
    public GameObject platform; // Assign the platform in the Inspector

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject); // Destroy the strawberry immediately
            Destroy(platform, 0.5f); // Destroy the platform after 0.5 seconds
        }
    }
}


