using UnityEngine;

public class LaserWall : MonoBehaviour
{
    public float moveSpeed = 2f; // Duvarýn hareket hýzý
    public float duration = 10f; // Duvarýn ne kadar süre boyunca aktif olacaðý

    private float elapsedTime = 0f;

    void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        // Belirtilen süre sonunda duvarý yok et
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= duration)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Oyuncuya hasar ver (örnek olarak)
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.TakeDamage(1);
            }
        }
    }
}
