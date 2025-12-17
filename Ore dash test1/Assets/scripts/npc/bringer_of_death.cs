using UnityEngine;
using TMPro;

public class Bringer_Of_Death : MonoBehaviour
{
    [Header("Statystyki")]
    public int maxHealth = 30;
    private int currentHealth;

    [Header("Ruch")]
    public float moveSpeed = 2f;
    public float detectionRange = 6f;
    public float attackRange = 1.2f;

    [Header("Atak")]
    public int damage = 10;
    public float attackCooldown = 1.5f;
    private float lastAttackTime;

    [Header("Referencje")]
    public Transform player;
    public Animator animator;
    public TextMeshPro healthTextPrefab;

    [Header("UI Offset")]
    public Vector3 healthOffset = new Vector3(0, 1.5f, 0);

    private TextMeshPro healthTextInstance;
    private Rigidbody2D rb;
    private bool facingRight = true;
    private bool isDead = false;
    private Health playerHealth;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        playerHealth = player.GetComponent<Health>();

        // Tworzymy HP tekst i dodajemy SLOT w inspectorze
        if (healthTextPrefab != null)
        {
            healthTextInstance = Instantiate(healthTextPrefab, transform.position + healthOffset, Quaternion.identity);
            healthTextInstance.text = $"HP:{currentHealth}/{maxHealth}";
            healthTextInstance.ForceMeshUpdate();
            healthTextInstance.alignment = TextAlignmentOptions.Center;

            var rend = healthTextInstance.GetComponent<MeshRenderer>();
            rend.sortingLayerName = "UI";
            rend.sortingOrder = 200;
        }
    }

    void Update()
    {
        if (isDead || player == null) return;

        if (playerHealth != null && (playerHealth.currentHealth <= 0 || Health.IsInvincible))
        {
            animator.SetBool("Attack", false);
            animator.SetBool("Walk", false);
            animator.SetBool("Idle", true);
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Health ph = player.GetComponent<Health>();
        if (ph != null && ph.currentHealth <= 0) return;

        // aktualizacja pozycji HP
        if (healthTextInstance != null)
            healthTextInstance.transform.position = transform.position + healthOffset;

        float dist = Vector2.Distance(transform.position, player.position);

        // CELOWO ODWROTNIE — patrzy nie tam gdzie gracz
        if (player.position.x > transform.position.x && facingRight)
            Flip();
        if (player.position.x < transform.position.x && !facingRight)
            Flip();


        // poza zasięgiem → idle
        if (dist > detectionRange)
        {
            animator.SetBool("Idle", true);
            animator.SetBool("Walk", false);
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // blisko → atak
        if (dist <= attackRange)
        {
            TryAttack();
            return;
        }

        // w gywno → chodzimy
        MoveTowardsPlayer();
    }


    void MoveTowardsPlayer()
    {
        animator.SetBool("Idle", false);
        animator.SetBool("Walk", true);

        float dir = player.position.x > transform.position.x ? 1 : -1;
        rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);
    }


    void TryAttack()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        animator.SetBool("Walk", false);
        animator.SetBool("Idle", false);

        if (Time.time >= lastAttackTime + 0.95f)

        {
            animator.SetBool("Attack", true);
            lastAttackTime = Time.time;
            Invoke(nameof(ApplyDamageToPlayer), 0.1f);

        }
        else
        {
            animator.SetBool("Attack", false);
        }
    }

    void ApplyDamageToPlayer()
    {
        if (player == null) return;
        if (Health.IsInvincible) return; // ⛔ NIE BIJE PODCZAS DASH

        Health ph = player.GetComponent<Health>();
        if (ph == null || ph.currentHealth <= 0) return;

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= attackRange + 0.2f)
            ph.Damage(damage);
    }



    public void DealDamage()
    {
        if (Health.IsInvincible) return; // ⛔ blokada dmg przy dash
        if (playerHealth == null || playerHealth.currentHealth <= 0) return;

        if (Vector2.Distance(transform.position, player.position) <= attackRange + 0.1f)
            playerHealth.Damage(damage);
    }



    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;

        if (healthTextInstance != null)
        {
            healthTextInstance.text = $"HP:{currentHealth}/{maxHealth}";
            healthTextInstance.ForceMeshUpdate(); // ← to wymusza odświeżenie TMP
        }

        if (currentHealth <= 0)
            Die();
    }



    void Die()
    {
        isDead = true;
        animator.SetBool("isDead", true); // <- bool zamiast triggera
        animator.SetBool("Walk", false);
        animator.SetBool("Idle", false);
        animator.SetBool("Attack", false);
        rb.linearVelocity = Vector2.zero;

        if (healthTextInstance != null)
            Destroy(healthTextInstance.gameObject);

        Destroy(gameObject, 1f);
    }



    void Flip()
    {
        facingRight = !facingRight;
        Vector3 s = transform.localScale;
        s.x *= -1;
        transform.localScale = s;
    }
}