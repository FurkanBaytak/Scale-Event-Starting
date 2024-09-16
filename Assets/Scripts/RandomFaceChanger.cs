using UnityEngine;

public class RandomFaceChanger: MonoBehaviour
{
    public Sprite[] faceSprites;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        InvokeRepeating("ChangeFace", 0f, 10f);
    }

    private void ChangeFace()
    {
        if (faceSprites.Length == 0)
        {
            Debug.LogWarning("Face sprite list is empty!");
            return;
        }

        int randomIndex = Random.Range(0, faceSprites.Length);
        spriteRenderer.sprite = faceSprites[randomIndex];
    }
}
