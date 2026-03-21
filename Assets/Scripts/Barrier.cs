using UnityEngine;

public class Barrier : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Projectile"))
        {
            Projectile p = other.GetComponent<Projectile>();
            if (p != null && !p.isReflected)
            {
                // Calculate normal based on the vector from barrier center to projectile
                Vector2 normal = (other.transform.position - transform.position).normalized;
                p.Reflect(normal);
            }
        }
    }
}
