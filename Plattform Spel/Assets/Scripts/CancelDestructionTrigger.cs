using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CancelDestructionTrigger : MonoBehaviour
{
    [SerializeField] private SpeedRun speedRunScript;
    [SerializeField] private string nextSceneName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            speedRunScript.OnTriggerEnter2D(other); // Call the cancel destruction method
        }
    }
}
