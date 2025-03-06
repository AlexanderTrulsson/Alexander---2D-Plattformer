using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement & Detection")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float detectionRadius = 5f; // Radius to detect the player
    [SerializeField] private float attackRange = 1f;       // Range to start attacking

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private int damageGiven = 1;
    [SerializeField] private float knockbackForce = 200f; // Force applied when enemy attacks the player
    [SerializeField] private float upwardForce = 100f;      // Upward force when enemy attacks the player

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;

    [Header("Knockback on Hit")]
    [SerializeField] private float enemyKnockbackForce = 100f; // Knockback force applied when enemy is hit
    [SerializeField] private float enemyUpwardForce = 50f;     // Upward force for enemy knockback

    [Header("Spawn Settings")]
    [SerializeField] private float spawnAnimationDuration = 0.5f; // Duration of the spawn animation

    private int currentHealth;
    private float lastAttackTime;
    private Transform player;
    private SpriteRenderer rend;
    private Animator animator;
    private Rigidbody2D rb;

    // Spawn state variables: before spawning the enemy is inactive.
    private bool isSpawned = false;
    private bool isSpawning = false;

    private void Start()
    {
        currentHealth = maxHealth;
        // Find the player by tag ("Player")
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        rend = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // Initially, hide the enemy and mark it as not spawned.
        isSpawned = false;
        isSpawning = false;
        rend.enabled = false;
    }

    private void Update()
    {
        if (player == null)
            return;

        // While the enemy hasn't spawned, check for the player's proximity and do nothing else.
        if (!isSpawned)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer <= detectionRadius && !isSpawning)
            {
                StartCoroutine(SpawnEnemy());
            }
            return; // Exit Update: no movement, attack, or behavior until spawned.
        }

        // Normal enemy behavior after spawning:
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= detectionRadius)
        {
            if (dist <= attackRange)
            {
                Attack();
            }
            else
            {
                // Chase the player.
                Vector2 direction = (player.position - transform.position).normalized;
                transform.Translate(direction * moveSpeed * Time.deltaTime);

                // Flip the sprite based on movement direction.
                if (direction.x > 0)
                    rend.flipX = true;
                else if (direction.x < 0)
                    rend.flipX = false;
            }
        }
        // (Optional: Add patrolling/idle behavior if player is not within detection radius.)
    }

    private IEnumerator SpawnEnemy()
    {
        isSpawning = true;
        // Enable the enemy’s sprite so it becomes visible.
        rend.enabled = true;
        // Trigger the spawn animation (make sure your Animator has a "Spawn" trigger).
        animator.SetTrigger("Spawn");
        // Wait for the spawn animation to complete.
        yield return new WaitForSeconds(spawnAnimationDuration);
        isSpawned = true;
        isSpawning = false;
    }

    private void Attack()
    {
        // Ensure enemy only attacks after spawning.
        if (!isSpawned)
            return;

        // Prevent attacking too often.
        if (Time.time - lastAttackTime < attackCooldown)
            return;

        lastAttackTime = Time.time;
        animator.SetTrigger("Attack");
        StartCoroutine(DelayedAttack());
    }

    private IEnumerator DelayedAttack()
    {
        yield return new WaitForSeconds(0.6f);
        if (Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            Playermovement playerMovement = player.GetComponent<Playermovement>();
            if (playerMovement != null)
            {
                // Apply knockback to the player.
                if (player.position.x > transform.position.x)
                    playerMovement.TakeKnockBack(knockbackForce, upwardForce);
                else
                    playerMovement.TakeKnockBack(-knockbackForce, upwardForce);
                // Apply damage to the player.
                playerMovement.TakeDamage(damageGiven);
            }
        }
        else
        {
            Debug.Log("Player evaded the attack!");
        }
    }

    // Called when the enemy is hit by the player's attack.
    public void TakeDamage(int damage)
    {
        StartCoroutine(DelayedTakeDamage(damage));
    }

    private IEnumerator DelayedTakeDamage(int damage)
    {
        yield return new WaitForSeconds(0.3f);
        currentHealth -= damage;

        // Apply knockback to the enemy when hit.
        if (player != null && rb != null)
        {
            Vector2 knockbackDirection = (transform.position - player.position).normalized;
            Vector2 knockbackVector = knockbackDirection * enemyKnockbackForce + Vector2.up * enemyUpwardForce;
            rb.AddForce(knockbackVector, ForceMode2D.Impulse);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("Hurt");
        }
    }

    private void Die()
    {
        animator.SetTrigger("Die");
        Destroy(gameObject, 1f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
