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
                attackArea.PerformAttack();

                // Spowolnienie gracza po ataku
                PlayerMovement playerMovement = GetComponent<PlayerMovement>();
                if (playerMovement != null)
                {
                    playerMovement._attackSlowTimer = playerMovement.attackSlowDuration;
                }
            }

            lastAttackTime = Time.time;
        }
    }
}