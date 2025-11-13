using System.Collections;
using UnityEngine;
using TMPro;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Healing")]
    public int healAmount = 25;
    public float healCooldown = 10f;
    private float healTimer;

    [Header("Floating HP (3D Text Prefab)")]
    public GameObject healthTextPrefab;
    private GameObject healthTextObject;
    private TextMeshPro healthTextTMP;
    public Vector3 healthOffset = new Vector3(0f, 1.2f, 0f);

    [Header("UI Canvas")]
    public TextMeshProUGUI healCooldownText;
    public TextMeshProUGUI deathCounterText;
    private bool isDead = false;

    [Header("Respawn")]
    public float respawnTextDuration = 2f;
    private PlayerRespawn respawnScript;

    private SpriteRenderer spriteRenderer;
    private int deathCount = 0;

    // references for disabling input/attack
    private Rigidbody2D rb;
    private PlayerAttack playerAttack;
    private PlayerMovement playerMovement;

    public static bool PlayerIsDead = false;
    private Transform mainCam;  // kamera gracza
    private bool cameraFrozen = false;
    public static bool IsInvincible = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        playerAttack = GetComponent<PlayerAttack>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Start()
    {
        healTimer = healCooldown;
        respawnScript = FindObjectOfType<PlayerRespawn>();
        if (Camera.main != null)
            mainCam = Camera.main.transform;
        currentHealth = maxHealth;

        if (healthTextPrefab != null)
        {
            healthTextObject = Instantiate(healthTextPrefab, transform.position + healthOffset, Quaternion.identity);
            healthTextTMP = healthTextObject.GetComponent<TextMeshPro>();
        }

        UpdateHealthText();
        UpdateHealCooldownUI();
        UpdateDeathCounterUI();
    }

    void Update()
    {
        healTimer += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.H) && healTimer >= healCooldown)
        {
            Heal(healAmount);
            healTimer = 0f;
            UpdateHealCooldownUI();
        }

        if (healTimer < healCooldown)
            UpdateHealCooldownUI();
    }

    void LateUpdate()
    {
        if (healthTextObject != null)
        {
            healthTextObject.transform.position = transform.position + healthOffset;
            healthTextObject.transform.rotation = Quaternion.identity;
        }

        // --- DODAJ TO NA KOŃCU ---
        if (mainCam != null)
        {
            if (cameraFrozen)
            {
                // Kamera stoi w miejscu, nic nie robi
                return;
            }
            else
            {
                // Kamera śledzi gracza
                mainCam.position = new Vector3(
                    transform.position.x,
                    transform.position.y,
                    mainCam.position.z
                );
            }
        }
    }


    public void Damage(int amount)
    {
        if (respawnScript != null && respawnScript.IsInvincible())
            return;

        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);
        UpdateHealthText();

        if (currentHealth <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        UpdateHealthText();
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;
        PlayerIsDead = true; // ✅ Ustawienie flagi
        deathCount++;
        UpdateDeathCounterUI();

        if (healthTextTMP != null)
        {
            healthTextTMP.text = "YOU ARE DEAD";
            healthTextTMP.fontSize = 14f;
            healthTextTMP.color = Color.red;
        }

        // disable player controls + physics
        DisablePlayerControls();

        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        if (mainCam != null)
        {
            cameraFrozen = true;  // zapamiętaj że kamera stoi
        }

        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnTextDuration);

        if (respawnScript != null)
            respawnScript.RespawnPlayer();

        if (mainCam != null)
        {
            cameraFrozen = false;  // kamera znowu śledzi
        }

        currentHealth = maxHealth;
        UpdateHealthText();

        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        EnablePlayerControls();
        isDead = false;
        PlayerIsDead = false; // ✅ reset flagi po odrodzeniu
        UpdateHealCooldownUI();
    }

    private void DisablePlayerControls()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false; // stop physics
        }

        if (playerAttack != null)
            playerAttack.enabled = false;

        if (playerMovement != null)
            playerMovement.enabled = false;
    }

    private void EnablePlayerControls()
    {
        if (rb != null)
            rb.simulated = true;

        if (playerAttack != null)
            playerAttack.enabled = true;

        if (playerMovement != null)
            playerMovement.enabled = true;
    }

    private void UpdateHealthText()
    {
        if (healthTextTMP == null) return;

        if (currentHealth <= 0)
        {
            healthTextTMP.text = "YOU ARE DEAD";
            healthTextTMP.color = Color.red;
            healthTextTMP.fontSize = 14f;
            return;
        }

        healthTextTMP.fontSize = 3f;
        healthTextTMP.text = $"HP: {currentHealth}/{maxHealth}";

        float pct = (float)currentHealth / maxHealth;
        if (pct > 0.66f) healthTextTMP.color = Color.green;
        else if (pct > 0.33f) healthTextTMP.color = new Color(1f, 0.85f, 0f);
        else healthTextTMP.color = Color.red;
    }

    private void UpdateHealCooldownUI()
    {
        if (healCooldownText == null) return;

        float remaining = healCooldown - healTimer;

        // Jeśli cooldown się skończył, od razu pokazuj zielony napis
        if (remaining <= 0.05f) // mała tolerancja żeby nie pokazywało "0.0s"
        {
            healCooldownText.text = "<color=green>Heal Ready (H)</color>";
        }
        else
        {
            healCooldownText.text = $"<color=orange>Heal CD: {remaining:F1}s</color>";
        }
    }


    private void UpdateDeathCounterUI()
    {
        if (deathCounterText == null) return;
        deathCounterText.text = $"Deaths: {deathCount}";
    }

    private void OnDestroy()
    {
        if (healthTextObject != null)
            Destroy(healthTextObject);
    }
}