using UnityEngine;

public class BossIconWobble : MonoBehaviour
{
    private Vector3 initialScale;
    private Vector3 initialPosition;
    private float randomOffset;

    void Start()
    {
        initialScale = transform.localScale;
        initialPosition = transform.localPosition;

        randomOffset = Random.Range(0f, 2f);
    }

    void Update()
    {
        transform.localScale = initialScale + WobbleScale(Time.time + randomOffset);
        transform.localPosition = initialPosition + WobblePosition(Time.time + randomOffset);
    }

    Vector3 WobbleScale(float time)
    {
        float wobbleX = Mathf.Sin(time * 3f) * 1f;
        float wobbleY = Mathf.Cos(time * 2.5f) * 1f;
        return new Vector3(wobbleX, wobbleY, 0);
    }

    Vector3 WobblePosition(float time)
    {
        float wobbleX = Mathf.Sin(time * 1.5f) * 0.9f;
        float wobbleY = Mathf.Cos(time * 2f) * 0.9f;
        return new Vector3(wobbleX, wobbleY, 0);
    }
}
