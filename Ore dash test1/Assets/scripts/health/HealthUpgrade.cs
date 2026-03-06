using UnityEngine;

public class HealthUpgrade : MonoBehaviour
{
    public Health playerHealth;

    public int healthIncrease = 20;
    public int mineralCost = 5;

    public void UpgradeHealth()
    {
        if (playerHealth == null) return;
        if (PlayerMining.Instance == null) return;

        int minerals = PlayerMining.Instance.GetMineralCount();

        if (minerals >= mineralCost)
        {
            PlayerMining.Instance.SpendMinerals(mineralCost);

            playerHealth.maxHealth += healthIncrease;
            playerHealth.currentHealth += healthIncrease;

            // 🔹 Odświeżamy UI
            playerHealth.UpdateHealthText();
            playerHealth.UpdateHealCooldownUI();

            Debug.Log("HP upgraded! New Max HP: " + playerHealth.maxHealth);
        }
        else
        {
            Debug.Log("Not enough minerals!");
        }
    }
}