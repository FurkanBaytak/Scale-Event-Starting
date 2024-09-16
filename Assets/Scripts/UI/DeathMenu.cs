using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Audio;

public class DeathMenu : MonoBehaviour
{
    public GameObject deathMenuUI;
    public GameObject mainMenuButton;
    public TMP_Text survivalTimeText;
    public TMP_Text finalSurvivalTimeText;
    public DeathHandler deathHandler;
    public AudioSource audioSource;
    public AudioMixer audioMixer;

    void Update()
    {
        if (!deathHandler.IsAlive)
        {
            ShowDeathMenu();
        }
    }

    public void ShowDeathMenu()
    {
        deathMenuUI.SetActive(true);
        mainMenuButton.SetActive(true);
        Time.timeScale = 0f;
        finalSurvivalTimeText.text = survivalTimeText.text;
        audioSource.spatialBlend = 0.9f;
    }

    public void RestartGame()
    {
        audioMixer.SetFloat("Pitch", 1.0f);
        Time.timeScale = 1f;
        deathHandler.IsAlive = true;
        audioSource.spatialBlend = 0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        LoadMainMenu();
    }

    private void LoadMainMenu()
    {
        Time.timeScale = 1f;
        deathHandler.IsAlive = true;
        audioSource.spatialBlend = 0f;

        SceneManager.LoadScene("StartMenu");
    }
}
