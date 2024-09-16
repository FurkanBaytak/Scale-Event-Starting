using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class WinMenu: MonoBehaviour
{
    public GameObject winMenuUI;
    public GameObject mainMenuButton;
    public TMP_Text winText;
    public AudioSource audioSource;
    public GameObject boss;

    void Update()
    {
        if (boss == null)
        {
            ShowWinMenu();
        }
    }

    public void ShowWinMenu()
    {
        winMenuUI.SetActive(true);
        mainMenuButton.SetActive(true);
        audioSource.spatialBlend = 0.9f;
    }
}
