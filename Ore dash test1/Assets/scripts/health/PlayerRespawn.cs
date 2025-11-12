using UnityEngine;
using System.Collections;

public class PlayerRespawn : MonoBehaviour
{
    [Header("Ustawienia Respawnu")]
    [Tooltip("Miejsce, w którym gracz siê odradza po œmierci.")]
    public Transform spawnPoint; // ?? Teraz publiczne — widoczne w Inspectorze!

    [Tooltip("Jak d³ugo po respawnie gracz jest nieœmiertelny (w sekundach).")]
    public float invincibilityTime = 5f; // ?? Edytowalne w Inspectorze

    private bool invincible = false;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        // Jeœli spawnPoint
        if (spawnPoint == null)
        {
            GameObject found = GameObject.Find("SpawnPoint");
            if (found != null)
                spawnPoint = found.transform;
        }
    }

    public void RespawnPlayer()
    {
        if (spawnPoint == null)
        {
            Debug.LogWarning("?? SpawnPoint nie zosta³ przypiêty do PlayerRespawn!");
            return;
        }

        // ?? Przenieœ gracza na spawn
        transform.position = spawnPoint.position;

        // ?? Uruchom okres nimiertelnoœci
        StartCoroutine(InvincibilityPeriod());
    }

    private IEnumerator InvincibilityPeriod()
    {
        invincible = true;

        float timer = 0f;
        while (timer < invincibilityTime)
        {
            if (sr != null)
                sr.enabled = !sr.enabled; // mruganie
            yield return new WaitForSeconds(0.2f);
            timer += 0.2f;
        }

        if (sr != null)
            sr.enabled = true;

        invincible = false;
    }

    public bool IsInvincible()
    {
        return invincible;
    }
}
