using UnityEngine;
using TMPro;

public class NPCDialogue : MonoBehaviour
{
    public GameObject dialoguePanel;        // Panel dialogowy
    public TMP_Text dialogueText;           // Tekst dialogu (TextMeshPro)
    public string[] dialogueLines;          // Linie dialogu przed pytaniem

    public DamageUpgrade damageUpgrade;     // Referencja do skryptu DamageUpgrade

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
                    // Po ostatniej linii dialogu pytamy gracza o decyzjê
                    dialogueText.text = "Would you like to exchange ores for some strengh? [Y/N]";
                    waitingForChoice = true;
                }
            }
            else if (waitingForChoice)
            {
                // Nic tu nie robimy, czekamy na Y/N
            }
        }

        if (waitingForChoice)
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                // Gracz wybra³ TAK
                if (damageUpgrade != null)
                {
                    damageUpgrade.UpgradeDamage();
                }
                dialoguePanel.SetActive(false);
                waitingForChoice = false;
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
                // Gracz wybra³ NIE
                dialoguePanel.SetActive(false);
                waitingForChoice = false;
            }
        }
    }

    void ShowLine()
    {
        dialogueText.text = dialogueLines[currentLine];
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            // Mo¿esz tu dodaæ np. podpowiedŸ "Naciœnij E, aby rozmawiaæ"
        }
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
