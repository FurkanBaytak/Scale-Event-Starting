using UnityEngine;

public class IconWobble : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector3 initialScale;
    private float randomOffset;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        initialScale = rectTransform.localScale;

        randomOffset = Random.Range(0f, 2f);
    }

    void Update()
    {
        rectTransform.localScale = initialScale + Wobble(Time.time + randomOffset);
    }

    Vector3 Wobble(float time)
    {
        float wobbleX = Mathf.Sin(time * 3f) * 0.08f;
        float wobbleY = Mathf.Cos(time * 2.5f) * 0.08f;
        return new Vector3(wobbleX, wobbleY, 0);
    }
}
