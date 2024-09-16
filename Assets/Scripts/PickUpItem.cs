using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    public Sprite newGunSprite; // Yerdeki e�yan�n sprite'�n� buraya atayacaks�n�z
    public GameObject gunObject; // Sahnedeki gun object'i buraya atayacaks�n�z
    public WeaponBase weapon;
    public AudioClip pickUpSound;

    private AudioSource audioSource;

    private void Awake()
    {
        gunObject = GameObject.FindGameObjectWithTag("Gun");
        weapon = GameObject.FindGameObjectWithTag("Gun").GetComponent<WeaponBase>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player") // Oyuncu ile �arp��may� kontrol ediyoruz
        {
            if (gunObject != null && newGunSprite != null)
            {
                Debug.Log("Weapon picked up");

                // Gun object'in sprite'�n� de�i�tiriyoruz
                SpriteRenderer gunSpriteRenderer = gunObject.GetComponent<SpriteRenderer>();
                if (gunSpriteRenderer != null)
                {
                    gunSpriteRenderer.sprite = newGunSprite;
                    if(gunSpriteRenderer.sprite.name == "weapon_1") weapon.SwitchToUzi();
                    if(gunSpriteRenderer.sprite.name == "weapon_2") weapon.SwitchToShotgun();
                    if (gunSpriteRenderer.sprite.name == "weapon_3") weapon.SwitchToTomy();
                    if (gunSpriteRenderer.sprite.name == "weapon_4") weapon.SwitchToBouncing();
                    if (gunSpriteRenderer.sprite.name == "weapon_5") weapon.SwitchToRPG();
                    if (gunSpriteRenderer.sprite.name == "weapon_6") weapon.SwitchToMachineGun();
                }

                if (pickUpSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(pickUpSound);
                }

                // Yerdeki e�yay� yok ediyoruz
                Destroy(gameObject, 0.2f);
            }
        }
    }
}
