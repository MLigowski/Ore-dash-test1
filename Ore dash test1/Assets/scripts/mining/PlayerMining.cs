using UnityEngine;

public class PlayerMining : MonoBehaviour
{
    public static PlayerMining Instance;

    [Header("Mining Settings")]
    public float miningRange = 1.5f;
    public int miningPower = 1;
    public KeyCode mineKey = KeyCode.Tab;
    public float miningCooldown = 1.0f;

    private float lastMineTime = -999f;
    private int mineralsCollected = 0;
    private int miningLayerMask;

    void Awake()
    {
        Instance = this;
        Debug.Log("PlayerMining Instance set");
    }

    void Start()
    {
        miningLayerMask = LayerMask.GetMask("Mineral", "ground");
        Debug.Log("Mining layers set: Mineral + ground");
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
        Vector2 direction = transform.localScale.x >= 0 ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, miningRange, miningLayerMask);

        Debug.DrawRay(transform.position, direction * miningRange, Color.red, 1f);

        if (hit.collider == null)
        {
            Debug.Log("Nothing hit");
            return;
        }

        string layerName = LayerMask.LayerToName(hit.collider.gameObject.layer);
        Debug.Log($"Hit: {hit.collider.name} (Layer: {layerName})");

        if (layerName == "Mineral")
        {
            MineralBlock mineral = hit.collider.GetComponent<MineralBlock>();
            if (mineral != null)
            {
                mineral.BreakBlock(miningPower);
                return;
            }
        }

        if (layerName == "ground")
        {
            StoneBlock stone = hit.collider.GetComponent<StoneBlock>();
            if (stone != null)
            {
                stone.BreakBlock(miningPower);
                return;
            }
        }

        Debug.Log("Hit object without MineralBlock or StoneBlock script");
    }

    public void AddMinerals(int amount)
    {
        mineralsCollected += amount;
        if (MineralUIManager.Instance != null)
        {
            MineralUIManager.Instance.UpdateMineralCount(mineralsCollected, amount);
        }
    }

    public int GetMineralCount()
    {
        return mineralsCollected;
    }

    public void SpendMinerals(int amount)
    {
        mineralsCollected -= amount;
        if (MineralUIManager.Instance != null)
        {
            MineralUIManager.Instance.UpdateMineralCount(mineralsCollected, 0);
        }
    }
}


