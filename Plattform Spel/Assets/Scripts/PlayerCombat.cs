using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;

    [SerializeField] private float attackCooldown = 0.5f; // Time delay between attacks
    private float lastAttackTime = -Mathf.Infinity;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Check if the attack button is pressed and if enough time has passed since the last attack
        if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time - lastAttackTime >= attackCooldown)
        {
            Attack();
        }
    }

    void Attack()
    {
        // Update the last attack time
        lastAttackTime = Time.time;

        // Play attack animation
        animator.SetTrigger("Attack");

        // Detect enemies within the attack range.
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Determine the facing direction based on the sprite's flip state.
        Vector2 facingDirection = spriteRenderer.flipX ? Vector2.left : Vector2.right;

        // Damage only enemies in front of the player.
        foreach (Collider2D enemy in hitEnemies)
        {
            Vector2 directionToEnemy = (enemy.transform.position - transform.position).normalized;

            // Dot product > 0.5 means the enemy is mostly in front of the player.
            if (Vector2.Dot(facingDirection, directionToEnemy) > 0.5f)
            {
                Debug.Log("We hit " + enemy.name);
                enemy.GetComponent<EnemyMovement>()?.TakeDamage(1);
            }
        }
    }

    // Visualize the attack range in the Scene view
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}


