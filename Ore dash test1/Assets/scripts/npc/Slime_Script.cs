using UnityEngine;
using TMPro;

public class Slime : MonoBehaviour
{
    [Header("Statystyki")]
    public int maxHealth = 5;
    private int currentHealth;

    public int damage = 10;
    public float jumpForce = 5f;
    public float jumpCooldown = 1.5f;
    public float moveSpeed = 2f;

    private Rigidbody2D rb;
    private float lastJumpTime = 0f;

    [Header("Śledzenie gracza")]
    public Transform player;
    public float chaseRange = 10f; // zasięg śledzenia gracza

    [Header("UI")]
    public TextMeshPro healthTextPrefab;
    private TextMeshPro healthTextInstance;
    public Vector3 healthOffset = new Vector3(0, 1f, 0);

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        // Tworzymy tekst nad Slime
        if (healthTextPrefab != null)
        {
            healthTextInstance = Instantiate(healthTextPrefab, transform.position + healthOffset, Quaternion.identity);
            healthTextInstance.text = $"{currentHealth}/{maxHealth}";
            healthTextInstance.alignment = TextAlignmentOptions.Center;

            var renderer = healthTextInstance.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.sortingLayerName = "Default";
                renderer.sortingOrder = 200;
            }
        }
    }

    void Update()
    {
        if (player == null) return;

        // Skacze co określony czas
        if (Time.time - lastJumpTime >= jumpCooldown)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= chaseRange)
                JumpTowardsPlayer(); // ściga gracza
            else
                RandomJump(); // losowy skok

            lastJumpTime = Time.time;
        }

        // Aktualizacja tekstu HP
        if (healthTextInstance != null)
            healthTextInstance.transform.position = transform.position + healthOffset;
    }

    void JumpTowardsPlayer()
    {
        if (player == null) return;

        float dirX = player.position.x - transform.position.x;
        float dir = Mathf.Sign(dirX); // -1 = lewo, 1 = prawo

        rb.linearVelocity = new Vector2(dir * moveSpeed, jumpForce);
    }

    void RandomJump()
    {
        // losowy kierunek: -1 (lewo), 0 (w miejscu), 1 (prawo)
        float randomDir = Random.Range(-1f, 1f);

        rb.linearVelocity = new Vector2(randomDir * moveSpeed, jumpForce * Random.Range(0.8f, 1.2f));
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
        UpdateHealthText();

        if (currentHealth <= 0)
            Die();
    }

    void UpdateHealthText()
    {
        if (healthTextInstance != null)
            healthTextInstance.text = $"{currentHealth}/{maxHealth}";
    }

    void Die()
    {
        if (healthTextInstance != null)
            Destroy(healthTextInstance.gameObject);
        Destroy(gameObject);
    }
}

