using UnityEngine;

public class DamageUpgrade : MonoBehaviour
{
    public int upgradeCost = 5;         // ile mineralow za 1 damage
    public int damageIncrease = 1;

    public PlayerMining playerMining;    // referencja do PlayerMining (zawiera mineraly)
    public PlayerStats playerStats;      // referencja do PlayerStats (damage)

    public KeyCode upgradeKey = KeyCode.U; // klawisz do ulepszania

    void Update()
    {
        if (Input.GetKeyDown(upgradeKey))
        {
            UpgradeDamage();
        }
    }

    public void UpgradeDamage()
    {
        // Sprawdzamy, czy gracz ma wystarczajaco mineralow
        if (GetMinerals() >= upgradeCost)
        {
            SpendMinerals(upgradeCost);
            playerStats.IncreaseDamage(damageIncrease);
            Debug.Log($"Ulepszono damage za {upgradeCost} mineralow!");

            // Zwiekszamy koszt ulepszenia o 5
            upgradeCost += 5;
        }
        else
        {
            Debug.Log("Za malo mineralow!");
        }
    }

    private int GetMinerals()
    {
        return playerMining.GetMineralCount();
    }

    private void SpendMinerals(int amount)
    {
        playerMining.SpendMinerals(amount);
    }
}


