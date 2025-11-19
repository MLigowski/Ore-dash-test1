using UnityEngine;

public class DamageUpgrade : MonoBehaviour
{
    public int upgradeCost = 3;         // ile mineralow za 1 damage
    public int damageIncrease = 1;

    public PlayerMining playerMining;    
    public PlayerStats playerStats;      

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
        
        if (GetMinerals() >= upgradeCost)
        {
            SpendMinerals(upgradeCost);
            playerStats.IncreaseDamage(damageIncrease);
            Debug.Log($"Ulepszono damage za {upgradeCost} mineralow!");

            
            upgradeCost += 0;
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


