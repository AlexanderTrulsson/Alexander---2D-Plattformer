using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    public float timeRemaining = 10f;                // Total time for the countdown
    public bool timerIsRunning = false;              // Controls if the timer is active
    public TextMeshProUGUI timerText;                
    public GameObject timerUI;                       

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
                timeRemaining -= Time.deltaTime;  
                UpdateTimerDisplay(timeRemaining);
            }
            else
            {
                
                timeRemaining = 0;
                timerIsRunning = false;
                UpdateTimerDisplay(timeRemaining);
                timerUI.SetActive(false);  
                Debug.Log("Time has run out and timer is hidden!");
            }
        }
    }

    // Called when the player enters the trigger zone to start the timer
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !timerIsRunning)  
        {
            timerIsRunning = true;        
            //timeRemaining = timeRemaining;          
            timerUI.SetActive(true);      
            Debug.Log("Timer started!");
        }
    }

    
    public void PauseTimer()
    {
        timerIsRunning = false;  // Stop the timer
        Debug.Log("Timer paused!");
    }

    
    void UpdateTimerDisplay(float timeToDisplay)
    {
        timeToDisplay += 1;  

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);  // Get minutes
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);  // Get seconds

        
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
