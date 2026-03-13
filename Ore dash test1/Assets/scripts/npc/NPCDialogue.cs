using UnityEngine;
using TMPro;

public class NPCDialogue : MonoBehaviour
{
    public GameObject dialoguePanel;        // Panel dialogowy
    public TMP_Text dialogueText;           // Tekst dialogu (TextMeshPro)
    public string[] dialogueLines;          // Linie dialogu przed pytaniem
    public DamageUpgrade damageUpgrade;     // Referencja do skryptu DamageUpgrade

    [Header("Quest Icon")]
    public GameObject exclamationMark;      // Wykrzyknik nad NPC

    private int currentLine = 0;
    private bool playerNearby = false;
    private bool waitingForChoice = false;
    private bool dialogLinesFinished = false;   // czy linie z inspektora zosta³y przeczytane

    void Start()
    {
        if (exclamationMark != null)
            exclamationMark.SetActive(true); // wykrzyknik dopóki dialog nie zakoñczony
    }

    void Update()
    {
        if (!playerNearby) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!dialoguePanel.activeSelf)
            {
                dialoguePanel.SetActive(true);

                // Jeœli linie dialogowe nie by³y przeczytane, pokazujemy je
                if (!dialogLinesFinished && dialogueLines.Length > 0)
                {
                    currentLine = 0;
                    waitingForChoice = false;
                    ShowLine();
                }
                else
                {
                    // Jeœli linie dialogowe ju¿ by³y przeczytane, pokazujemy pytanie Y/N
                    dialogueText.text = "exchange ores for more damage? [Y/N]";
                    waitingForChoice = true;
                }
            }
            else if (!waitingForChoice && !dialogLinesFinished)
            {
                // Kontynuacja wyœwietlania linii dialogowych
                currentLine++;
                if (currentLine < dialogueLines.Length)
                {
                    ShowLine();
                }
                else
                {
                    // Linie dialogowe zakoñczone
                    dialogLinesFinished = true;
                    dialogueText.text = "exchange ores for more damage? [Y/N]";
                    waitingForChoice = true;
                }
            }
        }

        // Obs³uga wyboru Y/N
        if (waitingForChoice)
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                if (damageUpgrade != null)
                    damageUpgrade.UpgradeDamage();

                EndDialogue(); // dopiero po wyborze Y/N panel i wykrzyknik znikaj¹
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
                EndDialogue();
            }
        }
    }

    void ShowLine()
    {
        dialogueText.text = dialogueLines[currentLine];
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        waitingForChoice = false;

        if (exclamationMark != null)
            exclamationMark.SetActive(false); // wykrzyknik znika po zakoñczeniu Y/N
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;

            // Ukryj panel tylko jeœli gracz przerwa³ linie dialogowe
            if (!waitingForChoice)
                dialoguePanel.SetActive(false);
        }
    }
}