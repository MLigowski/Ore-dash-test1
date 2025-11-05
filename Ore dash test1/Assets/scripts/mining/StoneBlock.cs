using UnityEngine;
using TMPro;
using System.Collections;

public class StoneBlock : MonoBehaviour
{
    [Header("W³aœciwoœci kamienia")]
    [Tooltip("Ile minera³ów lub punktów gracz otrzymuje po zniszczeniu tego bloku.")]
    public int stoneValue = 0;

    [Header("Wytrzyma³oœæ bloku")]
    [Tooltip("Ile uderzeñ potrzeba, ¿eby zniszczyæ kamieñ.")]
    public int hitsToBreak = 4;

    private int currentHits = 0;
    private SpriteRenderer sr;
    private Color originalColor;

    [Header("UI (opcjonalne)")]
    [Tooltip("Prefab z TextMeshPro 3D, który pokazuje ile uderzeñ pozosta³o.")]
    public TextMeshPro worldTextPrefab;

    private TextMeshPro worldTextInstance;

    [Header("Opcje wyœwietlania tekstu")]
    [Tooltip("Offset tekstu nad blokiem.")]
    public Vector3 textOffset = new Vector3(0, 0.6f, -0.01f);
    [Tooltip("Skala prefab TextMeshPro.")]
    public float textScale = 0.5f;
    [Tooltip("Ile sekund tekst ma siê pokazaæ jeœli nie uderzono bloku ponownie.")]
    public float textDisplayTime = 1.1f;

    private Coroutine hideTextCoroutine;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            originalColor = sr.color;
    }

    void Update()
    {
        if (worldTextInstance != null && worldTextInstance.gameObject.activeSelf)
        {
            Vector3 pos = transform.position + textOffset;
            pos.z = -0.01f;
            worldTextInstance.transform.position = pos;
        }
    }

    // ?? Teraz przyjmuje argument "damage"
    public void BreakBlock(int damage)
    {
        currentHits += damage;
        int remaining = hitsToBreak - currentHits;

        // Efekt wizualny – kamieñ jaœnieje przy uderzeniu
        if (sr != null)
        {
            float t = Mathf.Clamp01((float)currentHits / hitsToBreak);
            sr.color = Color.Lerp(originalColor, Color.gray, t * 0.5f);
        }

        ShowHitText(remaining);

        // Zniszczenie po osi¹gniêciu limitu uderzeñ
        if (remaining <= 0)
        {
            CollectStone();
        }
    }

    private void ShowHitText(int remaining)
    {
        if (worldTextInstance == null && worldTextPrefab != null)
        {
            worldTextInstance = Instantiate(worldTextPrefab, transform.position + textOffset, Quaternion.identity);
            worldTextInstance.transform.localScale = Vector3.one * textScale;

            var renderer = worldTextInstance.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.sortingLayerName = "Mineral"; // lub "Ground" – zale¿nie od projektu
                renderer.sortingOrder = 100;
            }
        }

        if (worldTextInstance != null)
        {
            worldTextInstance.text = remaining > 0 ? remaining.ToString() : "";
            worldTextInstance.gameObject.SetActive(true);

            if (hideTextCoroutine != null)
                StopCoroutine(hideTextCoroutine);

            hideTextCoroutine = StartCoroutine(HideTextAfterDelay());
        }
    }

    private IEnumerator HideTextAfterDelay()
    {
        yield return new WaitForSeconds(textDisplayTime);

        if (worldTextInstance != null)
            worldTextInstance.gameObject.SetActive(false);

        hideTextCoroutine = null;
    }

    private void CollectStone()
    {
        // Mo¿na tu dodaæ np. dodawanie punktów do gracza
        if (stoneValue > 0 && PlayerMining.Instance != null)
        {
            PlayerMining.Instance.AddMinerals(stoneValue);
        }

        if (worldTextInstance != null)
        {
            Destroy(worldTextInstance.gameObject);
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (worldTextInstance != null)
        {
            Destroy(worldTextInstance.gameObject);
        }
    }
}

