using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{

    private Animator anim;
    private bool hasPlayedAnimation;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasPlayedAnimation)
        {
            hasPlayedAnimation = true;
            anim.SetTrigger("Move");
        }
    }
}
