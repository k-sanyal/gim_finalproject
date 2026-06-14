using UnityEngine;
using TMPro;
using System.Collections;

public class PlanetTextDisplay : MonoBehaviour
{
    public TMP_Text displayText;
    public CanvasGroup canvasGroup;
    public float fadeTime = 1f;
    public float holdTime = 2.5f;

    public void ShowText(string text)
    {
        StopAllCoroutines();
        StartCoroutine(FadeText(text));
    }

    IEnumerator FadeText(string text)
    {
        // fade out first if something already showing
        if(canvasGroup.alpha > 0f)
        {
            float t = 0f;
            while(t < 1f)
            {
                t += Time.deltaTime / (fadeTime * 0.5f);
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
                yield return null;
            }
        }

        displayText.text = text;

        // fade in
        float t2 = 0f;
        while(t2 < 1f)
        {
            t2 += Time.deltaTime / fadeTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t2);
            yield return null;
        }

        yield return new WaitForSeconds(holdTime);

        // fade out
        float t3 = 0f;
        while(t3 < 1f)
        {
            t3 += Time.deltaTime / fadeTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t3);
            yield return null;
        }
    }
}