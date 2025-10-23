using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class Damageable : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public Color hitColor = Color.red;
    public float flashDuration = 0.1f;
    public float knockbackForce = 5f;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Color originalColor;

    private bool isFlashing = false;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        originalColor = spriteRenderer.color;
    }

    public void TakeDamage(int amount, Vector2 attackerPosition)
    {
        if (isDead) return;

        currentHealth -= amount;
        StartCoroutine(FlashEffect());
        ApplyKnockback(attackerPosition);

        if (currentHealth <= 0)
            Die();
    }

    private IEnumerator FlashEffect()
    {
        if (isFlashing) yield break;
        isFlashing = true;

        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;

        isFlashing = false;
    }

    private void ApplyKnockback(Vector2 attackerPosition)
    {
        if (rb == null) return;

        Vector2 direction = (Vector2)(transform.position) - attackerPosition;
        direction.Normalize();
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
    }

    private void Die()
    {
        isDead = true;
        Destroy(gameObject);
    }
}


