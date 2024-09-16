using TMPro;
using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public AudioSource audioSource;
    public bool isPaused = false;
    public TMP_Text countdown;
    private SlowMotionSkill slowMotionSkill;

    void Start()
    {
        slowMotionSkill = FindObjectOfType<SlowMotionSkill>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (slowMotionSkill != null && (slowMotionSkill.IsTransitioning() || slowMotionSkill.IsSlowMotionActive()))
            {
                return;
            }

            if (isPaused)
            {
                StartCoroutine(ResumeCountdown());
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        StartCoroutine(ResumeCountdown());
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        audioSource.spatialBlend = 0.9f;
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
    }

    private IEnumerator ResumeCountdown()
    {
        countdown.gameObject.SetActive(true);

        for (int i = 3; i > 0; i--)
        {
            countdown.text = i.ToString();
            yield return new WaitForSecondsRealtime(1f);
        }

        countdown.gameObject.SetActive(false);
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        audioSource.spatialBlend = 0f;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
