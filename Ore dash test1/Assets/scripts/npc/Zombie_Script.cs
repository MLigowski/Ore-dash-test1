using UnityEngine;

public class Zombie : MonoBehaviour
{
    public Transform player;
    public float speed = 2f;
    public float detectionRange = 8f;
    public int damage = 10;
    public float attackCooldown = 1f;
    private float attackTimer;

    private Rigidbody2D rb;
    private bool facingRight = true;

    [Header("Health")]
    public int maxHealth = 15;
    private int currentHealth;

    private SpriteRenderer spriteRenderer;
    private bool isDead = false;

    [Header("Floating HP (3D Text Prefab)")]
    public GameObject healthTextPrefab;
    private GameObject healthTextObject;
    private TMPro.TextMeshPro healthTextTMP;
    public Vector3 healthOffset = new Vector3(0f, 1.2f, 0f);


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        if (healthTextPrefab != null)
        {
            healthTextObject = Instantiate(healthTextPrefab, transform.position + healthOffset, Quaternion.identity);
            healthTextTMP = healthTextObject.GetComponent<TMPro.TextMeshPro>();
            UpdateHealthText();
        }

    }

    void Update()
    {
        if (isDead) return;
        if (player == null) return;

        // ? NIE goni, jeœli gracz martwy
        if (Health.PlayerIsDead)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance > detectionRange)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        float direction = player.position.x > transform.position.x ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);

        facingRight = direction > 0;
        Vector3 scale = transform.localScale;
        scale.x = facingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;

        attackTimer -= Time.deltaTime;

        if (healthTextObject != null)
        {
            healthTextObject.transform.position = transform.position + healthOffset;
            healthTextObject.transform.rotation = Quaternion.identity;
        }

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isDead || Health.PlayerIsDead) return;

        if (collision.gameObject.CompareTag("Player") && attackTimer <= 0)
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
                playerHealth.Damage(damage);

            attackTimer = attackCooldown;
        }
    }

    // ? PRZYWRÓCONA METODA
    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        UpdateHealthText();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthText()
    {
        if (healthTextTMP == null) return;

        if (isDead)
        {
            healthTextTMP.text = "";
            return;
        }

        healthTextTMP.text = $"HP: {currentHealth}/{maxHealth}";
        healthTextTMP.fontSize = 2.5f;
        healthTextTMP.color = Color.green;
    }


    private void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        spriteRenderer.enabled = false;
        if (healthTextObject != null)
            Destroy(healthTextObject);
        Destroy(gameObject, 1.5f);
    }
}