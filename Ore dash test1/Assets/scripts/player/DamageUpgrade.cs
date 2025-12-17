using UnityEngine;

public class DamageUpgrade : MonoBehaviour
{
    [Header("Upgrade settings")]
    public int upgradeCost = 3;        // koszt w minera³ach
    public int damageIncrease = 1;     // ile dmg dodaje upgrade

    [Header("References")]
    public PlayerStats playerStats;    // PlayerStats gracza

    [Header("Input (opcjonalne)")]
    public KeyCode upgradeKey = KeyCode.U;

    void Start()
    {
        // Auto-przypisanie PlayerStats jeœli nie ustawione w Inspectorze
        if (playerStats == null)
        {
            playerStats = FindObjectOfType<PlayerStats>();
        }

        if (playerStats == null)
        {
            Debug.LogError("DamageUpgrade: NIE znaleziono PlayerStats!");
        }
    }

    void Update()
    {
        // Opcjonalne ulepszanie z klawiatury (debug)
        if (Input.GetKeyDown(upgradeKey))
        {
            UpgradeDamage();
        }
    }

    public void UpgradeDamage()
    {
        // Zabezpieczenia
        if (PlayerMining.Instance == null)
        {
            Debug.LogError("DamageUpgrade: PlayerMining.Instance == null");
            return;
        }

        if (playerStats == null)
        {
            Debug.LogError("DamageUpgrade: PlayerStats == null");
            return;
        }

        // Sprawdzenie kosztu
        if (PlayerMining.Instance.GetMineralCount() < upgradeCost)
        {
            Debug.Log("Za ma³o minera³ów!");
            return;
        }

        // P³atnoœæ + upgrade
        PlayerMining.Instance.SpendMinerals(upgradeCost);
        playerStats.IncreaseDamage(damageIncrease);

        Debug.Log($"Ulepszono DMG o +{damageIncrease}. Aktualny DMG: {playerStats.TotalDamage}");
    }
}
