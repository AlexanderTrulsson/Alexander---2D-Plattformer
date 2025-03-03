using System.Collections;
using UnityEngine;

public class PlayerToLight : MonoBehaviour
{
    public Animator animator;
    public float flyingDuration = 2.5f; // Exact time you remain in light form
    public float moveSpeed = 5f;

    private bool isTransformed = false;
    private Rigidbody2D rb;
    private float originalGravityScale;

    // Reference to the reparented light object.
    public GameObject attachedLight;
    // Prefab to spawn after reverting transformation.
    public GameObject lightPrefab;
    // Stores the original world position of the light.
    public Vector3 originalLightPosition;

    // Colliders for swapping: normal (e.g., CapsuleCollider2D) and ball (e.g., CircleCollider2D).
    public Collider2D normalCollider;
    public Collider2D ballCollider;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            originalGravityScale = rb.gravityScale;
        }
        // Ensure the normal collider is enabled and the ball collider is disabled at start.
        if (normalCollider != null) normalCollider.enabled = true;
        if (ballCollider != null) ballCollider.enabled = false;
    }

    public void TransformToBall()
    {
        // Immediately stop falling by resetting velocity and disabling gravity.
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.velocity = Vector2.zero;
        }
        StartCoroutine(TransformationSequence());
    }

    private IEnumerator TransformationSequence()
    {
        // Trigger the transformation animation and swap colliders immediately.
        animator.SetTrigger("Transform");
        if (normalCollider != null) normalCollider.enabled = false;
        if (ballCollider != null) ballCollider.enabled = true;

        // Set transformed state immediately so the player can move without delay.
        isTransformed = true;
        animator.SetBool("isTransformed", true);

        // Remain in flying (light) state for exactly flyingDuration seconds.
        yield return new WaitForSeconds(flyingDuration);

        // Trigger the revert animation and swap colliders back immediately.
        isTransformed = false;
        animator.SetBool("isTransformed", false);
        animator.SetTrigger("Revert");
       
        if (ballCollider != null) ballCollider.enabled = false;
        if (normalCollider != null) normalCollider.enabled = true;

        // Temporarily ignore collisions with the ground (layer "Ground") to avoid interference.
        int playerLayer = gameObject.layer;
        int groundLayer = LayerMask.NameToLayer("Ground");
        if (groundLayer >= 0)
        {
            Physics2D.IgnoreLayerCollision(playerLayer, groundLayer, true);
            yield return new WaitForSeconds(0.1f);
            Physics2D.IgnoreLayerCollision(playerLayer, groundLayer, false);
        }
        else
        {
            Debug.LogWarning("Ground layer not found. Please ensure a layer named 'Ground' exists and your ground objects are assigned to it.");
        }

        // Destroy the attached light since transformation is done.
        if (attachedLight != null)
        {
            Destroy(attachedLight);
        }

        // Restore the original gravity.
        if (rb != null)
        {
            rb.gravityScale = originalGravityScale;
        }

        // Wait 2 seconds after reverting before spawning the new light prefab.
        yield return new WaitForSeconds(0.8f);
        if (lightPrefab != null)
        {
            Instantiate(lightPrefab, originalLightPosition, Quaternion.identity);
        }
    }

    // Use FixedUpdate for physics-based movement so collisions are respected.
    private void FixedUpdate()
    {
        if (isTransformed)
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveY = Input.GetAxis("Vertical");
            Vector2 movement = new Vector2(moveX, moveY);
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }

    // Called by the LightTrigger to save the reference to the reparented light
    // and record its original world position.
    public void SetAttachedLight(GameObject lightObject)
    {
        attachedLight = lightObject;
        originalLightPosition = lightObject.transform.position;
    }
}



