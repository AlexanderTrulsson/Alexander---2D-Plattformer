using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpeedRun : MonoBehaviour
{
    [SerializeField] private float timeLimit = 10f;  // Time before destruction
    //[SerializeField] private string nextSceneName;   // The name of the next scene to load

    private Coroutine destroyCoroutine;             // Reference to the destruction coroutine
    private bool isDestructionScheduled = false;    // Flag to track if destruction is scheduled
    private bool destructionCancelled = false;      // Flag to confirm if the destruction was cancelled

    // Reference to the PlayerMovement script to call Respawn()
    private Playermovement playerMovement;

    private void Start()
    {
        // Assuming the PlayerMovement script is on the same GameObject or assign it from the inspector
        playerMovement = FindObjectOfType<Playermovement>();  // You can also assign this reference manually in the inspector if needed
    }

    // Called when the player exits the trigger to start the destruction process
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isDestructionScheduled && !destructionCancelled)
        {
            // Start the destruction process if it's not already started
            destroyCoroutine = StartCoroutine(DestroyAfterTime());
            isDestructionScheduled = true;
            Debug.Log("Destruction process started!");
        }
    }

    // Called when the player enters another specific trigger to stop the destruction process
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isDestructionScheduled && !destructionCancelled)
        {
            // Stop the destruction process if it's in progress
            if (destroyCoroutine != null)
            {
                StopCoroutine(destroyCoroutine);
                isDestructionScheduled = false;
                destructionCancelled = true;  // Mark the destruction as cancelled
                Debug.Log("Destruction process stopped!");
            }
        }
    }

    // Coroutine to call the Respawn() method after 'timeLimit' seconds
    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(timeLimit);  // Wait for the specified time

        // Only call Respawn() if the destruction hasn't been cancelled
        if (!destructionCancelled)
        {
            if (playerMovement != null)
            {
                playerMovement.Respawn();  // Call the Respawn method from PlayerMovement script
                Debug.Log("Player respawned!");
            }
            else
            {
                Debug.LogError("PlayerMovement script not found!");
            }
        }
        else
        {
            Debug.Log("Player destruction avoided due to cancellation.");
        }
    }
}
