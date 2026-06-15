using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SignalObject : MonoBehaviour
{
    public bool isWow = false;
    private Image img;

    public void Init(bool wow)
    {
        img = GetComponent<Image>();
        img.color = wow ?
            new Color(1f, 0.4f, 0f) :
            new Color(0f, 0.9f, 0.4f);

        float size = wow ? 24f : 14f;
        GetComponent<RectTransform>().sizeDelta = new Vector2(size, size);
        StartCoroutine(Pulse());

        if(!wow) StartCoroutine(FadeOut(Random.Range(15f, 25f))); // increased from 6-12
    }

    IEnumerator Pulse()
    {
        while(true)
        {
            float t = Mathf.PingPong(Time.time * (isWow ? 3f : 1.5f), 1f);
            img.color = new Color(
                img.color.r, img.color.g, img.color.b,
                Mathf.Lerp(0.4f, 1f, t)
            );
            yield return null;
        }
    }

    IEnumerator FadeOut(float delay)
    {
        yield return new WaitForSeconds(delay);
        // increase fade time
        float t = 0f;
        Color c = img.color;
        while(t < 1f)
        {
            t += Time.deltaTime * 0.3f; // slower fade
            img.color = new Color(c.r, c.g, c.b, Mathf.Lerp(1f, 0f, t));
            yield return null;
        }
        Destroy(gameObject);
    }
}