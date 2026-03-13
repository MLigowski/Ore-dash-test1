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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

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

        // jeśli gracz umrze → otwórz ściany
        if (playerHealth != null && playerHealth.currentHealth <= 0)
        {
            OpenArena();
            return;
        }

        if (playerHealth != null && (playerHealth.currentHealth <= 0 || Health.IsInvincible))
        {
            animator.SetBool("Attack", false);
            animator.SetBool("Walk", false);
            animator.SetBool("Idle", true);
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (healthTextInstance != null)
            healthTextInstance.transform.position = transform.position + healthOffset;

        float dist = Vector2.Distance(transform.position, player.position);

        // start walki gdy boss wykryje gracza
        if (!fightStarted && dist <= detectionRange)
        {
            StartFight();
        }

        if (player.position.x > transform.position.x && facingRight)
            Flip();
        if (player.position.x < transform.position.x && !facingRight)
            Flip();

        if (dist > detectionRange)
        {
            animator.SetBool("Idle", true);
            animator.SetBool("Walk", false);
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        if (dist <= attackRange)
        {
            TryAttack();
            return;
        }

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

    // ANIMATION EVENT
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
        {
            healthTextInstance.text = $"HP:{currentHealth}/{maxHealth}";
        }

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

    void Die()
    {
        isDead = true;
        CancelInvoke();

        OpenArena();

        animator.SetBool("isDead", true);
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