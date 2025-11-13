using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Atak")]
    public AttackArea attackArea;
    public float attackCooldown = 0.5f;

    private float lastAttackTime;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time - lastAttackTime >= attackCooldown)
        {
            if (attackArea != null)
            {
                var stats = GetComponent<PlayerStats>();
                if (stats != null)
                    attackArea.PerformAttack(stats.TotalDamage);
            }


            lastAttackTime = Time.time;
        }
    }
}