using UnityEngine;
using TMPro;

public class NPCDialogueHealth : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public string[] dialogueLines;

    public HealthUpgrade healthUpgrade;

    private int currentLine = 0;
    private bool playerNearby = false;
    private bool waitingForChoice = false;

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            if (!dialoguePanel.activeSelf)
            {
                dialoguePanel.SetActive(true);
                currentLine = 0;
                ShowLine();
            }
            else if (!waitingForChoice)
            {
                currentLine++;
                if (currentLine < dialogueLines.Length)
                {
                    ShowLine();
                }
                else
                {
                    dialogueText.text = "Exchange ores for more health? [Y/N]";
                    waitingForChoice = true;
                }
            }
            else if (waitingForChoice)
            {
                // Czekamy na Y/N, nic nie robimy
            }
        }

        if (waitingForChoice)
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                if (healthUpgrade != null)
                {
                    healthUpgrade.UpgradeHealth();
                }
                dialoguePanel.SetActive(false);
                waitingForChoice = false;
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
                dialoguePanel.SetActive(false);
                waitingForChoice = false;
            }
        }
    }

    void ShowLine()
    {
        if (dialogueLines.Length > 0 && currentLine < dialogueLines.Length)
        {
            dialogueText.text = dialogueLines[currentLine];
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerNearby = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            dialoguePanel.SetActive(false);
            waitingForChoice = false;
        }
    }
}