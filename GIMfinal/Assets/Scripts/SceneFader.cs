using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    public Image blackPanel;
    public float fadeDuration = 1.5f;
    public SceneFader fader;

    void Start()
    {
        // always fade in from black on scene start
        StartCoroutine(FadeIn());
    }

    

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            fader.FadeToScene("ConferenceRoom");
    }

    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeOutToScene(sceneName));
    }

    public void FadeToBlackOnly()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeIn()
    {
        blackPanel.color = new Color(0, 0, 0, 1f);
        float t = 0f;
        while(t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            blackPanel.color = new Color(0, 0, 0, Mathf.Lerp(1f, 0f, t));
            yield return null;
        }
        blackPanel.color = new Color(0, 0, 0, 0f);
    }

    IEnumerator FadeOut()
    {
        float t = 0f;
        while(t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            blackPanel.color = new Color(0, 0, 0, Mathf.Lerp(0f, 1f, t));
            yield return null;
        }
        blackPanel.color = new Color(0, 0, 0, 1f);
    }

    IEnumerator FadeOutToScene(string sceneName)
    {
        yield return StartCoroutine(FadeOut());
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene(sceneName);
    }
}