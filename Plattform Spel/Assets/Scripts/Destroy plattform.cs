using UnityEngine;

public class DestroyPlatform : MonoBehaviour
{
    public GameObject platform; // Assign the platform in the Inspector

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Trigger the platform's rising behavior
            platform.GetComponent<PlatformController>().RisePlatform();
            // Destroy the strawberry immediately
            Destroy(gameObject);
        }
    }
}

