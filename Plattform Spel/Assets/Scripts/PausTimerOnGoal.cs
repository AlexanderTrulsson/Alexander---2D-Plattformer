using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausTimerOnGoal : MonoBehaviour
{
    public CountdownTimer countdownTimer;  // Reference to the CountdownTimer script

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            countdownTimer.PauseTimer();  // Call the PauseTimer method from the CountdownTimer script
            Debug.Log("Player reached the goal! Timer paused.");
        }
    }
}
