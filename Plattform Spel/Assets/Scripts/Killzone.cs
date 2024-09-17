using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Killzone : MonoBehaviour
{
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private Slider healthSlider;
    private int startingHealth = 5;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            healthSlider.value = startingHealth;
            other.transform.position = spawnPosition.position;
            other.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }
}
