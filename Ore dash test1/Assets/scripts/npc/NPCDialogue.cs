using UnityEngine;
using TMPro;

public class NPCDialogue : MonoBehaviour
{
    public GameObject dialoguePanel;        // Panel dialogowy
    public TMP_Text dialogueText;           // Tekst dialogu (TextMeshPro)
    public string[] dialogueLines;          // Linie dialogu

    public DamageUpgrade damageUpgrade;     // Referencja do skryptu DamageUpgrade

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
                    // Po zakoñczeniu dialogu wywo³ujemy ulepszenie damage
                    if (damageUpgrade != null)
                    {
                        damageUpgrade.UpgradeDamage();
                    }
                    else
                    {
                        Debug.LogWarning("Brak przypisanego DamageUpgrade w NPCDialogue!");
                    }
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
            // Tu mo¿esz dodaæ np. podpowiedŸ "Naciœnij E, aby rozmawiaæ"
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

