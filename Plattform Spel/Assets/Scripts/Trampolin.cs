using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampolin : MonoBehaviour
{

    [SerializeField] private float jumpy = 200f;
    [SerializeField] private float jumpX = 200f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D playerRigidbody = other.GetComponent<Rigidbody2D>();
            playerRigidbody.velocity = new Vector2(0, 0);
            playerRigidbody.AddForce(new Vector2(jumpX, jumpy));
            GetComponent<Animator>().SetTrigger("Jump");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
