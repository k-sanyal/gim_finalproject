using UnityEngine;
using System.Collections;

public class SkyboxTransition : MonoBehaviour
{
    public Material daySkybox;
    public Material spaceSkybox;
    public float duration = 4f;

    void Start()
    {
        // set day skybox immediately at scene start
        RenderSettings.skybox = daySkybox;
        DynamicGI.UpdateEnvironment();
    }

    public void StartTransition()
    {
        StartCoroutine(Blend());
    }

    IEnumerator Blend()
    {
        // create instance so we don't modify the original asset
        Material blendMat = new Material(daySkybox);
        RenderSettings.skybox = blendMat;

        float t = 0f;
        while(t < 1f)
        {
            t += Time.deltaTime / duration;
            blendMat.Lerp(daySkybox, spaceSkybox, t);
            DynamicGI.UpdateEnvironment();
            yield return null;
        }

        RenderSettings.skybox = spaceSkybox;
        DynamicGI.UpdateEnvironment();
    }
}