using UnityEngine;
using TMPro;

public class SettingsUI : MonoBehaviour
{
    public TMP_Text dashText;
    public TMP_Text digText;

    public KeyCode defaultDash = KeyCode.LeftShift;
    public KeyCode defaultDig = KeyCode.E;

    void Start()
    {
        if (KeybindManager.Instance.DashKey == KeyCode.None)
            KeybindManager.Instance.DashKey = defaultDash;

        if (KeybindManager.Instance.DigKey == KeyCode.None)
            KeybindManager.Instance.DigKey = defaultDig;

        UpdateTexts();
    }

    void UpdateTexts()
    {
        if (dashText != null)
            dashText.text = "Default: " + KeybindManager.Instance.DashKey;

        if (digText != null)
            digText.text = "Default: " + KeybindManager.Instance.DigKey;
    }

    public void RebindDash()
    {
        KeybindManager.Instance.StartRebinding("Dash");
    }

    public void RebindDig()
    {
        KeybindManager.Instance.StartRebinding("Dig");
    }
}
