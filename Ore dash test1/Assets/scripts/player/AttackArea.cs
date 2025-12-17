using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(LineRenderer))]
public class AttackArea : MonoBehaviour
{
    [Header("Attack Settings")]
    [Tooltip("Ile HP zabiera atak.")]
    public int damage = 3;

    [Tooltip("Promień ataku (zasięg).")]
    public float range = 1.2f;

    [Tooltip("Jak długo atak jest aktywny (sekundy).")]
    public float attackDuration = 0.15f;

    [Tooltip("Jak długo widoczne jest czerwone podświetlenie.")]
    public float flashDuration = 0.2f;

    private Transform player;
    private CircleCollider2D circleCollider;
    private LineRenderer lineRenderer;

    private bool canDamage = false;

    void Start()
    {
        // Szukamy gracza
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("❌ AttackArea: Nie znaleziono obiektu z tagiem 'Player'!");
            return;
        }

        // Collider
        circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.isTrigger = true;
        circleCollider.enabled = false; // tylko podczas ataku

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

    void Update()
    {
        if (player == null) return;
        transform.position = player.position; // AttackArea podąża za graczem
    }

    public void PerformAttack(int currentDamage)
    {
        damage = currentDamage; // pobiera damage z PlayerStats
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

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!canDamage) return;

        if (collider.TryGetComponent(out Health health))
            health.Damage(damage);

        if (collider.TryGetComponent(out Slime slime))
            slime.TakeDamage(damage);

        if (collider.TryGetComponent(out Zombie zombie))
            zombie.TakeDamage(damage);

        if (collider.TryGetComponent(out Bringer_Of_Death bod))
            bod.TakeDamage(damage);
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