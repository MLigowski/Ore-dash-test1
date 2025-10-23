using UnityEngine;
using TMPro;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int health = 100;
    private int MAX_HEALTH = 100;

    [Header("Damage & Heal Settings")]
    [Tooltip("How much damage you take when pressing Right Ctrl")]
    public int selfDamageAmount = 10;

    [Tooltip("How much HP you heal when pressing H")]
    public int healAmount = 25;

    [Tooltip("Cooldown time for healing in seconds")]
    public float healCooldown = 15f;

    private float lastHealTime = -999f;

    [Header("UI (World Text)")]
    public TextMeshPro worldTextPrefab; // Prefab TMP 3D
    public Vector3 textOffset = new Vector3(0, 1.2f, 0);
    private TextMeshPro worldTextInstance;

    [Header("UI (Heal Cooldown Display)")]
    public TextMeshProUGUI healCooldownTextPrefab; // Prefab TMP UI
    private TextMeshProUGUI healCooldownTextInstance;

    void Start()
    {
        // Create health text above character
        if (worldTextPrefab != null)
        {
            worldTextInstance = Instantiate(worldTextPrefab, transform.position + textOffset, Quaternion.identity);
            worldTextInstance.text = $"HP: {health}/{MAX_HEALTH}";
            worldTextInstance.alignment = TextAlignmentOptions.Center;
            worldTextInstance.fontSize = 3f;

            var renderer = worldTextInstance.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.sortingLayerName = "Default";
                renderer.sortingOrder = 200;
            }

            UpdateHealthColor();
        }
        else
        {
            Debug.LogWarning("No TMP world text prefab assigned in Health.cs");
        }

        // Create heal cooldown text in corner
        if (healCooldownTextPrefab != null)
        {
            // ? Nowa wersja metody
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                healCooldownTextInstance = Instantiate(healCooldownTextPrefab, canvas.transform);
                healCooldownTextInstance.rectTransform.anchorMin = new Vector2(0, 1);
                healCooldownTextInstance.rectTransform.anchorMax = new Vector2(0, 1);
                healCooldownTextInstance.rectTransform.pivot = new Vector2(0, 1);
                healCooldownTextInstance.rectTransform.anchoredPosition = new Vector2(20, -20);
                healCooldownTextInstance.fontSize = 20;
                healCooldownTextInstance.color = Color.black;
                healCooldownTextInstance.text = "";
            }
            else
            {
                Debug.LogWarning("No Canvas found in the scene for healCooldownTextPrefab!");
            }
        }
    }

    void Update()
    {
        // Keep text above the character
        if (worldTextInstance != null)
        {
            Vector3 pos = transform.position + textOffset;
            pos.z = -0.01f;
            worldTextInstance.transform.position = pos;
        }

        // Heal key (H)
        if (Input.GetKeyDown(KeyCode.H))
        {
            TryHeal();
        }

        // Self-damage key (Right Ctrl)
        if (Input.GetKeyDown(KeyCode.RightControl))
        {
            Damage(selfDamageAmount);
        }

        // Update heal cooldown text
        UpdateHealCooldownDisplay();
    }

    private void TryHeal()
    {
        if (Time.time - lastHealTime < healCooldown)
        {
            float remaining = healCooldown - (Time.time - lastHealTime);
            Debug.Log($"?? Heal on cooldown ({remaining:F1}s remaining)");
            return;
        }

        Heal(healAmount);
        lastHealTime = Time.time;
        Debug.Log($"?? You healed for {healAmount} HP!");
    }

    private void UpdateHealCooldownDisplay()
    {
        if (healCooldownTextInstance == null)
            return;

        float elapsed = Time.time - lastHealTime;
        float remaining = healCooldown - elapsed;

        if (remaining > 0)
        {
            float percent = Mathf.Clamp01(remaining / healCooldown);

            // Color transitions: red ? yellow ? green
            Color color;
            if (percent > 0.7f)
                color = Color.red;
            else if (percent > 0.4f)
                color = Color.Lerp(Color.yellow, Color.red, (percent - 0.4f) / 0.3f);
            else
                color = Color.green;

            string colorHex = ColorUtility.ToHtmlStringRGB(color);

            healCooldownTextInstance.text =
                $"<color=#000000>Heal ready in:</color> <color=#{colorHex}>{remaining:F1}s</color>";
        }
        else
        {
            string readyColorHex = ColorUtility.ToHtmlStringRGB(new Color(0.37f, 1f, 0.37f)); // #5EFF5E approx
            healCooldownTextInstance.text = $"<color=#{readyColorHex}>Heal ready</color>";
        }
    }

    public void Damage(int amount)
    {
        if (amount < 0)
            throw new System.ArgumentOutOfRangeException("Cannot have negative Damage");

        health -= amount;
        health = Mathf.Max(health, 0);

        UpdateHealthText();

        if (health <= 0)
            Die();
        else
            Debug.Log($"?? Took {amount} damage! Current HP: {health}/{MAX_HEALTH}");
    }

    public void Heal(int amount)
    {
        if (amount < 0)
            throw new System.ArgumentOutOfRangeException("Cannot have negative healing");

        health = Mathf.Min(health + amount, MAX_HEALTH);
        UpdateHealthText();
    }

    private void UpdateHealthText()
    {
        if (worldTextInstance == null) return;

        worldTextInstance.text = $"HP: {health}/{MAX_HEALTH}";
        UpdateHealthColor();
    }

    private void UpdateHealthColor()
    {
        if (worldTextInstance == null) return;

        float healthPercent = (float)health / MAX_HEALTH;

        if (healthPercent > 0.7f)
        {
            worldTextInstance.color = Color.green;
        }
        else if (healthPercent > 0.4f)
        {
            worldTextInstance.color = Color.Lerp(
                new Color(1f, 0.9f, 0f),
                new Color(1f, 0.5f, 0f),
                (0.7f - healthPercent) / 0.3f
            );
        }
        else
        {
            worldTextInstance.color = Color.red;
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died!");
        if (worldTextInstance != null)
            Destroy(worldTextInstance.gameObject);
        if (healCooldownTextInstance != null)
            Destroy(healCooldownTextInstance.gameObject);

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (worldTextInstance != null)
            Destroy(worldTextInstance.gameObject);
        if (healCooldownTextInstance != null)
            Destroy(healCooldownTextInstance.gameObject);
    }
}
