using UnityEngine;

public class LightTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Teleport the player to the trigger's position.
            other.transform.position = transform.position;

            // Reparent the light (child object) to the player.
            foreach (Transform child in transform)
            {
                child.SetParent(other.transform);
                // Optionally, if there are multiple children and you only want the light,
                // you could check child.name or child.tag here.
                PlayerToLight ptl = other.GetComponent<PlayerToLight>();
                if (ptl != null)
                {
                    ptl.SetAttachedLight(child.gameObject);
                }
            }

            // Start the transformation sequence.
            PlayerToLight playerToLight = other.GetComponent<PlayerToLight>();
            if (playerToLight != null)
            {
                playerToLight.TransformToBall();
            }

            // Destroy the original trigger object (leaving the reparented light with the player).
            Destroy(gameObject);
        }
    }
}
