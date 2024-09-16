using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class WeaponBase : MonoBehaviour
{
    public GameObject ammoPrefab;
    public GameObject bounceAmmoPrefab;
    public GameObject rpgProjectilePrefab;
    public GameObject ammoIconPrefab;
    public Transform ammoContainer;
    public GameObject ammoText;
    public Transform firePoint;
    public Sprite fullAmmoSprite;
    public Sprite emptyAmmoSprite;
    public TextMeshProUGUI ammoExtraText;
    public float ammoSpeed;
    public float ammoLifeSpan;
    public float reloadTime;
    public int maxAmmo;
    public float fireRate;
    public int currentAmmo;

    private float lastShotTime;
    private bool isReloading;
    private List<Image> ammoIcons;

    public AudioClip reloadSound;
    public AudioClip pickUpSound;
    private AudioSource audioSource;

    public AudioClip pistolFireSound;
    public AudioClip uziFireSound;
    public AudioClip shotgunFireSound;
    public AudioClip tomyFireSound;
    public AudioClip bouncingFireSound;
    public AudioClip rpgFireSound;
    public AudioClip machineGunFireSound;

    // Uzi özellikleri
    private float uziFireRate = 0.1f;
    private int uziAmmo = 50;

    // Shotgun özellikleri
    private int shotgunAmmo = 8;
    private float shotgunSpreadAngle = 15f;

    // Makineli tüfek özellikleri
    private float machineGunFireRate = 0.05f;
    private int machineGunAmmo = 150; // 250 mermilik kapasite
    private float firingConeAngle = 30f; // Koni açýsý

    // Seken mermi özellikleri
    private int bounceAmmo = 20;
    private int maxBounces = 3; // Sekme sayýsý
    private float bounceRadius = 5f; // Sekme menzili

    // RPG özellikleri
    private int rpgAmmo = 5;

    // Mevcut silah türü
    public enum WeaponType { Pistol, Uzi, Shotgun, Tomy, Bouncing, RPG, MachineGun }
    private WeaponType currentWeapon = WeaponType.Pistol;

    public GameObject gun;
    public Sprite newGunSprite;

    void Start()
    {
        currentWeapon = WeaponType.Pistol;
        SwitchToPistol();
        UpdateAmmoBoard();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo && !isReloading)
        {
            Reload();
        }

        if (Input.GetMouseButton(0) && currentWeapon == WeaponType.MachineGun)
        {
            Shoot();
        }
    }

    public WeaponType GetCurrentWeaponType()
    {
        return currentWeapon;
    }

    void SetupAmmoIcons()
    {
        DeleteChildren(ammoContainer);
        ammoIcons = new List<Image>();

        int iconCount = Mathf.CeilToInt((float)maxAmmo / GetIconReductionFactor());

        for (int i = 0; i < iconCount; i++)
        {
            GameObject icon = Instantiate(ammoIconPrefab, ammoContainer);
            ammoIcons.Add(icon.GetComponent<Image>());
            ammoIcons[i].sprite = fullAmmoSprite;
        }

        int iconReductionFactor = GetIconReductionFactor();
        if (iconReductionFactor > 1)
        {
            ammoExtraText.text = "x" + iconReductionFactor.ToString();
            ammoExtraText.gameObject.SetActive(true);
        }
        else
        {
            ammoExtraText.gameObject.SetActive(false);
        }
    }

    int GetIconReductionFactor()
    {
        if (maxAmmo == 50)
        {
            return 5;
        }
        else if (maxAmmo > 50 && maxAmmo <= 100)
        {
            return 5;
        }
        else if (maxAmmo > 100)
        {
            return 25;
        }
        return 1;
    }



    public void DeleteChildren(Transform container)
    {
        int childCount = container.childCount;

        for (int i = 0; i < childCount; i++)
        {
            Destroy(container.GetChild(i).gameObject);
        }
    }

    public void Shoot()
    {
        if (Time.time - lastShotTime > fireRate && currentAmmo > 0 && !isReloading)
        {
            lastShotTime = Time.time;

            PlayWeaponSound();

            if (currentWeapon == WeaponType.RPG)
            {
                FireRPG();
            }
            else if (currentWeapon == WeaponType.MachineGun)
            {
                FireMachineGun();
                StartCoroutine(PlayFastGunSound(machineGunFireSound));
            }
            else if (currentWeapon == WeaponType.Tomy)
            {
                FireBullet();
                StartCoroutine(PlayFastGunSound(tomyFireSound));
            }
            else if (currentWeapon == WeaponType.Bouncing)
            {
                FireBouncingBullet();
            }
            else if (currentWeapon == WeaponType.Uzi)
            {
                StartCoroutine(ShootUzi());
            }
            else if (currentWeapon == WeaponType.Shotgun)
            {
                ShootShotgun();
            }
            else
            {
                FireBullet();
            }
        }
    }



    private void PlayWeaponSound()
    {
        if (audioSource == null) return;

        switch (currentWeapon)
        {
            case WeaponType.Pistol:
                audioSource.PlayOneShot(pistolFireSound);
                break;
            case WeaponType.Uzi:
                StartCoroutine(PlayUziSound());
                break;
            case WeaponType.Shotgun:
                audioSource.PlayOneShot(shotgunFireSound);
                break;
            case WeaponType.Tomy:
                StartCoroutine(PlayFastGunSound(tomyFireSound));
                break;
            case WeaponType.Bouncing:
                audioSource.PlayOneShot(bouncingFireSound);
                break;
            case WeaponType.RPG:
                audioSource.PlayOneShot(rpgFireSound);
                break;
            case WeaponType.MachineGun:
                StartCoroutine(PlayFastGunSound(machineGunFireSound));
                break;
            default:
                break;
        }
    }

    private IEnumerator PlayFastGunSound(AudioClip gunSound)
    {
        if (!audioSource.isPlaying || audioSource.clip != gunSound)
        {
            audioSource.clip = gunSound;
            audioSource.loop = true;
            audioSource.Play();
        }

        while (Input.GetMouseButton(0))
        {
            yield return null;
        }

        if (audioSource.isPlaying && audioSource.clip == gunSound)
        {
            audioSource.Stop();
        }
    }

    private IEnumerator PlayUziSound()
    {
        for (int i = 0; i < 3; i++)
        {
            audioSource.PlayOneShot(uziFireSound);
            yield return new WaitForSeconds(uziFireRate);
        }
    }

    private void FireMachineGun()
    {
        // Koni içinde rastgele bir açý belirle
        float randomAngle = Random.Range(-firingConeAngle, firingConeAngle);
        Quaternion rotation = Quaternion.Euler(0, 0, firePoint.eulerAngles.z + randomAngle);
        // Mermiyi oluþtur ve yönlendir
        GameObject bullet = Instantiate(ammoPrefab, firePoint.position, rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(rotation * Vector2.up * ammoSpeed, ForceMode2D.Impulse);

        SetAmmo(currentAmmo - 1);

        if (currentAmmo == 0)
        {
            SwitchToPistol();
            currentWeapon = WeaponType.Pistol;
        }

        Destroy(bullet, ammoLifeSpan);
    }

    private void FireRPG()
    {
        GameObject rpg = Instantiate(rpgProjectilePrefab, firePoint.position, firePoint.rotation);
        SetAmmo(currentAmmo - 1);

        if (currentAmmo == 0)
        {
            SwitchToPistol();
            currentWeapon = WeaponType.Pistol;
        }
    }

    private void FireBouncingBullet()
    {
        GameObject bullet = Instantiate(bounceAmmoPrefab, firePoint.position, firePoint.rotation);
        BouncingBullet bouncingBullet = bullet.GetComponent<BouncingBullet>();

        if (bouncingBullet != null)
        {
            bouncingBullet.Initialize(ammoSpeed, ammoLifeSpan, maxBounces, bounceRadius);
        }

        SetAmmo(currentAmmo - 1);

        if (currentAmmo == 0)
        {
            SwitchToPistol();
            currentWeapon = WeaponType.Pistol;
        }
    }

    private IEnumerator ShootUzi()
    {
        for (int i = 0; i < 3; i++)
        {
            if (currentAmmo > 0)
            {
                FireBullet();
                yield return new WaitForSeconds(uziFireRate);
            }
            else
            {
                break;
            }
        }

        if (currentAmmo <= 0)
        {
            SwitchToPistol();
            currentWeapon = WeaponType.Pistol;
        }
    }

    private void FireBullet()
    {
        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, firePoint.up);
        GameObject bullet = Instantiate(ammoPrefab, firePoint.position, rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(firePoint.up * ammoSpeed, ForceMode2D.Impulse);

        if (currentAmmo != 99999) SetAmmo(currentAmmo - 1);
        if (currentAmmo == 0)
        {
            if(currentWeapon != WeaponType.Pistol)
            {
                SwitchToPistol();currentWeapon = WeaponType.Pistol;
            }
            else
                Reload();
        }
        
        Destroy(bullet, ammoLifeSpan);
    }

    private void ShootShotgun()
    {
        float baseAngle = Mathf.Atan2(firePoint.up.y, firePoint.up.x) * Mathf.Rad2Deg;

        for (int i = 0; i < 5; i++)
        {
            float spreadAngle = baseAngle + Random.Range(-shotgunSpreadAngle, shotgunSpreadAngle);
            Quaternion rotation = Quaternion.Euler(0, 0, spreadAngle);
            GameObject bullet = Instantiate(ammoPrefab, firePoint.position, rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.AddForce(rotation * Vector2.right * ammoSpeed, ForceMode2D.Impulse);

            Destroy(bullet, ammoLifeSpan);
        }

        SetAmmo(currentAmmo - 1);

        if (currentAmmo == 0)
        {
            SwitchToPistol();
            currentWeapon = WeaponType.Pistol;
        }
    }

    public void Reload()
    {
        if (isReloading) return;

        if (currentWeapon != WeaponType.Pistol)
        {
            return;
        }

        else
        {
            StartCoroutine(DoReload());
        }

        if (reloadSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }
    }

    public void UpdateAmmoBoard()
    {
        if (isReloading)
        {
            ammoText.GetComponent<TextMeshProUGUI>().text = "Reloading...";
        }
        else
        {
            ammoText.GetComponent<TextMeshProUGUI>().text = currentAmmo >= 99980 ? "Infinite" : currentAmmo + "/" + maxAmmo;

            int iconUpdateFactor = GetIconReductionFactor();
            int activeIcons = Mathf.CeilToInt((float)currentAmmo / iconUpdateFactor);

            for (int i = 0; i < ammoIcons.Count; i++)
            {
                ammoIcons[i].sprite = i < activeIcons ? fullAmmoSprite : emptyAmmoSprite;
            }

            if (iconUpdateFactor > 1)
            {
                ammoExtraText.text = "x" + iconUpdateFactor.ToString();
                ammoExtraText.gameObject.SetActive(true);
            }
            else
            {
                ammoExtraText.gameObject.SetActive(false);
            }
        }
    }

    public void SetAmmo(int ammo)
    {
        currentAmmo = ammo;
        UpdateAmmoBoard();
    }

    private IEnumerator DoReload()
    {
        isReloading = true;

        int missingAmmo = maxAmmo - currentAmmo;
        float reloadInterval = reloadTime / missingAmmo;

        for (int i = 0; i < missingAmmo; i++)
        {
            yield return new WaitForSeconds(reloadInterval);

            if (currentAmmo < maxAmmo)
            {
                currentAmmo++;

                if (currentAmmo - 1 < ammoIcons.Count)
                {
                    ammoIcons[currentAmmo - 1].sprite = fullAmmoSprite;
                }
            }

            UpdateAmmoBoard();
        }

        isReloading = false;
        UpdateAmmoBoard();
    }

    public void SwitchToUzi()
    {
        currentWeapon = WeaponType.Uzi;
        maxAmmo = uziAmmo;
        currentAmmo = maxAmmo;
        ammoSpeed = 15f;
        ammoLifeSpan = 5f;
        reloadTime = 1.3f;
        fireRate = 1f;
        SetupAmmoIcons();
        UpdateAmmoBoard();
    }

    public void SwitchToPistol()
    {
        currentWeapon = WeaponType.Pistol;
        maxAmmo = 12; // Pistol mermi sayýsý
        currentAmmo = maxAmmo;
        ammoSpeed = 15f;
        ammoLifeSpan = 5f;
        reloadTime = 1.3f;
        fireRate = 0.5f; // Pistol atýþ hýzý
        SpriteRenderer gunSpriteRenderer = gun.GetComponent<SpriteRenderer>();
        if (gunSpriteRenderer != null)
        {
            gunSpriteRenderer.sprite = newGunSprite;
        }
        if (pickUpSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(pickUpSound);
        }
        SetupAmmoIcons();
        UpdateAmmoBoard();
    }

    public void SwitchToShotgun()
    {
        currentWeapon = WeaponType.Shotgun;
        maxAmmo = shotgunAmmo;
        currentAmmo = maxAmmo;
        ammoSpeed = 12f;
        ammoLifeSpan = 3f;
        reloadTime = 2f;
        fireRate = 1.2f;
        SetupAmmoIcons();
        UpdateAmmoBoard();
    }

    public void SwitchToTomy()
    {
        currentWeapon = WeaponType.Tomy;
        maxAmmo = machineGunAmmo;
        currentAmmo = maxAmmo;
        ammoSpeed = 20f;
        ammoLifeSpan = 4f;
        reloadTime = 3f;
        fireRate = machineGunFireRate;
        SetupAmmoIcons();
        UpdateAmmoBoard();
    }

    public void SwitchToBouncing()
    {
        currentWeapon = WeaponType.Bouncing;
        maxAmmo = bounceAmmo;
        currentAmmo = maxAmmo;
        ammoSpeed = 20f;
        ammoLifeSpan = 4f;
        reloadTime = 2.5f;
        fireRate = 0.5f; // Daha yavaþ ateþ hýzý
        SetupAmmoIcons();
        UpdateAmmoBoard();
    }

    public void SwitchToRPG()
    {
        currentWeapon = WeaponType.RPG;
        maxAmmo = rpgAmmo;
        currentAmmo = maxAmmo;
        ammoSpeed = 10f; // RPG'nin daha yavaþ hýzý
        ammoLifeSpan = 3f; // RPG'nin mermisinin ömrü
        reloadTime = 3f;
        fireRate = 1f;
        SetupAmmoIcons();
        UpdateAmmoBoard();
    }

    public void SwitchToMachineGun()
    {
        currentWeapon = WeaponType.MachineGun;
        maxAmmo = machineGunAmmo;
        currentAmmo = maxAmmo;
        ammoSpeed = 20f;
        ammoLifeSpan = 4f;
        reloadTime = 3f;
        fireRate = machineGunFireRate;
        SetupAmmoIcons();
        UpdateAmmoBoard();
    }
}