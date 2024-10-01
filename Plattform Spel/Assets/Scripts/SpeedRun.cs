using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpeedRun : MonoBehaviour
{
    [SerializeField] private float timeLimit = 10f;  
    //[SerializeField] private string nextSceneName;  // not used anymore

    private Coroutine destroyCoroutine;             
    private bool isDestructionScheduled = false;    
    private bool destructionCancelled = false;      

    // Reference to the Playermovement script to call Respawn()
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

  
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isDestructionScheduled && !destructionCancelled)
        {
            
            if (destroyCoroutine != null)
            {
                StopCoroutine(destroyCoroutine);
                isDestructionScheduled = false;
                destructionCancelled = true;  
                Debug.Log("Destruction process stopped!");
            }
        }
    }

    
    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(timeLimit);  

        // Only call Respawn() if the destruction hasn't been canceled
        if (!destructionCancelled)
        {
            if (playerMovement != null)
            {
                playerMovement.Respawn();  // Call the Respawn method from Playermovement script
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
