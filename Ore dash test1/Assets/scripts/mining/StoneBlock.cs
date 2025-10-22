using UnityEngine;

public class StoneBlock : MonoBehaviour
{
    [Tooltip("Czas zanim blok zniknie po wykopaniu (dla efektow).")]
    public float breakTime = 0.1f;

    public void BreakBlock()
    {
        // Tu m
        Debug.Log("Zniszczono kamienny blok!");

        // Usuwamy blok z gry
        Destroy(gameObject, breakTime);
    }
}

