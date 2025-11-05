using UnityEngine;
using TMPro;
using System.Collections;

public class MineralBlock : MonoBehaviour
{
    public int mineralValue = 1;
    public int hitsToBreak = 2;

    private int currentHits = 0;
    private SpriteRenderer sr;
    private Color originalColor;

    public TextMeshPro worldTextPrefab;
    private TextMeshPro worldTextInstance;
    public Vector3 textOffset = new Vector3(0, 0.6f, -0.01f);
    public float textScale = 0.5f;
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

    public void BreakBlock(int damage)
    {
        currentHits += damage;
        int remaining = hitsToBreak - currentHits;

        if (sr != null)
        {
            float t = Mathf.Clamp01((float)currentHits / hitsToBreak);
            sr.color = Color.Lerp(originalColor, Color.yellow, t * 0.5f);
        }

        ShowHitText(remaining);

        if (remaining <= 0)
        {
            CollectMinerals();
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
                renderer.sortingLayerName = "Mineral";
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

    private void CollectMinerals()
    {
        if (PlayerMining.Instance != null)
        {
            PlayerMining.Instance.AddMinerals(mineralValue);
            Debug.Log($"Added {mineralValue} minerals to player");
        }

        if (worldTextInstance != null)
            Destroy(worldTextInstance.gameObject);

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (worldTextInstance != null)
            Destroy(worldTextInstance.gameObject);
    }
}
