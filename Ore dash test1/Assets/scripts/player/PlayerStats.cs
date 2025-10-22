using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int baseDamage = 10;
    public int bonusDamage = 0;

    public int TotalDamage => baseDamage + bonusDamage;

    public void IncreaseDamage(int amount)
    {
        bonusDamage += amount;
        Debug.Log($" Damage zwiêkszony! Nowy: {TotalDamage}");
    }
}


