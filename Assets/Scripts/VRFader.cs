using UnityEngine;
using System.Collections;

public class VRFader : MonoBehaviour
{
    public float fadeDuration = 1f;
    private Material fadeMaterial;

    void Awake()
    {
        fadeMaterial = GetComponent<Renderer>().material;
        SetAlpha(0f);
    }

    public IEnumerator FadeOut()
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            SetAlpha(timer / fadeDuration);
            yield return null;
        }

        SetAlpha(1f);
    }

    public IEnumerator FadeIn()
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            SetAlpha(1f - (timer / fadeDuration));
            yield return null;
        }

        SetAlpha(0f);
    }

    void SetAlpha(float alpha)
    {
        Color color = fadeMaterial.color;
        color.a = alpha;
        fadeMaterial.color = color;
    }
}
