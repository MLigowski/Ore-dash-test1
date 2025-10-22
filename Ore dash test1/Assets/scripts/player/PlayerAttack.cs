using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject attackArea;
    public float timeToAttack = 0.25f;
    public float attackCooldown = 0.5f;

    private bool attacking = false;
    private float attackTimer = 0f;
    private float cooldownTimer = 0f;

    void Start()
    {
        attackArea.SetActive(false);
    }

    void Update()
    {
        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space) && !attacking && cooldownTimer <= 0f)
        {
            Attack();
        }

        if (attacking)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= timeToAttack)
            {
                attackTimer = 0f;
                attacking = false;
                attackArea.SetActive(false);
                cooldownTimer = attackCooldown;
            }
        }
    }

    private void Attack()
    {
        attacking = true;
        attackArea.SetActive(true);
        // Tu mo¿esz dodaæ animacjê lub dŸwiêk
    }
}

