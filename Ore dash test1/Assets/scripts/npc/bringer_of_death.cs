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

    [Header("Arena Walls")]
    public GameObject leftWall;
    public GameObject rightWall;

    [Header("UI Offset")]
    public Vector3 healthOffset = new Vector3(0, 1.5f, 0);

    private TextMeshPro healthTextInstance;
    private Rigidbody2D rb;
    private bool facingRight = true;
    private bool isDead = false;
    private bool fightStarted = false;

    private Health playerHealth;

    private Vector3 startPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        startPosition = transform.position;

        if (player != null)
            playerHealth = player.GetComponent<Health>();

        if (healthTextPrefab != null)
        {
            healthTextInstance = Instantiate(healthTextPrefab, transform.position + healthOffset, Quaternion.identity);
            healthTextInstance.text = $"HP:{currentHealth}/{maxHealth}";
            healthTextInstance.alignment = TextAlignmentOptions.Center;

            var rend = healthTextInstance.GetComponent<MeshRenderer>();
            rend.sortingLayerName = "UI";
            rend.sortingOrder = 200;
        }
    }

    void Update()
    {
        if (isDead || player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        // Jeśli gracz nie żyje, wylecz boss'a i przywróć pozycję
        if (playerHealth != null && playerHealth.currentHealth <= 0)
        {
            HealAfterPlayerDeath();
            return;
        }

        // Jeśli gracz jest nietykalny, zatrzymaj boss'a
        if (playerHealth != null && Health.IsInvincible)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (healthTextInstance != null)
            healthTextInstance.transform.position = transform.position + healthOffset;

        // Start walki, gdy gracz w zasięgu
        if (dist <= detectionRange)
        {
            StartFight();
        }

        // Obracanie boss'a w stronę gracza
        if (player.position.x > transform.position.x && facingRight)
            Flip();
        if (player.position.x < transform.position.x && !facingRight)
            Flip();

        // Jeśli gracz poza zasięgiem, boss stoi w miejscu
        if (dist > detectionRange)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // Atak w zasięgu
        if (dist <= attackRange)
        {
            TryAttack();
            return;
        }

        // Podchodzenie do gracza
        MoveTowardsPlayer();
    }

    void MoveTowardsPlayer()
    {
        animator.SetBool("Walk", true);

        float dir = player.position.x > transform.position.x ? 1 : -1;
        rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);
    }

    void TryAttack()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        animator.SetBool("Walk", false);

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            animator.SetBool("Attack", true);
            lastAttackTime = Time.time;
        }
        else
        {
            animator.SetBool("Attack", false);
        }
    }

    public void DealDamage()
    {
        if (Health.IsInvincible) return;
        if (playerHealth == null || playerHealth.currentHealth <= 0) return;

        if (Vector2.Distance(transform.position, player.position) <= attackRange + 0.1f)
            playerHealth.Damage(damage);
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;

        if (healthTextInstance != null)
            healthTextInstance.text = $"HP:{currentHealth}/{maxHealth}";

        if (currentHealth <= 0)
            Die();
    }

    void StartFight()
    {
        fightStarted = true;

        if (leftWall != null)
            leftWall.SetActive(true);
        if (rightWall != null)
            rightWall.SetActive(true);
    }

    void OpenArena()
    {
        if (leftWall != null)
            leftWall.SetActive(false);
        if (rightWall != null)
            rightWall.SetActive(false);
    }

    [Header("Teleport po śmierci")]
    public AreaTeleport linkedTeleport; // przeciągnij w inspectorze

    void Die()
    {
        isDead = true;
        CancelInvoke();

        OpenArena();

        animator.SetBool("isDead", true);
        animator.SetBool("Walk", false);
        animator.SetBool("Attack", false);

        rb.linearVelocity = Vector2.zero;

        if (healthTextInstance != null)
            Destroy(healthTextInstance.gameObject);

        // Odblokowanie teleportu
        if (linkedTeleport != null)
            linkedTeleport.UnlockTeleport();

        Destroy(gameObject, 1f);
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 s = transform.localScale;
        s.x *= -1;
        transform.localScale = s;
    }

    void HealAfterPlayerDeath()
    {
        // Przerwij walkę
        fightStarted = false;

        // Otwórz arenę
        OpenArena();

        // Przywróć zdrowie
        currentHealth = maxHealth;
        if (healthTextInstance != null)
            healthTextInstance.text = $"HP:{currentHealth}/{maxHealth}";

        // Przywróć startową pozycję
        transform.position = startPosition;
    }
}