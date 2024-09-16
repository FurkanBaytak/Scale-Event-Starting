using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;

public class ColorCycle : MonoBehaviour
{
    public Light2D light2D;
    public float cycleDuration = 1.0f;

    private float hue = 0f;

    void Start()
    {
        if (light2D == null)
        {
            light2D = GetComponent<Light2D>();
        }
    }

    void Update()
    {
        hue += Time.deltaTime / cycleDuration;
        if (hue > 1f)
        {
            hue -= 1f;
        }

        Color newColor = Color.HSVToRGB(hue, 1f, 1f);
        light2D.color = newColor;
    }
}
