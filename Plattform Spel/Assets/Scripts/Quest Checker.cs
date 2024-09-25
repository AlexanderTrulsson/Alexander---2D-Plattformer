using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestChecker : MonoBehaviour
{

    [SerializeField] private GameObject dialogBox, finishedText, unfinishedText;
    [SerializeField] private int questGoal = 16;
    [SerializeField] private int leveltoLoad;
    [SerializeField] private AudioClip levelWin, levelFail;

    private Animator anim;
    private AudioSource audioSource;

    private void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.GetComponent<Playermovement>().melonsCollected >= questGoal)
            {
                dialogBox.SetActive(true);
                finishedText.SetActive(true);
                anim.SetTrigger("Flag");
                audioSource.PlayOneShot(levelWin, 0.4f);
                Invoke("LoadNextLevel", 4.5f);
            }
            else
            {
                dialogBox.SetActive(true);
                audioSource.PlayOneShot(levelFail, 0.4f);
                unfinishedText.SetActive(true);
            }
        }
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(leveltoLoad);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            dialogBox.SetActive(false);
            finishedText.SetActive(false);
            unfinishedText.SetActive(false);

        }
    }
}
