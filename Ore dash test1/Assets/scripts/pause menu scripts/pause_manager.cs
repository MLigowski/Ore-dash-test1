using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public int pauseSceneIndex = 2;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        SceneManager.LoadScene(pauseSceneIndex, LoadSceneMode.Additive);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        SceneManager.UnloadSceneAsync(pauseSceneIndex);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;                 // ?? odmra¿amy grê
        SceneManager.LoadScene(0);           // ?? scena 0 = main menu
    }
}
