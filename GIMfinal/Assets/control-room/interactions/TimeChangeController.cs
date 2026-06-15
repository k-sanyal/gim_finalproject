using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class TimeChangeController : MonoBehaviour
{
    [Header("Window Video")]
    public VideoPlayer windowVideoPlayer;

    [Header("Interior Lights")]
    public Light roomLight;
    public Light[] extraInteriorLights;

    [Header("Light Settings")]
    public float dayRoomIntensity = 0.3f;
    public float nightRoomIntensity = 1.6f;

    public float dayExtraInteriorIntensity = 0.2f;
    public float nightExtraInteriorIntensity = 1.4f;

    [Header("Transition")]
    public float lightTransitionDuration = 15f;

    [Header("Debug")]
    public bool testWithVKey = true;

    private Coroutine lightRoutine;
    private bool hasStarted = false;
    private bool isPrepared = false;

    private void Start()
    {
        ApplyInteriorLight(dayRoomIntensity, dayExtraInteriorIntensity);

        if (windowVideoPlayer != null)
        {
            windowVideoPlayer.playOnAwake = false;
            windowVideoPlayer.isLooping = false;
            windowVideoPlayer.waitForFirstFrame = true;

            windowVideoPlayer.Stop();
            windowVideoPlayer.time = 0;

            StartCoroutine(PrepareFirstFrameRoutine());
        }
        else
        {
            Debug.LogWarning("Window Video Player is not assigned.");
        }

        Debug.Log("TimeChangeController ready.");
    }

    private void Update()
    {
        if (!testWithVKey)
            return;

        if (Input.GetKeyDown(KeyCode.V))
        {
            StartTimeChange();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            ResetTimeChange();
        }
    }

    private IEnumerator PrepareFirstFrameRoutine()
    {
        if (windowVideoPlayer == null)
            yield break;

        isPrepared = false;

        windowVideoPlayer.Prepare();

        while (!windowVideoPlayer.isPrepared)
        {
            yield return null;
        }

        // 첫 프레임을 RenderTexture에 실제로 그리기 위해 잠깐 재생
        windowVideoPlayer.time = 0;
        windowVideoPlayer.Play();

        yield return null;
        yield return null;

        windowVideoPlayer.Pause();
        windowVideoPlayer.time = 0;

        isPrepared = true;

        Debug.Log("Window video first frame prepared. Day image is displayed.");
    }

    public void StartTimeChange()
    {
        if (hasStarted)
        {
            Debug.Log("Time change already started.");
            return;
        }

        hasStarted = true;

        if (windowVideoPlayer != null)
        {
            StartCoroutine(PlayTimeVideoRoutine());
        }

        if (lightRoutine != null)
            StopCoroutine(lightRoutine);

        lightRoutine = StartCoroutine(ChangeInteriorLightRoutine());
    }

    private IEnumerator PlayTimeVideoRoutine()
    {
        if (windowVideoPlayer == null)
            yield break;

        if (!isPrepared)
        {
            while (!isPrepared)
                yield return null;
        }

        windowVideoPlayer.Stop();
        windowVideoPlayer.time = 0;

        yield return null;

        windowVideoPlayer.Play();

        Debug.Log("Window time video started.");
    }

    public void ResetTimeChange()
    {
        hasStarted = false;

        if (lightRoutine != null)
        {
            StopCoroutine(lightRoutine);
            lightRoutine = null;
        }

        ApplyInteriorLight(dayRoomIntensity, dayExtraInteriorIntensity);

        if (windowVideoPlayer != null)
        {
            StartCoroutine(PrepareFirstFrameRoutine());
        }

        Debug.Log("Time change reset to day.");
    }

    private IEnumerator ChangeInteriorLightRoutine()
    {
        float timer = 0f;

        float startRoomIntensity = roomLight != null ? roomLight.intensity : dayRoomIntensity;
        float startExtraIntensity = GetCurrentExtraInteriorIntensity();

        while (timer < lightTransitionDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / lightTransitionDuration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            float roomIntensity = Mathf.Lerp(startRoomIntensity, nightRoomIntensity, smoothT);
            float extraIntensity = Mathf.Lerp(startExtraIntensity, nightExtraInteriorIntensity, smoothT);

            ApplyInteriorLight(roomIntensity, extraIntensity);

            yield return null;
        }

        ApplyInteriorLight(nightRoomIntensity, nightExtraInteriorIntensity);
        lightRoutine = null;

        Debug.Log("Interior light transition finished.");
    }

    private void ApplyInteriorLight(float roomIntensity, float extraIntensity)
    {
        if (roomLight != null)
            roomLight.intensity = roomIntensity;

        if (extraInteriorLights == null)
            return;

        foreach (Light light in extraInteriorLights)
        {
            if (light != null)
                light.intensity = extraIntensity;
        }
    }

    private float GetCurrentExtraInteriorIntensity()
    {
        if (extraInteriorLights == null || extraInteriorLights.Length == 0)
            return dayExtraInteriorIntensity;

        foreach (Light light in extraInteriorLights)
        {
            if (light != null)
                return light.intensity;
        }

        return dayExtraInteriorIntensity;
    }
}