using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Playermovement player = other.GetComponent<Playermovement>();
            if (player != null)
            {
                if (player.melonsCollected >= 5)
                {
                    player.UpdateSpawnPosition(transform);
                    Debug.Log("Checkpoint reached! New spawn position set to: " + transform.position);
                }
                else
                {
                    Debug.Log("Not enough pickups collected to activate checkpoint.");
                }
            }
        }
    }
}
