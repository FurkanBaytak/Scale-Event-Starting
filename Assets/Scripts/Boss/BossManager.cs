using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Cinemachine;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class BossManager : MonoBehaviour
{
    public string bossTime;
    public Slider timerSlider;
    public TMP_Text timerText;
    public TMP_Text bossText;
    private TextAnimation textAnimation;
    public GameObject bossPrefab;
    public Transform bossSpawnPoint;
    public GameObject globalLight2D;
    public GameObject timerGameObject;
    public CinemachineVirtualCamera virtualCamera;
    public AudioSource backgroundMusic;
    public AudioSource bossMusic;
    public AudioSource explosionAudioSource;
    public AudioClip explosionSound;
    public WinMenu winMenu;
    public List<GameObject> otherObjectsToDeactivate;
    public static BossManager Instance;

    private ColorCycle colorCycle;
    private CinemachineBasicMultiChannelPerlin noise;
    private float initialAmplitudeGain;
    private float initialFrequencyGain;

    private bool bossSpawned = false;
    private GameObject currentBoss;

    public Slider LazerbossHealthSlider;
    public TMP_Text LazerbossNameText;

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

        explosionAudioSource.clip = explosionSound;
        explosionAudioSource.loop = false;
        textAnimation = bossText.GetComponent<TextAnimation>();
    }

    void Update()
    {
        if (timerText.text == bossTime && !bossSpawned)
        {
            StartBossFight();
        }

        if (bossSpawned && currentBoss == null)
        {
            BossDefeated();
        }
    }

    void StartBossFight()
    {
        bossSpawned = true;

        StartCoroutine(SmoothOrthoSizeChange(8f, 2f));

        colorCycle.enabled = true;

        timerGameObject.SetActive(false);

        DestroyAllEnemies();

        currentBoss = Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity);
        currentBoss.tag = "Boss";

        // Slider ve boss adý için yeni kodlar
        LazerbossHealthSlider.maxValue = currentBoss.GetComponent<LaserBoss>().maxHealth;
        LazerbossHealthSlider.value = LazerbossHealthSlider.maxValue; // Baþlangýçta tam dolu olsun
        LazerbossNameText.text = "Laser Boss"; // Boss'un adý
        LazerbossHealthSlider.gameObject.SetActive(true);
        LazerbossNameText.gameObject.SetActive(true);

        foreach (GameObject obj in otherObjectsToDeactivate)
        {
            obj.SetActive(false);
        }

        StartCoroutine(CameraShakeRoutine());

        // Directly stop the background music and start the boss music without fading
        backgroundMusic.Stop();
        bossMusic.Play();

        StartCoroutine(ShowAnimatedText());
    }

    void DestroyAllEnemies()
    {
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("enemy"))
        {
            Destroy(enemy);
        }
    }

    void BossDefeated()
    {
        StartCoroutine(WinRoutine());
        colorCycle.enabled = false;
        var light = globalLight2D.GetComponent<Light2D>();
        light.color = Color.white;

        // Slider ve boss adý için eklenen kodlar
        LazerbossHealthSlider.gameObject.SetActive(false);
        LazerbossNameText.gameObject.SetActive(false);
    }

    void PlayDefeatSound()
    {
        if (explosionAudioSource != null)
        {
            explosionAudioSource.PlayOneShot(explosionSound);
        }
    }

    IEnumerator WinRoutine()
    {
        yield return new WaitForSeconds(3f);
        winMenu.ShowWinMenu();
    }

    IEnumerator CameraShakeRoutine()
    {
        noise.m_AmplitudeGain = 3f;
        noise.m_FrequencyGain = 3f;

        yield return new WaitForSeconds(3f);

        float time = 0f;
        float duration = 2f;

        while (time < duration)
        {
            time += Time.deltaTime;
            noise.m_AmplitudeGain = Mathf.Lerp(3f, initialAmplitudeGain * 5, time / duration);
            noise.m_FrequencyGain = Mathf.Lerp(3f, initialFrequencyGain * 5, time / duration);
            yield return null;
        }

        noise.m_AmplitudeGain = initialAmplitudeGain * 5;
        noise.m_FrequencyGain = initialFrequencyGain * 5;
    }

    private IEnumerator ShowAnimatedText()
    {
        textAnimation.InitializeText();
        textAnimation.StartTextAnimation();

        yield return new WaitForSeconds(5f);

        yield return StartCoroutine(textAnimation.ReverseTextAnimation());
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

    public void UpdateSliderForPhaseTwo()
    {
        LazerbossHealthSlider.fillRect.GetComponent<Image>().color = Color.red;
        StartCoroutine(ShakeFillArea());
    }

    private IEnumerator ShakeFillArea()
    {
        RectTransform fillAreaRect = LazerbossHealthSlider.fillRect.GetComponent<RectTransform>();
        Vector3 originalPosition = fillAreaRect.localPosition;
        float duration = 0.5f;
        float magnitude = 5f;

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
