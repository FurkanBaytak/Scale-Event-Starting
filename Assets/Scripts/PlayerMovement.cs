using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class PlayerMovement : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public float moveSpeed;
    public float dashSpeed;
    public float dashRate;
    public float dashSize;
    private AudioSource audioSource;
    public AudioClip takeDamageSound;
    public AudioClip dashSound;
    public bool isGodMode;
    public WeaponBase WeaponBase;

    public int maxHealth = 5;
    public GameObject heartPrefab;
    public Transform heartsContainer;
    public Sprite fullHeartSprite;
    public Sprite emptyHeartSprite;

    private int currentHealth;
    private int totalHealth;
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private Vector2 mousePosition;
    private float lastDashTime;
    private bool isDashing;
    private Camera mainCamera;
    private float totalDashTime = 0.3f;
    private DeathHandler deathHandler;
    private CinemachineBasicMultiChannelPerlin camNoise;
    public Timer timer;

    private SlowMotionSkill slowMotionSkill;

    public Transform gun;
    public Transform AmmoSpawn;
    public float gunDistanceFromPlayer = 1.5f;

    private float keyHoldTime = 0f;
    public float requiredHoldTime = 0.2f;

    public SpriteRenderer spriteRenderer;
    public float invincibilityDuration = 3.0f;
    private bool isInvincible;

    public float pistolGunDistance = 1.5f;
    public float uziGunDistance = 1.8f;
    public float shotgunGunDistance = 1.7f;
    public float tomyGunDistance = 2.0f;
    public float bouncingGunDistance = 1.9f;
    public float rpgGunDistance = 2.5f;
    public float machineGunDistance = 2.2f;

    public float minX, maxX, minY, maxY;

    private void Start()
    {
        deathHandler = DeathHandler.GetInstance();
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        deathHandler.IsAlive = true;
        camNoise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        slowMotionSkill = GetComponent<SlowMotionSkill>();
        currentHealth = 2;
        totalHealth = 2;
        UpdateHeartsUI();
        StartCoroutine(HealthIncreaseOverTime());
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!deathHandler.IsAlive) return;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        if (Input.GetMouseButton(0))
        {
            WeaponBase.Shoot();
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            Dash();
        }

        if (Input.GetKey(KeyCode.Space))
        {
            keyHoldTime += Time.deltaTime;

            if (keyHoldTime >= requiredHoldTime && !slowMotionSkill.IsSlowMotionActive() && !slowMotionSkill.IsTransitioning())
            {
                slowMotionSkill.ToggleSlowMotion();
            }

            if (slowMotionSkill.IsSlowMotionActive())
            {
                slowMotionSkill.UpdateSkill(Time.deltaTime, true);
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (slowMotionSkill.IsSlowMotionActive())
            {
                slowMotionSkill.ToggleSlowMotion();
            }

            keyHoldTime = 0f;
        }

        mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        moveDirection = new Vector2(moveX, moveY).normalized;

        UpdateGunDistance();

        Vector2 aimDirection = mousePosition - (Vector2)transform.position;
        aimDirection.Normalize();
        gun.position = (Vector2)transform.position + aimDirection * gunDistanceFromPlayer;

        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        gun.rotation = Quaternion.Euler(0, 0, angle);

        Vector3 gunScale = gun.localScale;

        if (angle > 90 || angle < -90)
        {
            gunScale.y = Mathf.Abs(gunScale.y) * -1;
            AmmoSpawn.localRotation = Quaternion.Euler(0, 0, 90);
        }
        else
        {
            gunScale.y = Mathf.Abs(gunScale.y);
            AmmoSpawn.localRotation = Quaternion.Euler(0, 0, -90);
        }

        gun.localScale = gunScale;
    }

    void FixedUpdate()
    {
        if (!deathHandler.IsAlive) return;

        if (!isDashing)
        {
            rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);

            LimitPosition();
        }

    }

    private void LimitPosition()
    {
        float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
        float clampedY = Mathf.Clamp(transform.position.y, minY, maxY);

        transform.position = new Vector2(clampedX, clampedY);
    }

    public void Dash()
    {
        if (Time.time - lastDashTime > dashRate)
        {
            float moveX = moveDirection.x;
            float moveY = moveDirection.y;
            isGodMode = true;
            if (dashSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(dashSound);
            }
            for (int i = 0; i < dashSize; i++)
            {
                StartCoroutine(DoDashLogic(i, moveX, moveY));
                StartCoroutine(EndDash());
            }
        }
    }

    private IEnumerator DoDashLogic(int i, float moveX, float moveY)
    {
        yield return new WaitForSeconds((totalDashTime / dashSize) * i);
        float posX = transform.position.x;
        float posY = transform.position.y;
        lastDashTime = Time.time;
        isDashing = true;
        transform.position = new Vector2(posX + moveX * dashSpeed, posY + moveY * dashSpeed);

        LimitPosition();
    }

    private IEnumerator EndDash()
    {
        yield return new WaitForSeconds((totalDashTime / dashSize) * dashSize);
        isDashing = false;
        isGodMode = false;
    }

    public void TakeDamage(int damage)
    {
        if (isGodMode || isInvincible) return;

        if (takeDamageSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(takeDamageSound);
        }
        currentHealth = Mathf.Max(currentHealth - damage, 0);
        UpdateHeartsUI();

        if (currentHealth <= 0)
        {
            deathHandler.IsAlive = false;
        }

        StartCoroutine(ScreenShake());
        StartCoroutine(ActivateInvincibility());
    }

    private IEnumerator ScreenShake()
    {
        float elapsed = 0f;
        float duration = 0.2f;
        float initialMagnitude = 2 * 5f;

        while (elapsed < duration)
        {
            float magnitude = Mathf.Lerp(initialMagnitude, 2, elapsed / duration);
            camNoise.m_AmplitudeGain = magnitude * 0.5f;
            camNoise.m_FrequencyGain = magnitude * 1f;

            elapsed += Time.deltaTime;

            yield return null;
        }

        camNoise.m_AmplitudeGain = 0.2f;
        camNoise.m_FrequencyGain = 0.3f;
    }

    private IEnumerator ActivateInvincibility()
    {
        isInvincible = true;
        float blinkInterval = 0.1f;
        float elapsedTime = 0f;

        while (elapsedTime < invincibilityDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(blinkInterval);
            elapsedTime += blinkInterval;
        }

        spriteRenderer.enabled = true;
        isInvincible = false;
    }

    private IEnumerator HealthIncreaseOverTime()
    {
        bool firstHealthIncreaseDone = false;
        bool secondHealthIncreaseDone = false;
        bool thirdHealthIncreaseDone = false;

        while (true)
        {
            yield return new WaitUntil(() => !IsBossFightActive());

            if (timer.GetCurrentTime() >= 60 && totalHealth < maxHealth && !firstHealthIncreaseDone)
            {
                firstHealthIncreaseDone = true;
                Debug.Log("Health increase at 1 minute.");
                IncreaseHealth();
            }

            yield return new WaitUntil(() => !IsBossFightActive());

            if (timer.GetCurrentTime() >= 120 && totalHealth < maxHealth && !secondHealthIncreaseDone)
            {
                secondHealthIncreaseDone = true;
                Debug.Log("Health increase at 3 minutes.");
                IncreaseHealth();
            }

            yield return new WaitUntil(() => !IsBossFightActive());

            if (timer.GetCurrentTime() >= 180 && totalHealth < maxHealth && !thirdHealthIncreaseDone)
            {
                thirdHealthIncreaseDone = true;
                Debug.Log("Health increase at 5 minutes.");
                IncreaseHealth();
            }

            yield return null;
        }
    }


    private bool IsBossFightActive()
    {
        return MiniBossManager.Instance.IsBossFightActive;
    }


    private void IncreaseHealth()
    {
        if (totalHealth < maxHealth)
        {
            currentHealth++;
            totalHealth++;
            UpdateHeartsUI();
        }
    }

    private void UpdateHeartsUI()
    {
        foreach (Transform child in heartsContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < currentHealth; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartsContainer);
            Image heartImage = heart.GetComponent<Image>();
            heartImage.sprite = fullHeartSprite;

            if (heartImage.sprite.name == "health_1")
            {
                Color imageColor;
                ColorUtility.TryParseHtmlString("#026C00", out imageColor);
                heartImage.color = imageColor;

                UnityEngine.Rendering.Universal.Light2D light2D = heart.GetComponent<UnityEngine.Rendering.Universal.Light2D>();
                if (light2D != null)
                {
                    Color lightColor;
                    ColorUtility.TryParseHtmlString("#073300", out lightColor);
                    light2D.color = lightColor;
                }
            }
        }

        for (int i = currentHealth; i < totalHealth; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartsContainer);
            Image heartImage = heart.GetComponent<Image>();
            heartImage.sprite = emptyHeartSprite;

            if (heartImage.sprite.name == "health_1")
            {
                Color imageColor;
                ColorUtility.TryParseHtmlString("#026C00", out imageColor);
                heartImage.color = imageColor;

                UnityEngine.Rendering.Universal.Light2D light2D = heart.GetComponent<UnityEngine.Rendering.Universal.Light2D>();
                if (light2D != null)
                {
                    Color lightColor;
                    ColorUtility.TryParseHtmlString("#073300", out lightColor);
                    light2D.color = lightColor;
                }
            }
        }
    }
    private void UpdateGunDistance()
    {
        switch (WeaponBase.GetCurrentWeaponType())
        {
            case WeaponBase.WeaponType.Pistol:
                gunDistanceFromPlayer = pistolGunDistance;
                break;
            case WeaponBase.WeaponType.Uzi:
                gunDistanceFromPlayer = uziGunDistance;
                break;
            case WeaponBase.WeaponType.Shotgun:
                gunDistanceFromPlayer = shotgunGunDistance;
                break;
            case WeaponBase.WeaponType.Tomy:
                gunDistanceFromPlayer = tomyGunDistance;
                break;
            case WeaponBase.WeaponType.Bouncing:
                gunDistanceFromPlayer = bouncingGunDistance;
                break;
            case WeaponBase.WeaponType.RPG:
                gunDistanceFromPlayer = rpgGunDistance;
                break;
            case WeaponBase.WeaponType.MachineGun:
                gunDistanceFromPlayer = machineGunDistance;
                break;
            default:
                gunDistanceFromPlayer = 1.5f; // Varsayýlan deðer
                break;
        }
    }
}
