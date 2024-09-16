using UnityEngine;

public class LaserWall : MonoBehaviour
{
    public float moveSpeed = 2f; // Duvar�n hareket h�z�
    public float duration = 10f; // Duvar�n ne kadar s�re boyunca aktif olaca��

    private float elapsedTime = 0f;

    void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        // Belirtilen s�re sonunda duvar� yok et
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
            // Oyuncuya hasar ver (�rnek olarak)
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.TakeDamage(1);
            }
        }
    }
}
