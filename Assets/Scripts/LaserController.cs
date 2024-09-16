using UnityEngine;

public class LaserController : MonoBehaviour
{
    public Animator laserAnimator;
    public float scaleSpeed = 2f;
    public int laserDamage = 1; // Laser'in vereceði hasar
    public LayerMask playerLayer; // Player layer'ý kontrol etmek için
    public Transform targetPosition; // Lazerin hareket edeceði hedef pozisyon
    private bool isScaling = false;
    private bool isLaserActive = false;
    private bool isLaserEnding = false;
    private float currentTime = 0f;
    private float laserDuration; // Laser'in ne kadar süre ikinci aþamada kalacaðý
    private BoxCollider2D laserCollider;

    void Start()
    {
        isScaling = true;
        laserCollider = gameObject.GetComponent<BoxCollider2D>();
        laserCollider.isTrigger = true;
        laserCollider.enabled = false; // Baþlangýçta collider kapalý
    }

    void Update()
    {
        if (isScaling)
        {
            // Lazer objesinin x skalasýný artýr
            transform.localScale = new Vector3(transform.localScale.x + scaleSpeed * Time.deltaTime, transform.localScale.y, transform.localScale.z);

            if (transform.localScale.x >= 7f) // Ýstediðin bir x skala deðerine ulaþtýðýnda
            {
                // Ýkinci aþama animasyonu tetikle
                laserAnimator.SetTrigger("LaserHit");
                isScaling = false;
                isLaserActive = true;
                laserCollider.enabled = true;
            }
        }

        if (isLaserActive)
        {
            // Lazer hareketi
            if (targetPosition != null)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, scaleSpeed * Time.deltaTime);
            }

            // Laser'in aktif kalma süresi
            currentTime += Time.deltaTime;
            if (currentTime >= laserDuration)
            {
                // Son aþama animasyonunu tetikle
                laserAnimator.SetTrigger("LaserEnd");
                isLaserActive = false;
                isLaserEnding = true;
                 // Collider'ý kapat
            }
        }

        if (isLaserEnding)
        {
            // Son animasyon tamamlandýðýnda objeyi yok et
            if (laserAnimator.GetCurrentAnimatorStateInfo(0).IsName("LaserEnd") &&
                laserAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Player'a hasar ver
            PlayerMovement playerHealth = other.GetComponent<PlayerMovement>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(laserDamage);
            }
        }
    }

    // Lazerin süresini ve hedef pozisyonunu ayarlamak için bir fonksiyon ekle
    public void ActivateLaser(float duration, Transform target = null)
    {
        laserDuration = duration;
        targetPosition = target;
    }
}
