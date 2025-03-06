using UnityEngine;
using UnityEngine.SceneManagement;

public class Killzone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var lightComponent = other.GetComponent<PlayerToLight>();
            if (lightComponent != null && lightComponent.isTransformed)
            {
                // Set the flag to abort transformation early.
                lightComponent.abortTransformation = true;
            }
            else
            {
                // For a non-transformed player, use your regular respawn logic.
                other.GetComponent<Playermovement>().Respawn();
            }
        }
    }

}
