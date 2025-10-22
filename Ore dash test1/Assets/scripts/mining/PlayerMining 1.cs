using UnityEngine;

public class PlayerMining : MonoBehaviour
{
    public static PlayerMining Instance;

    [Header("Ustawienia kopania")]
    [Tooltip("Jak daleko gracz moze siegnac, zeby kopac.")]
    public float miningRange = 1.5f;

    [Tooltip("Klucz aktywujacy kopanie.")]
    public KeyCode mineKey = KeyCode.Tab;

    [Tooltip("Czas w sekundach miedzy kolejnymi kopaniami.")]
    public float miningCooldown = 1.0f;

    private float lastMineTime = -999f;
    private int mineralsCollected = 0;

    // Maski warstw – Mineral i ground
    private int miningLayerMask;

    void Awake()
    {
        Instance = this;
        Debug.Log("✅ PlayerMining Instance ustawione");
    }

    void Start()
    {
        // Ustawiamy mase z dokladnymi nazwami warstw
        miningLayerMask = LayerMask.GetMask("Mineral", "ground");
        Debug.Log("🎯 Warstwy kopania ustawione: Mineral + ground");
    }

    void Update()
    {
        if (Input.GetKeyDown(mineKey) && Time.time - lastMineTime >= miningCooldown)
        {
            TryMine();
            lastMineTime = Time.time;
        }
    }

    void TryMine()
    {
        // Kierunek kopania zalezy od kierunku, w ktorym zwrocony jest gracz
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        // Promien debugowy w scenie (czerwony)
        Debug.DrawRay(transform.position, direction * miningRange, Color.red, 1f);

        // Raycast tylko po warstwach Mineral i ground
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, miningRange, miningLayerMask);

        if (hit.collider == null)
        {
            Debug.Log("⛏️ Nic nie trafiono!");
            return;
        }

        string layerName = LayerMask.LayerToName(hit.collider.gameObject.layer);
        Debug.Log($"🔸 Trafiono: {hit.collider.name} (Layer: {layerName})");

        // 💎 Minerały
        if (layerName == "Mineral")
        {
            MineralBlock mineral = hit.collider.GetComponent<MineralBlock>();
            if (mineral != null)
            {
                Debug.Log("💎 Trafiono mineral!");
                mineral.BreakBlock();
                return;
            }
        }

        // 🪨 Kamienie (layer ground)
        if (layerName == "ground")
        {
            StoneBlock stone = hit.collider.GetComponent<StoneBlock>();
            if (stone != null)
            {
                Debug.Log("🪨 Trafiono kamien!");
                stone.BreakBlock();
                return;
            }
        }

        Debug.Log("⚠️ Trafiono obiekt bez skryptu MineralBlock ani StoneBlock!");
    }

    public void AddMinerals(int amount)
    {
        mineralsCollected += amount;

        if (MineralUIManager.Instance != null)
        {
            MineralUIManager.Instance.UpdateMineralCount(mineralsCollected, amount);
        }
        else
        {
            Debug.LogWarning("⚠️ Brak MineralUIManager.Instance w scenie!");
        }
    }

    // NOWE METODY – do ulepszania damage

    public int GetMineralCount()
    {
        return mineralsCollected;
    }

    public void SpendMinerals(int amount)
    {
        mineralsCollected -= amount;
        mineralsCollected = Mathf.Max(0, mineralsCollected);

        if (MineralUIManager.Instance != null)
        {
            MineralUIManager.Instance.UpdateMineralCount(mineralsCollected, 0); // bez animacji koloru
        }
    }
}

