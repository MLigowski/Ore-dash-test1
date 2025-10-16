using UnityEngine;
using TMPro;  // Pami�taj o tym, je�li u�ywasz TextMeshPro

public class NPCDialogue : MonoBehaviour
{
    public GameObject dialoguePanel;  // Panel dialogowy
    public TMP_Text dialogueText;     // Tekst dialogu (TextMeshPro)
    public string[] dialogueLines;    // Linie dialogu

    private int currentLine = 0;
    private bool playerNearby = false;

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
            else
            {
                currentLine++;
                if (currentLine < dialogueLines.Length)
                {
                    ShowLine();
                }
                else
                {
                    dialoguePanel.SetActive(false);
                }
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
            // Tu mo�esz doda� np. podpowied� "Naci�nij E, aby rozmawia�"
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            dialoguePanel.SetActive(false);
        }
    }
}

