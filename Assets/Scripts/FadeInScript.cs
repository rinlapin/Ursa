
using UnityEngine;
using System.Collections;

public class FadeIn : MonoBehaviour
{
    public CanvasGroup fadePanel;
    public float fadeDuration = 2f;

    void Start()
    {
        fadePanel.alpha = 1f;
        StartCoroutine(FadeFromBlack());
    }

    IEnumerator FadeFromBlack()
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadePanel.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }

        fadePanel.alpha = 0f;
        fadePanel.gameObject.SetActive(false);
    }
}
