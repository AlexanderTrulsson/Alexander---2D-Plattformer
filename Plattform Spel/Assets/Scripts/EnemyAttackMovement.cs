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

    private int currentHealth;
    private float lastAttackTime;
    private Transform player;
    private SpriteRenderer rend;
    private Animator animator;
    private Rigidbody2D rb;

    private void Start()
    {
        currentHealth = maxHealth;
        // Find the player in the scene by its tag ("Player")
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        rend = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (player == null)
            return;

        // Determine distance from enemy to player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // If player is within detection radius, begin behavior
        if (distanceToPlayer <= detectionRadius)
        {
            // If the player is within attack range, attack
            if (distanceToPlayer <= attackRange)
            {
                Attack();
            }
            else
            {
                // Otherwise, chase the player
                Vector2 direction = (player.position - transform.position).normalized;
                transform.Translate(direction * moveSpeed * Time.deltaTime);

                // Flip the sprite based on movement direction
                if (direction.x > 0)
                    rend.flipX = true;
                else if (direction.x < 0)
                    rend.flipX = false;
            }
        }
        // Optional: Add behavior for when the player is not in range (like patrolling)
    }

    private void Attack()
    {
        // Prevent attacking too often using a cooldown
        if (Time.time - lastAttackTime < attackCooldown)
            return;

        lastAttackTime = Time.time;

        // Trigger the enemy attack animation
        animator.SetTrigger("Attack");

        // Start a coroutine that delays the damage by 0.3 seconds
        StartCoroutine(DelayedAttack());
    }

    private IEnumerator DelayedAttack()
    {
        // Wait for 0.3 seconds before checking if the player is still within range
        yield return new WaitForSeconds(0.5f);

        // If the player is still within the attack range, apply damage
        if (Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            Playermovement playerMovement = player.GetComponent<Playermovement>();
            if (playerMovement != null)
            {
                // Apply knockback relative to positions for player's damage
                if (player.position.x > transform.position.x)
                    playerMovement.TakeKnockBack(knockbackForce, upwardForce);
                else
                    playerMovement.TakeKnockBack(-knockbackForce, upwardForce);

                // Apply damage to the player
                playerMovement.TakeDamage(damageGiven);
            }
        }
        else
        {
            Debug.Log("Player evaded the attack!");
        }
    }

    // This method is called by the player's attack script when you hit the enemy.
    // It now delays applying damage (and enemy knockback) by 0.3 seconds.
    public void TakeDamage(int damage)
    {
        StartCoroutine(DelayedTakeDamage(damage));
    }

    private IEnumerator DelayedTakeDamage(int damage)
    {
        // Wait for 0.3 seconds before applying damage and knockback to the enemy
        yield return new WaitForSeconds(0.3f);

        currentHealth -= damage;

        // Apply knockback to the enemy when hit by the player
        if (player != null && rb != null)
        {
            // Calculate knockback direction (from the player to the enemy)
            Vector2 knockbackDirection = (transform.position - player.position).normalized;
            // Combine horizontal and upward force
            Vector2 knockbackVector = knockbackDirection * enemyKnockbackForce + Vector2.up * enemyUpwardForce;
            rb.AddForce(knockbackVector, ForceMode2D.Impulse);
        }

        // Optional: Trigger a hurt animation here

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Trigger death animation and then destroy the enemy object after a short delay
        animator.SetTrigger("Die");
        Destroy(gameObject, 1f);
    }

    // Draw the attack range in the Scene view when the enemy is selected.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
