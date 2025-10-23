using UnityEngine;

public class Zombie : MonoBehaviour
{
    public int maxHealth = 15;
    private int currentHealth;

    public int damage = 20;
    public float speed = 2f;
    public float jumpForce = 5f;

    public Transform player;

    private Rigidbody2D rb;
    private bool facingRight = true;

    public LayerMask groundLayer;
    public float rayLength = 0.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (player == null) return;

        // Porusz si? w stron? gracza
        float dir = player.position.x > transform.position.x ? 1f : -1f;
        rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);
        facingRight = dir > 0;

        // Skok je?li przeszkoda lub dziura
        if (IsObstacleAhead() || IsGapAhead())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private bool IsObstacleAhead()
    {
        Vector2 origin = (Vector2)transform.position + Vector2.up * 0.5f;
        Vector2 direction = facingRight ? Vector2.right : Vector2.left;
        return Physics2D.Raycast(origin, direction, rayLength, groundLayer);
    }

    private bool IsGapAhead()
    {
        Vector2 origin = (Vector2)transform.position + (facingRight ? Vector2.right : Vector2.left) * 0.5f;
        return !Physics2D.Raycast(origin, Vector2.down, rayLength, groundLayer);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
                playerHealth.Damage(damage);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0) Destroy(gameObject);
    }
}
