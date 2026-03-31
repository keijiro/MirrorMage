using UnityEngine;

public class PulseEffect : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    public float speed = 1.0f;
    public float minIntensity = 0.85f;
    public float maxIntensity = 1.05f;
    public Color baseColor = Color.white;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (_spriteRenderer == null) return;

        // Use Perlin noise for organic, flame-like flickering
        // PerlinNoise returns a value between 0.0 and 1.0
        float noise = Mathf.PerlinNoise(Time.time * speed, 0.5f);
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
        
        _spriteRenderer.color = baseColor * intensity;
    }
}
