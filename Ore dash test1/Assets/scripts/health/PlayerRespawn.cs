using UnityEngine;
using System.Collections;

public class PlayerRespawn : MonoBehaviour
{
    [Header("Ustawienia Respawnu")]
    [Tooltip("Miejsce, w kt�rym gracz si� odradza po �mierci.")]
    public Transform spawnPoint; // ?? Teraz publiczne � widoczne w Inspectorze!

    [Tooltip("Jak d�ugo po respawnie gracz jest nie�miertelny (w sekundach).")]
    public float invincibilityTime = 5f; // ?? Edytowalne w Inspectorze

    private bool invincible = false;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        // Je�li spawnPoint nie jest przypi�ty, spr�buj znale�� pusty obiekt o nazwie "SpawnPoint"
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
            Debug.LogWarning("?? SpawnPoint nie zosta� przypi�ty do PlayerRespawn!");
            return;
        }

        // ?? Przenie� gracza na spawn
        transform.position = spawnPoint.position;

        // ?? Uruchom okres nie�miertelno�ci
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