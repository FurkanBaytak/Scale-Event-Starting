using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Cinemachine;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class MiniBossManager : MonoBehaviour
{
    public string miniBossTime;
    public TMP_Text timerText;
    public TMP_Text miniBossText;
    public GameObject miniBossPrefab;
    public Transform miniBossSpawnPoint;
    public GameObject globalLight2D;
    public CinemachineVirtualCamera virtualCamera;
    public List<GameObject> objectsToDeactivate;
    public AudioSource explosionAudioSource;
    public AudioClip explosionSound;
    public AudioClip defeatSound;
    public AudioSource miniBossMusic;
    public AudioSource backgroundMusic;
    private TextAnimation textAnimation;

    private ColorCycle colorCycle;
    private CinemachineBasicMultiChannelPerlin noise;
    private float initialAmplitudeGain;
    private float initialFrequencyGain;
    private Color initialLightColor;
    private float initialCameraSize;
    private bool miniBossSpawned = false;
    private GameObject currentMiniBoss;
    private bool miniBossDefeatedTextShown = false;

    public static MiniBossManager Instance;
    public bool IsBossFightActive { get; private set; }
    public EnemySpawner enemySpawner;
    public SpriteRenderer defeatedSpriteRenderer;
    public Sprite defeatedSprite;

    public Slider bossHealthSlider;
    public TMP_Text bossNameText;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        colorCycle = globalLight2D.GetComponent<ColorCycle>();
        noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        initialAmplitudeGain = noise.m_AmplitudeGain;
        initialFrequencyGain = noise.m_FrequencyGain;
        initialLightColor = globalLight2D.GetComponent<Light2D>().color;
        initialCameraSize = virtualCamera.m_Lens.OrthographicSize;
        textAnimation = miniBossText.GetComponent<TextAnimation>();

        explosionAudioSource.clip = explosionSound;
        explosionAudioSource.loop = false;
    }

    void Update()
    {
        if (timerText.text == miniBossTime && !miniBossSpawned)
        {
            StartMiniBossFight();
        }

        if (miniBossSpawned && currentMiniBoss == null && !miniBossDefeatedTextShown)
        {
            MiniBossDefeated();
        }
    }

    void StartMiniBossFight()
    {
        IsBossFightActive = true;
        enemySpawner.StopSpawningEnemies();
        miniBossSpawned = true;
        StartCoroutine(SmoothOrthoSizeChange(6f, 1.5f));

        DestroyAllEnemies();

        currentMiniBoss = Instantiate(miniBossPrefab, miniBossSpawnPoint.position, Quaternion.identity);
        currentMiniBoss.tag = "MiniBoss";

        foreach (GameObject obj in objectsToDeactivate)
        {
            obj.SetActive(false);
        }

        // GlobalLight2D ColorCycle aktif
        // colorCycle.enabled = true;

        // Patlama efekti çal
        PlayExplosionSound();

        StartCoroutine(FadeInMusic(miniBossMusic, 0.5f));
        StartCoroutine(FadeOutMusic(backgroundMusic, 0.5f));

        StartCoroutine(ShowAnimatedText("Difficulty Scale Up !"));
        StartCoroutine(CameraShakeRoutineMiniBoss());

        // Initialize the slider with max health and display the name
        bossHealthSlider.maxValue = currentMiniBoss.GetComponent<MiniBoss>().maxHealth;
        bossHealthSlider.value = bossHealthSlider.maxValue; // Set it to full health initially
        bossNameText.text = "Mushroom MiniBoss"; // Set the name of the MiniBoss
        bossHealthSlider.gameObject.SetActive(true);
        bossNameText.gameObject.SetActive(true);
    }

    void PlayExplosionSound()
    {
        if (!explosionAudioSource.isPlaying)
        {
            explosionAudioSource.Play();
        }
    }

    void MiniBossDefeated()
    {
        IsBossFightActive = false;
        PlayDefeatSound();

        StartCoroutine(FadeOutMusic(miniBossMusic, 0.51f, PlayBackgroundMusicWithDelay));
        // Hide the slider and name text
        bossHealthSlider.gameObject.SetActive(false);
        bossNameText.gameObject.SetActive(false);

        enemySpawner.ResumeSpawningEnemies();
        miniBossDefeatedTextShown = true;
        defeatedSpriteRenderer.sprite = defeatedSprite;
        StartCoroutine(ShowMiniBossDefeatedText());
    }

    void PlayDefeatSound()
    {
        // Burada ses efektini çalabilirsiniz
        if (defeatSound != null)
        {
            explosionAudioSource.PlayOneShot(defeatSound);
        }
    }

    IEnumerator ShowMiniBossDefeatedText()
    {
        // MiniBoss öldüðünde yazýyý göster
        miniBossText.text = "MiniBoss Defeated!";
        textAnimation.InitializeText();
        textAnimation.StartTextAnimation();

        yield return new WaitForSeconds(3f);

        // Yazýyý kaldýr
        yield return StartCoroutine(textAnimation.ReverseTextAnimation());
        miniBossText.text = "";

        StartCoroutine(FadeOutMusic(miniBossMusic, 0.51f));
        StartCoroutine(FadeInMusic(backgroundMusic, 0.5f));

        StartCoroutine(ResetSceneAfterMiniBoss());
    }

    IEnumerator ResetSceneAfterMiniBoss()
    {
        StartCoroutine(SmoothOrthoSizeChange(initialCameraSize, 2f));
        yield return new WaitForSeconds(2f);

        foreach (GameObject obj in objectsToDeactivate)
        {
            obj.SetActive(true);
        }

        var light = globalLight2D.GetComponent<Light2D>();
        light.color = initialLightColor;

        colorCycle.enabled = false;
    }

    IEnumerator CameraShakeRoutineMiniBoss()
    {
        noise.m_AmplitudeGain = 2f;
        noise.m_FrequencyGain = 2f;

        yield return new WaitForSeconds(2f);

        noise.m_AmplitudeGain = initialAmplitudeGain;
        noise.m_FrequencyGain = initialFrequencyGain;
    }

    private IEnumerator SmoothOrthoSizeChange(float targetSize, float duration)
    {
        float startSize = virtualCamera.m_Lens.OrthographicSize;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(startSize, targetSize, time / duration);
            yield return null;
        }

        virtualCamera.m_Lens.OrthographicSize = targetSize;
    }

    private IEnumerator ShowAnimatedText(string message)
    {
        miniBossText.text = message;
        textAnimation.InitializeText();
        textAnimation.StartTextAnimation();

        yield return new WaitForSeconds(5f);

        yield return StartCoroutine(textAnimation.ReverseTextAnimation());
    }

    private void DestroyAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
    }

    private IEnumerator FadeOutMusic(AudioSource audioSource, float duration, System.Action onComplete = null)
    {
        float startVolume = audioSource.volume;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / duration);
            yield return null;
        }

        audioSource.volume = 0;
        audioSource.Stop();

        onComplete?.Invoke();
    }

    private void PlayBackgroundMusicWithDelay()
    {
        StartCoroutine(FadeInMusic(backgroundMusic, 0.5f));
    }


    private IEnumerator FadeInMusic(AudioSource audioSource, float duration)
    {
        audioSource.volume = 0;
        audioSource.Play();

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0, 1, t / duration);
            yield return null;
        }

        audioSource.volume = 1;
    }

    public void UpdateSliderForPhaseTwo()
    {
        bossHealthSlider.fillRect.GetComponent<Image>().color = Color.red;

        StartCoroutine(ShakeFillArea());
    }

    private IEnumerator ShakeFillArea()
    {
        RectTransform fillAreaRect = bossHealthSlider.fillRect.GetComponent<RectTransform>();
        Vector3 originalPosition = fillAreaRect.localPosition;
        float duration = 0.01f;
        float magnitude = 1f;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            fillAreaRect.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        fillAreaRect.localPosition = originalPosition;
    }
}
