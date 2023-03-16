using UnityEngine;

public class AnimateEllipse : MonoBehaviour
{
    public float noiseScale = 0.1f;
    public float noiseSpeed = 1f;

    private Vector3 initialScale;

    void Start()
    {
        initialScale = transform.localScale;
    }

    void Update()
    {
        float noise = (Mathf.PerlinNoise(Time.time * noiseSpeed, 
            transform.position.y * noiseScale) - 0.5f) * 2f;
        transform.localScale = initialScale * (1f + noise);
    }
}
