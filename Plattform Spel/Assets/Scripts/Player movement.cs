using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class Playermovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float Jumpforce = 300f;
    [SerializeField] private Transform LeftFoot, RightFoot;
    [SerializeField] private Transform spawnPosition;

    [SerializeField] private LayerMask whatisGround;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text melonText;
    [SerializeField] private AudioClip jumpSound, pickupSound, strawberrySound, plattformSound, trampolineSound, rockDeath, frogSound, trophySound;
    [SerializeField] private GameObject melonParticles, rocketParticles, strawberryParticles;

    private float horizontalValue;
    private bool isGrounded;
    private bool canMove;
    private Rigidbody2D rgdb;
    private SpriteRenderer rend;
    private Animator anim;
    private float rayDistance = 0.25f;
    private int startingHealth = 2;
    private int currentHealth = 0;
    public int melonsCollected = 0;
    private AudioSource audioSource;

    private RectTransform melonTextRectTransform;

    // New variable to check if the player is dead
    private bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        canMove = true;
        currentHealth = startingHealth;
        melonText.text = "" + melonsCollected;

        melonTextRectTransform = melonText.GetComponent<RectTransform>(); // Get RectTransform component
        UpdateMelonText();

        rgdb = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // If the player is dead, don't allow any input.
        if (isDead)
            return;

        horizontalValue = Input.GetAxis("Horizontal");

        if (horizontalValue < 0)
        {
            FlipSprite(true);
        }
        else if (horizontalValue > 0)
        {
            FlipSprite(false);
        }

        if (Input.GetButtonDown("Jump") && CheckIfGrounded())
        {
            Jump();
        }

        anim.SetFloat("MoveSpeed", Mathf.Abs(rgdb.velocity.x));
        anim.SetFloat("VerticalSpeed", rgdb.velocity.y);
        anim.SetBool("IsGrounded", CheckIfGrounded());
    }

    private void FixedUpdate()
    {
        if (!canMove || isDead)
        {
            return;
        }

        rgdb.velocity = new Vector2(horizontalValue * moveSpeed * Time.deltaTime, rgdb.velocity.y);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Melon"))
        {
            Destroy(other.gameObject);
            melonsCollected++;
            UpdateMelonText();
            audioSource.pitch = Random.Range(0.92f, 1.08f);
            audioSource.PlayOneShot(pickupSound, 0.15f);
            Instantiate(melonParticles, other.transform.position, Quaternion.identity);
        }

        if (other.CompareTag("Health"))
        {
            RestoreHealth(other.gameObject);
        }

        if (other.CompareTag("Strawberry"))
        {
            audioSource.PlayOneShot(strawberrySound, 0.15f);
            Instantiate(strawberryParticles, other.transform.position, Quaternion.identity);
            Invoke("PlaySound", 0.5f);
        }
        if (other.CompareTag("Trampolin"))
        {
            audioSource.PlayOneShot(trampolineSound, 0.23f);
        }

        if (other.CompareTag("Enemy"))
        {
            audioSource.PlayOneShot(rockDeath, 0.16f);
        }

        if (other.CompareTag("Frog"))
        {
            audioSource.PlayOneShot(frogSound, 0.4f);
        }

        if (other.CompareTag("Trophy"))
        {
            audioSource.PlayOneShot(trophySound, 0.26f);
        }
    }

    void PlaySound()
    {
        audioSource.PlayOneShot(plattformSound, 0.4f); // Play the assigned sound
    }

    private void UpdateMelonText()
    {
        melonText.text = melonsCollected.ToString(); // Update text

        // Adjust the position based on the number of melons collected
        Vector2 newPosition = melonTextRectTransform.anchoredPosition;

        if (melonsCollected >= 10) // Adjust this threshold if needed
        {
            // Shift left for double-digit numbers
            newPosition.x = -98f; // Adjust this value as needed for your layout
        }
        else
        {
            // Reset position for single-digit numbers
            newPosition.x = -83f;
        }

        melonTextRectTransform.anchoredPosition = newPosition; // Apply new position
    }

    private void RestoreHealth(GameObject healthPickup)
    {
        if (currentHealth >= startingHealth)
        {
            return;
        }
        else
        {
            currentHealth += 3;
            updatehealthBar();
            Destroy(healthPickup);

            if (currentHealth >= startingHealth)
            {
                currentHealth = startingHealth;
            }
        }
    }

    private void FlipSprite(bool direction)
    {
        rend.flipX = direction;
    }

    private void Jump()
    {
        rgdb.AddForce(new Vector2(0, Jumpforce));
        audioSource.PlayOneShot(jumpSound, 0.5f);
        Instantiate(rocketParticles, transform.position, rocketParticles.transform.localRotation);
    }

    // Modified TakeDamage to trigger a die animation and delay respawn for 1 second.
    // It also immediately stops movement and cancels knockback sliding.
    public void TakeDamage(int damageAmount)
    {
        if (isDead)
            return;

        currentHealth -= damageAmount;
        healthSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            // Set dead state, disable movement, cancel current velocity,
            // and disable physics simulation to stop any sliding.
            isDead = true;
            canMove = false;
            rgdb.velocity = Vector2.zero;
            rgdb.simulated = false;

            // Trigger the death animation.
            anim.SetTrigger("Die");

            // Start a coroutine to delay respawn for 1 second.
            StartCoroutine(DelayedRespawn());
        }
        else
        {
            // For non-lethal damage, trigger the hurt animation.
            anim.SetTrigger("Hurt");
        }
    }

    public void TakeKnockBack(float knockbackForce, float upwards)
    {
        if (isDead)
            return;

        canMove = false;
        rgdb.AddForce(new Vector2(knockbackForce, upwards));
        anim.SetTrigger("Hurt");
        Invoke("canmoveAgain", 0.25f);
    }

    private void canmoveAgain()
    {
        if (!isDead)
            canMove = true;
    }

    public void UpdateSpawnPosition(Transform newSpawn)
    {
        spawnPosition = newSpawn;
        Debug.Log("Checkpoint reached! New spawn position: " + spawnPosition.position);
    }


    // Instead of just repositioning the player, we reload the entire scene upon respawn.
    public void Respawn()
    {
        // Reset health and update UI.
        currentHealth = startingHealth;
        healthSlider.value = currentHealth;

        // Clear any lingering velocity and re-enable physics simulation.
        rgdb.velocity = Vector2.zero;
        rgdb.simulated = true;  // <== Re-enable physics here

        // Move the player to the designated spawn position.
        transform.position = spawnPosition.position;

        // Reset the player's animation state to idle.
        anim.Play("KNIGHT_IDLE");

        // Re-enable player controls and reset death state.
        canMove = true;
        isDead = false;
    }



    // Coroutine to delay respawn for 1 second after the death animation.
    private IEnumerator DelayedRespawn()
    {
        yield return new WaitForSeconds(1f);
        Respawn();
    }

    private void updatehealthBar()
    {
        healthSlider.value = currentHealth;
    }

    private bool CheckIfGrounded()
    {
        RaycastHit2D leftHit = Physics2D.Raycast(LeftFoot.position, Vector2.down, rayDistance, whatisGround);
        RaycastHit2D rightHit = Physics2D.Raycast(RightFoot.position, Vector2.down, rayDistance, whatisGround);

        if ((leftHit.collider != null && leftHit.collider.CompareTag("Ground")) ||
            (rightHit.collider != null && rightHit.collider.CompareTag("Ground")))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
