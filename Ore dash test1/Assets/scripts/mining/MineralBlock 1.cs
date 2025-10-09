using UnityEngine;

public class MineralBlock : MonoBehaviour
{
    public int mineralValue = 1; // ile minera³ów dostajesz za zniszczenie tego bloku

    public void BreakBlock()
    {
        // Mo¿esz dodaæ tu animacje, efekty dŸwiêkowe itp.

        // Powiadamiamy gracza o zdobyciu minera³ów
        PlayerMining.Instance.AddMinerals(mineralValue);

        // Usuwamy blok z gry
        Destroy(gameObject);
    }
}
