using UnityEngine;

public class StoneBlock : MonoBehaviour
{
    [Tooltip("Czas zanim blok zniknie po wykopaniu (dla efektów).")]
    public float breakTime = 0.1f;

    public void BreakBlock()
    {
        // Tu mo¿esz dodaæ efekty cz¹steczek, dŸwiêk, animacjê itp.
        Debug.Log("Zniszczono kamienny blok!");

        // Usuwamy blok z gry
        Destroy(gameObject, breakTime);
    }
}
