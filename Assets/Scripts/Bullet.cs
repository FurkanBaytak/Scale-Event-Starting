using UnityEngine;

public class Bullet : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "enemy" || collision.CompareTag("Boss") || collision.CompareTag("MiniBoss"))
        {
            if (collision.CompareTag("MiniBoss"))
            {
                collision.GetComponent<MiniBoss>().TakeDamage(1);
            }
            else if(collision.CompareTag("Boss"))
            {
                collision.GetComponent<LaserBoss>().TakeDamage(1);
            }
            else if (collision.tag == "enemy")
            {
                collision.gameObject.GetComponent<BaseEnemy>().TakeDamage(1);
            }

            Destroy(gameObject);
        }
        
    }

}
