using UnityEngine;
using TMPro;
using System.Collections;

public class MineralUIManager : MonoBehaviour
{
    public static MineralUIManager Instance;

    [Header("UI Element")]
    public TextMeshProUGUI mineralCountText;

    [Header("Colors per gain amount")]
    public Color color1 = new Color(0.3f, 0.7f, 1f);   // Niebieski (dla +1)
    public Color color3 = new Color(0.7f, 0.3f, 1f);   // Fioletowy (dla +3)
    public Color color5 = new Color(0.3f, 1f, 0.5f);   // Zielony (dla +5)
    public Color normalColor = Color.white;            // Normalny kolor tekstu

    private Coroutine flashRoutine;

    void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Aktualizuje licznik minerałów i odpala efekt błysku w zależności od ilości zdobytych.
    /// </summary>
    public void UpdateMineralCount(int totalCount, int gainedAmount)
    {
        mineralCountText.text = $"Minerals: {totalCount}";

        Color flashColor = normalColor;
        switch (gainedAmount)
        {
            case 1: flashColor = color1; break;
            case 3: flashColor = color3; break;
            case 5: flashColor = color5; break;
        }

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashColor(flashColor));
    }

    IEnumerator FlashColor(Color targetColor)
    {
        float duration = 0.4f;
        float t = 0f;

        // rozbłysk koloru
        while (t < duration / 2f)
        {
            mineralCountText.color = Color.Lerp(normalColor, targetColor, t / (duration / 2f));
            t += Time.deltaTime;
            yield return null;
        }

        // powrót do normalnego koloru
        t = 0f;
        while (t < duration / 2f)
        {
            mineralCountText.color = Color.Lerp(targetColor, normalColor, t / (duration / 2f));
            t += Time.deltaTime;
            yield return null;
        }

        mineralCountText.color = normalColor;
    }
}
