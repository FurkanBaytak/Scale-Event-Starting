using System.Collections;
using UnityEngine;

public class SpriteSwitcher : MonoBehaviour
{
    public Sprite sprite1;
    public Sprite sprite2;

    private SpriteRenderer spriteRenderer;

    private bool isUsingSprite1 = true;

    private Coroutine spriteSwitchCoroutine;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        spriteSwitchCoroutine = StartCoroutine(SwitchSprite());
    }

    void OnDisable()
    {
        if (spriteSwitchCoroutine != null)
        {
            StopCoroutine(spriteSwitchCoroutine);
        }
    }

    IEnumerator SwitchSprite()
    {
        while (true)
        {
            if (isUsingSprite1)
            {
                spriteRenderer.sprite = sprite1;
            }
            else
            {
                spriteRenderer.sprite = sprite2;
            }

            isUsingSprite1 = !isUsingSprite1;

            yield return new WaitForSeconds(1f);
        }
    }
}
