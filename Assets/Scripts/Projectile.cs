using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 5f;
    public bool isReflected = false;
    public Vector2 direction;
    public Sprite reflectedSprite;

    private SpriteRenderer _sr;

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);

        // Simple cleanup if out of bounds
        if (Vector2.Distance(transform.position, Vector2.zero) > 50f)
        {
            Destroy(gameObject);
        }
    }

    public void Reflect(Vector2 normal)
    {
        if (isReflected) return;

        isReflected = true;
        
        // Pure geometric reflection off the surface normal
        direction = Vector2.Reflect(direction, normal).normalized;
        
        speed *= 2f;
        
        // Visual feedback: swap sprite and reset color to white
        if (reflectedSprite != null && _sr != null)
        {
            _sr.sprite = reflectedSprite;
            _sr.color = Color.white;
        }
        else if (_sr != null)
        {
            _sr.color = Color.yellow; // Fallback
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isReflected)
        {
            if (other.CompareTag("Enemy"))
            {
                Enemy enemy = other.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.Die();
                }
                else
                {
                    Destroy(other.gameObject);
                }
                Destroy(gameObject);
            }
        }
        else
        {
            if (other.CompareTag("Player"))
            {
                // Player handling is done in PlayerController or here
                Destroy(gameObject);
            }
        }
    }
}
