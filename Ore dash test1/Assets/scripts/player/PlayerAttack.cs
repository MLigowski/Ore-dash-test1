using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackDuration = 0.25f;   // czas dzia³ania hitboxa
    public int damage = 5;                  // bazowe obra¿enia
    public float range = 1f;                // zasiêg ataku
    public float attackCooldown = 0.5f;     // cooldown w sekundach

    private float attackTimer = 0f;
    private bool attacking = false;
    private float lastAttackTime = -999f;

    private GameObject attackArea;

    private PlayerStats playerStats; // ?? Dodane

    void Start()
    {
        // ?? ZnajdŸ komponent statystyk gracza
        playerStats = GetComponent<PlayerStats>();
        if (playerStats == null)
            Debug.LogWarning("Brak komponentu PlayerStats na obiekcie gracza!");

        // Tworzymy dynamicznie AttackArea
        attackArea = new GameObject("AttackArea");
        attackArea.transform.parent = transform;
        attackArea.transform.localPosition = Vector3.zero;

        CircleCollider2D col = attackArea.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = range;

        AttackArea attackScript = attackArea.AddComponent<AttackArea>();
        attackScript.damage = damage;
        attackScript.range = range;

        attackArea.SetActive(false);
    }

    void Update()
    {
        // Atak lewym przyciskiem myszy z cooldownem
        if (Input.GetMouseButtonDown(0) && Time.time - lastAttackTime >= attackCooldown)
        {
            StartAttack();
        }

        // Odliczanie czasu ataku
        if (attacking)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackDuration)
            {
                attackTimer = 0f;
                attacking = false;
                attackArea.SetActive(false);
            }
        }
    }

    private void StartAttack()
    {
        attacking = true;
        lastAttackTime = Time.time;

        // Aktualizacja parametrów attackArea
        CircleCollider2D col = attackArea.GetComponent<CircleCollider2D>();
        col.radius = range;

        AttackArea attackScript = attackArea.GetComponent<AttackArea>();

        // ?? U¿ywamy prawdziwych obra¿eñ z PlayerStats
        attackScript.damage = playerStats != null ? playerStats.TotalDamage : damage;
        attackScript.range = range;

        attackArea.SetActive(true);
    }
}
