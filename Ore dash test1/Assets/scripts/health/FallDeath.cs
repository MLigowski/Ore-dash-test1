using UnityEngine;

public class FallDeath : MonoBehaviour
{
    public float deathY = -15f;

    private Health health;

    void Start()
    {
        health = GetComponent<Health>();
    }

    void Update()
    {
        if (transform.position.y < deathY && health != null && !Health.PlayerIsDead)
        {
            health.Damage(9999); // zabija gracza przez system Health
        }
    }
}
