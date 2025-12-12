using UnityEngine;

public class KeybindManager : MonoBehaviour
{
    public static KeybindManager Instance;

    public KeyCode DashKey = KeyCode.LeftShift;
    public KeyCode DigKey = KeyCode.E;
    public KeyCode defaultDash = KeyCode.LeftShift;
    public KeyCode defaultDig = KeyCode.E;

    private string waitingForKey = null;

    void Awake()
    {
        if (Instance == null) Instance = this;

        DashKey = (KeyCode)PlayerPrefs.GetInt("dash_key", (int)KeyCode.LeftShift);
        DigKey = (KeyCode)PlayerPrefs.GetInt("dig_key", (int)KeyCode.E);

        if (DashKey == KeyCode.None) DashKey = KeyCode.LeftShift;
        if (DigKey == KeyCode.None) DigKey = KeyCode.E;
       
    }


    void OnGUI()
    {
        if (waitingForKey != null)
        {
            Event e = Event.current;
            if (e.isKey)
            {
                AssignKey(waitingForKey, e.keyCode);
                waitingForKey = null;
            }
        }
    }

    public void StartRebinding(string keyName)
    {
        waitingForKey = keyName;
        Debug.Log("Press any key to bind " + keyName);
    }

    private void AssignKey(string keyName, KeyCode newKey)
    {
        if (keyName == "Dash")
        {
            DashKey = newKey;
            PlayerPrefs.SetInt("dash_key", (int)newKey);
        }
        else if (keyName == "Dig")
        {
            DigKey = newKey;
            PlayerPrefs.SetInt("dig_key", (int)newKey);
        }

        PlayerPrefs.Save();
        Debug.Log($"{keyName} set to {newKey}");
    }
    public KeyCode GetKey(string keyName)
    {
        if (keyName == "Dash") return DashKey;
        if (keyName == "Dig") return DigKey;

        return KeyCode.None; // bezpieczny fallback
    }
}
