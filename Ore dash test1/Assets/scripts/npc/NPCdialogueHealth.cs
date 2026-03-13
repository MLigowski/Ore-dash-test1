using UnityEngine;
using TMPro;

public class NPCDialogueHealth : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public string[] dialogueLines;

    public HealthUpgrade healthUpgrade;

    [Header("Quest Icon")]
    public GameObject exclamationMark;  // Wykrzyknik nad NPC

    private int currentLine = 0;
    private bool playerNearby = false;
    private bool waitingForChoice = false;
    private bool talked = false;       // czy gracz ju¿ rozmawia³

    void Start()
    {
        if (exclamationMark != null)
            exclamationMark.SetActive(!talked); // poka¿ jeœli jeszcze nie rozmawiano
    }

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
        }

        if (waitingForChoice)
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                if (healthUpgrade != null)
                    healthUpgrade.UpgradeHealth();

                EndDialogue();
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
                EndDialogue();
            }
        }
    }

    void ShowLine()
    {
        if (dialogueLines.Length > 0 && currentLine < dialogueLines.Length)
            dialogueText.text = dialogueLines[currentLine];
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        waitingForChoice = false;
        talked = true;

        if (exclamationMark != null)
            exclamationMark.SetActive(false); // po rozmowie wykrzyknik znika
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