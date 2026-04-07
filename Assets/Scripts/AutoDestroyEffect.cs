using UnityEngine;

public class AutoDestroyEffect : MonoBehaviour
{
    public float delay = 1.0f;

    void Start()
    {
        Destroy(gameObject, delay);
    }
}