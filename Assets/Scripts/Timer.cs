using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Cinemachine;

public class Timer : MonoBehaviour
{
    public Slider slider;
    public ParticleSystem particle;
    public Image sliderFill;
    public TMP_Text timerText;
    public TMP_Text animatedText;
    public AudioSource audioExplode;
    public CinemachineVirtualCamera virtualCamera;

    public float totalDuration = 300f;
    private float currentTime = 0f;
    public float shakeMagnitude = 2f;
    private float nextBurstTime = 30f;

    private Vector3 originalPos;
    private CinemachineBasicMultiChannelPerlin virtualCameraNoise;

    private TextAnimation textAnimation;

    void Start()
    {
        slider.maxValue = totalDuration;
        slider.value = 0;
        originalPos = sliderFill.rectTransform.anchoredPosition;

        virtualCameraNoise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        textAnimation = animatedText.GetComponent<TextAnimation>();

        StartShakeLoop();
        ChangeColor();
        UpdateTimerText();
    }

    void Update()
    {
        if (currentTime < totalDuration)
        {
            currentTime += Time.deltaTime;
            slider.value = currentTime;

            UpdateTimerText();
        }
    }

    public float GetCurrentTime()
    {
        return currentTime;
    }

    public void StartEvent(BaseEvent baseEvent)
    {
        animatedText.text = baseEvent.GetName();
        audioExplode.Play();
        ChangeColor();
        PlayParticleEffect();
        StartShakeLoop();
        StartCoroutine(ScreenShake());
        StartCoroutine(ShowAnimatedText(baseEvent.GetEventDuration()));
    }

    private void ChangeColor()
    {
        Color newColor = new Color(Random.value, Random.value, Random.value);

        sliderFill.color = newColor;

        var mainModule = particle.main;
        mainModule.startColor = newColor;
    }

    private void PlayParticleEffect()
    {
        particle.Play();
    }

    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void StartShakeLoop()
    {
        DOTween.Kill(sliderFill.rectTransform);

        DOTween.To(() => 0f, x =>
        {
            float logFactor = Mathf.Log10(1 + x * 9) / Mathf.Log10(10);
            Vector2 shakeOffset = Random.insideUnitCircle * shakeMagnitude * logFactor;
            sliderFill.rectTransform.anchoredPosition = originalPos + (Vector3)shakeOffset;
        }, 1f, nextBurstTime)
        .SetEase(Ease.Linear)
        .OnComplete(() =>
        {
            sliderFill.rectTransform.anchoredPosition = originalPos;
            StartShakeLoop();
        });
    }

    private IEnumerator ScreenShake()
    {
        float elapsed = 0f;
        float duration = 0.2f;
        float initialMagnitude = shakeMagnitude * 5f;

        while (elapsed < duration)
        {
            float magnitude = Mathf.Lerp(initialMagnitude, shakeMagnitude, elapsed / duration);
            virtualCameraNoise.m_AmplitudeGain = magnitude * 1f;
            virtualCameraNoise.m_FrequencyGain = magnitude * 2f;

            elapsed += Time.deltaTime;

            yield return null;
        }

        virtualCameraNoise.m_AmplitudeGain = 0.2f;
        virtualCameraNoise.m_FrequencyGain = 0.3f;
    }

    private IEnumerator ShowAnimatedText(float duration)
    {
        textAnimation.InitializeText();
        textAnimation.StartTextAnimation();

        yield return new WaitForSeconds(duration);

        yield return StartCoroutine(textAnimation.ReverseTextAnimation());
    }
}
