using System.Collections;
using UnityEngine;
using TMPro;

public class TextAnimation : MonoBehaviour
{
    public Vector3 endPosition;
    private Vector3 startPosition;
    public float speed = 1.0f;

    private RectTransform rectTransform;
    private TMP_Text textMesh;
    private RainbowText rainbowText;

    private float startFontSize = 0f;
    private float endFontSize = 40f;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        textMesh = GetComponent<TMP_Text>();
        rainbowText = GetComponent<RainbowText>();

        endPosition = rectTransform.anchoredPosition3D;
        startPosition = new Vector3(endPosition.x, endPosition.y + 4, endPosition.z);
    }

    public void InitializeText()
    {
        rectTransform.anchoredPosition3D = startPosition;
        textMesh.fontSize = startFontSize;

        if (rainbowText != null) rainbowText.enabled = true;
    }

    public void StartTextAnimation()
    {
        if (rainbowText != null) rainbowText.enabled = true;
        StartCoroutine(AnimateText());
    }

    private IEnumerator AnimateText()
    {
        while (Vector3.Distance(rectTransform.anchoredPosition3D, endPosition) > 0.1f || Mathf.Abs(textMesh.fontSize - endFontSize) > 0.1f)
        {
            rectTransform.anchoredPosition3D = Vector3.Lerp(rectTransform.anchoredPosition3D, endPosition, Time.deltaTime * speed);
            textMesh.fontSize = Mathf.Lerp(textMesh.fontSize, endFontSize, Time.deltaTime * speed);

            yield return null;
        }

        rectTransform.anchoredPosition3D = endPosition;
        textMesh.fontSize = endFontSize;
    }

    public IEnumerator ReverseTextAnimation()
    {
        while (Vector3.Distance(rectTransform.anchoredPosition3D, startPosition) > 0.1f || Mathf.Abs(textMesh.fontSize - startFontSize) > 0.1f)
        {
            rectTransform.anchoredPosition3D = Vector3.Lerp(rectTransform.anchoredPosition3D, startPosition, Time.deltaTime * speed);
            textMesh.fontSize = Mathf.Lerp(textMesh.fontSize, startFontSize, Time.deltaTime * speed);

            yield return null;
        }

        rectTransform.anchoredPosition3D = startPosition;
        textMesh.fontSize = startFontSize;
    }
}
