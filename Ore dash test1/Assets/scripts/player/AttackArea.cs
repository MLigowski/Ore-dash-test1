using UnityEngine;
using System.Collections;

public class AttackArea : MonoBehaviour
{
    [Header("Attack Settings")]
    public int damage = 3;            // ile HP zabiera atak
    public float range = 1f;          // zasi?g ataku od ?rodka gracza
    public float flashDuration = 0.1f; // jak d?ugo ?wieci na czerwono

    [Header("Efekt pod?wietlenia")]
    [Tooltip("Renderer lub sprite u?ywany jako efekt ataku (mo?e by? np. czerwona pó?przezroczysta kula)")]
    public SpriteRenderer flashRenderer;

    private void Start()
    {
        if (flashRenderer != null)
        {
            flashRenderer.enabled = false; // domy?lnie wy??czony
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Health health = collider.GetComponent<Health>();
        if (health != null)
        {
            health.Damage(damage);
        }

        var slime = collider.GetComponent<Slime>();
        if (slime != null)
            slime.TakeDamage(damage);

        var zombie = collider.GetComponent<Zombie>();
        if (zombie != null)
            zombie.TakeDamage(damage);

        // Efekt pod?wietlenia
        if (flashRenderer != null)
            StartCoroutine(FlashEffect());
    }

    private IEnumerator FlashEffect()
    {
        flashRenderer.enabled = true;
        yield return new WaitForSeconds(flashDuration);
        flashRenderer.enabled = false;
    }
}