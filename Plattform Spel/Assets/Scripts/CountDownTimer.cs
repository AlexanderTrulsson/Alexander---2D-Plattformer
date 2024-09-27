using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    public float timeRemaining = 10f;                // Total time for the countdown
    public bool timerIsRunning = false;              // Controls if the timer is active
    public TextMeshProUGUI timerText;                // Reference to the TextMeshProUGUI component
    public GameObject timerUI;                       // Reference to the UI element (to show/hide)

    private void Start()
    {
        // Initially hide the timer UI
        timerUI.SetActive(false);
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;  // Reduce the time each frame
                UpdateTimerDisplay(timeRemaining);
            }
            else
            {
                // Timer has reached zero
                timeRemaining = 0;
                timerIsRunning = false;
                UpdateTimerDisplay(timeRemaining);
                timerUI.SetActive(false);  // Hide the timer UI
                Debug.Log("Time has run out and timer is hidden!");
            }
        }
    }

    // Called when the player enters the trigger zone to start the timer
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !timerIsRunning)  // Check if the player enters the trigger
        {
            timerIsRunning = true;        // Start the timer
            timeRemaining = timeRemaining;          // Reset the timer (if needed)
            timerUI.SetActive(true);      // Show the timer UI
            Debug.Log("Timer started!");
        }
    }

    // Method to pause the timer when reaching the goal
    public void PauseTimer()
    {
        timerIsRunning = false;  // Stop the timer
        Debug.Log("Timer paused!");
    }

    // Method to update the UI text to show the remaining time in minutes and seconds
    void UpdateTimerDisplay(float timeToDisplay)
    {
        timeToDisplay += 1;  // Adjust to make the timer display more accurately

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);  // Get minutes
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);  // Get seconds

        // Update the TextMeshProUGUI text to display the time
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
