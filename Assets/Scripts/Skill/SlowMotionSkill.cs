using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Collections;

public class SlowMotionSkill : MonoBehaviour
{
    public Volume globalVolume;
    public AudioMixer audioMixer;
    public float slowMotionScale = 0.5f;
    public float transitionDuration = 1.0f;
    public float maxUsageTime = 5.0f;
    public Slider skillSlider;

    private ChromaticAberration chromaticAberration;
    private PaniniProjection paniniProjection;
    private Vignette vignette;

    public GameObject spriteObject;
    public Sprite slowMotionSprite;
    public Sprite normalSprite;
    private SpriteRenderer spriteRenderer;

    private bool isSlowMotionActive = false;
    private bool isTransitioning = false;
    private float originalTimeScale;
    private float originalFixedDeltaTime;
    private float originalChromaticAberrationIntensity;
    private float originalPaniniProjectionDistance;
    private float originalVignetteIntensity;
    private float currentUsageTime;
    private bool isRecharging = false;

    private void Start()
    {
        originalTimeScale = Time.timeScale;
        originalFixedDeltaTime = Time.fixedDeltaTime;

        globalVolume.profile.TryGet(out chromaticAberration);
        globalVolume.profile.TryGet(out paniniProjection);
        globalVolume.profile.TryGet(out vignette);

        originalChromaticAberrationIntensity = chromaticAberration.intensity.value;
        originalPaniniProjectionDistance = paniniProjection.distance.value;
        originalVignetteIntensity = vignette.intensity.value;

        if (spriteObject != null)
        {
            spriteRenderer = spriteObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && normalSprite != null)
            {
                spriteRenderer.sprite = normalSprite;
            }
        }

        currentUsageTime = maxUsageTime;
        skillSlider.minValue = 0f;
        skillSlider.maxValue = maxUsageTime;
        skillSlider.value = currentUsageTime;
    }

    public void UpdateSkill(float deltaTime, bool isKeyHeld)
    {
        if (isRecharging || currentUsageTime <= 0f) return;

        if (isKeyHeld && !isTransitioning && currentUsageTime > 0f)
        {
            if (!isSlowMotionActive)
            {
                StartCoroutine(ApplySlowMotion());
            }

            currentUsageTime -= deltaTime;
            skillSlider.value = currentUsageTime;

            if (currentUsageTime <= 0f)
            {
                StartCoroutine(RevertSlowMotion());
            }
        }
        else if (isSlowMotionActive)
        {
            StartCoroutine(RevertSlowMotion());
        }
    }

    public void ToggleSlowMotion()
    {
        if (isRecharging || currentUsageTime <= 0f) return;

        if (isSlowMotionActive)
        {
            StartCoroutine(RevertSlowMotion());
        }
        else
        {
            StartCoroutine(ApplySlowMotion());
        }
    }

    private IEnumerator ApplySlowMotion()
    {
        isTransitioning = true;
        float elapsedTime = 0f;

        if (spriteRenderer != null && slowMotionSprite != null)
        {
            spriteRenderer.sprite = slowMotionSprite;
        }

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            Time.timeScale = Mathf.Lerp(originalTimeScale, slowMotionScale, elapsedTime / transitionDuration);
            Time.fixedDeltaTime = Time.timeScale * 0.02f;

            audioMixer.SetFloat("Pitch", Mathf.Lerp(1.0f, slowMotionScale, elapsedTime / transitionDuration));

            chromaticAberration.intensity.value = Mathf.Lerp(originalChromaticAberrationIntensity, 0.6f, elapsedTime / transitionDuration);
            paniniProjection.distance.value = Mathf.Lerp(originalPaniniProjectionDistance, 0.6f, elapsedTime / transitionDuration);
            vignette.intensity.value = Mathf.Lerp(originalVignetteIntensity, 0.55f, elapsedTime / transitionDuration);

            yield return null;
        }

        isSlowMotionActive = true;
        isTransitioning = false;
    }

    private IEnumerator RevertSlowMotion()
    {
        isTransitioning = true;
        float elapsedTime = 0f;

        if (spriteRenderer != null && normalSprite != null)
        {
            spriteRenderer.sprite = normalSprite;
        }

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            Time.timeScale = Mathf.Lerp(slowMotionScale, originalTimeScale, elapsedTime / transitionDuration);
            Time.fixedDeltaTime = Time.timeScale * 0.02f;

            audioMixer.SetFloat("Pitch", Mathf.Lerp(slowMotionScale, 1.0f, elapsedTime / transitionDuration));

            chromaticAberration.intensity.value = Mathf.Lerp(0.6f, originalChromaticAberrationIntensity, elapsedTime / transitionDuration);
            paniniProjection.distance.value = Mathf.Lerp(0.6f, originalPaniniProjectionDistance, elapsedTime / transitionDuration);
            vignette.intensity.value = Mathf.Lerp(0.55f, originalVignetteIntensity, elapsedTime / transitionDuration);

            yield return null;
        }

        Time.timeScale = originalTimeScale;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        audioMixer.SetFloat("Pitch", 1.0f);
        chromaticAberration.intensity.value = originalChromaticAberrationIntensity;
        paniniProjection.distance.value = originalPaniniProjectionDistance;
        vignette.intensity.value = originalVignetteIntensity;

        isSlowMotionActive = false;
        isTransitioning = false;

        float rechargeTime = (maxUsageTime - currentUsageTime) * 2f;
        StartCoroutine(RechargeSkill(rechargeTime));
    }

    private IEnumerator RechargeSkill(float rechargeTime)
    {
        isRecharging = true;
        float elapsedTime = 0f;
        float startValue = currentUsageTime;

        while (elapsedTime < rechargeTime)
        {
            elapsedTime += Time.deltaTime;
            currentUsageTime = Mathf.Lerp(startValue, maxUsageTime, elapsedTime / rechargeTime);
            skillSlider.value = currentUsageTime;
            yield return null;
        }

        currentUsageTime = maxUsageTime;
        skillSlider.value = currentUsageTime;
        isRecharging = false;
    }

    public bool IsSlowMotionActive()
    {
        return isSlowMotionActive;
    }

    public bool IsTransitioning()
    {
        return isTransitioning;
    }
}
