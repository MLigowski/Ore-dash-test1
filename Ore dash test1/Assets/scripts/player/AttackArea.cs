using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(LineRenderer))]
public class AttackArea : MonoBehaviour
{
    [Header("Attack Settings")]
    public int damage = 3;
    public float range = 1.2f;
    public float attackDuration = 0.15f;
    public float flashDuration = 0.2f;

    private CircleCollider2D circleCollider;
    private LineRenderer lineRenderer;
    private bool canDamage = false;

    [Header("Position Offset")]
    public Vector2 forwardOffset = new Vector2(0.8f, 0f);

    void Start()
    {
        // Collider
        circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.isTrigger = true;
        circleCollider.enabled = false;
        circleCollider.offset = forwardOffset; // ustaw offset zamiast ruszać transform.position

        // LineRenderer — efekt czerwonego koła
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = true;
        lineRenderer.positionCount = 64;
        lineRenderer.startWidth = 0.04f;
        lineRenderer.endWidth = 0.04f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = new Color(1f, 0f, 0f, 0f);
        lineRenderer.endColor = new Color(1f, 0f, 0f, 0f);

        DrawCircle();
    }

    // Usuwamy Update() – nie ruszamy transform.position

    public void PerformAttack()
    {
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        canDamage = true;
        circleCollider.radius = range;
        circleCollider.enabled = true;
        FlashCircle(true);

        yield return new WaitForSeconds(attackDuration);

        canDamage = false;
        circleCollider.enabled = false;

        yield return new WaitForSeconds(flashDuration - attackDuration);

        FlashCircle(false);
    }
    [Header("References")]
    public PlayerStats playerStats; // referencja do PlayerStats

    private void OnTriggerEnter2D(Collider2D collider)
    {
        int currentDamage = (playerStats != null) ? playerStats.TotalDamage : damage;

        if (collider.TryGetComponent(out Health health))
            health.Damage(currentDamage);

        if (collider.TryGetComponent(out Slime slime))
            slime.TakeDamage(currentDamage);

        if (collider.TryGetComponent(out Zombie zombie))
            zombie.TakeDamage(currentDamage);

        if (collider.TryGetComponent(out Bringer_Of_Death enemy))
            enemy.TakeDamage(currentDamage);

    }

    private void DrawCircle()
    {
        float angleStep = 360f / lineRenderer.positionCount;
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            float angle = Mathf.Deg2Rad * i * angleStep;
            float x = Mathf.Cos(angle) * range;
            float y = Mathf.Sin(angle) * range;
            lineRenderer.SetPosition(i, new Vector3(x, y, 0));
        }
    }

    private void FlashCircle(bool show)
    {
        Color start = show ? new Color(1f, 0f, 0f, 0.3f) : new Color(1f, 0f, 0f, 0f);
        Color end = show ? new Color(1f, 0f, 0f, 0.3f) : new Color(1f, 0f, 0f, 0f);
        lineRenderer.startColor = start;
        lineRenderer.endColor = end;
    }

}
