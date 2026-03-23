using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float delay = 0.44f; // 16 frames / 36 fps (approx 0.44s)

    void Start()
    {
        Destroy(gameObject, delay);
    }
}
