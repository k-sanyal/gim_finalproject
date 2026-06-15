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
    private Coroutine prepareRoutine;
    private Coroutine videoRoutine;

    private bool hasStarted = false;
    private bool isPreparingFirstFrame = false;

    private void Awake()
    {
        if (windowVideoPlayer == null)
            windowVideoPlayer = GetComponent<VideoPlayer>();
    }

    private void Start()
    {
        ResetTimeChange();
        Debug.Log("TimeChangeController ready.");
    }

    private void Update()
    {
        if (!testWithVKey)
            return;

        if (Input.GetKeyDown(KeyCode.V))
        {
            Debug.Log("V key pressed.");
            StartTimeChange();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("B key pressed.");
            ResetTimeChange();
        }
    }

    public void ResetTimeChange()
    {
        hasStarted = false;

        if (lightRoutine != null)
        {
            StopCoroutine(lightRoutine);
            lightRoutine = null;
        }

        if (videoRoutine != null)
        {
            StopCoroutine(videoRoutine);
            videoRoutine = null;
        }

        if (prepareRoutine != null)
        {
            StopCoroutine(prepareRoutine);
            prepareRoutine = null;
        }

        ApplyInteriorLight(dayRoomIntensity, dayExtraInteriorIntensity);

        if (windowVideoPlayer == null)
        {
            Debug.LogWarning("Window Video Player is not assigned.");
            return;
        }

        windowVideoPlayer.playOnAwake = false;
        windowVideoPlayer.isLooping = false;
        windowVideoPlayer.waitForFirstFrame = true;

        windowVideoPlayer.Stop();
        windowVideoPlayer.time = 0;

        prepareRoutine = StartCoroutine(PrepareFirstFrameRoutine());

        Debug.Log("Time change reset to day.");
    }

    private IEnumerator PrepareFirstFrameRoutine()
    {
        if (windowVideoPlayer == null)
            yield break;

        isPreparingFirstFrame = true;

        windowVideoPlayer.Prepare();

        while (!windowVideoPlayer.isPrepared)
        {
            yield return null;
        }

        // 여기서 이미 E가 눌려서 시간 변화가 시작됐다면,
        // 첫 프레임 준비가 나중에 와서 영상을 Pause시키면 안 됨.
        if (hasStarted)
        {
            isPreparingFirstFrame = false;
            prepareRoutine = null;
            Debug.Log("First frame prepare skipped because time change already started.");
            yield break;
        }

        windowVideoPlayer.time = 0;
        windowVideoPlayer.Play();

        yield return null;
        yield return null;

        if (!hasStarted)
        {
            windowVideoPlayer.Pause();
            windowVideoPlayer.time = 0;
            Debug.Log("Window video first frame prepared. Day image is displayed.");
        }

        isPreparingFirstFrame = false;
        prepareRoutine = null;
    }

    public void StartTimeChange()
    {
        Debug.Log("StartTimeChange received by: " + gameObject.name);

        if (hasStarted)
        {
            Debug.Log("Time change already started.");
            return;
        }

        hasStarted = true;

        // 핵심: 첫 프레임 준비 코루틴이 아직 돌고 있으면 끊어야 함.
        // 안 그러면 PrepareFirstFrameRoutine이 나중에 Pause를 걸 수 있음.
        if (prepareRoutine != null)
        {
            StopCoroutine(prepareRoutine);
            prepareRoutine = null;
            isPreparingFirstFrame = false;
            Debug.Log("First frame prepare routine stopped because time change started.");
        }

        if (videoRoutine != null)
        {
            StopCoroutine(videoRoutine);
            videoRoutine = null;
        }

        if (windowVideoPlayer != null)
        {
            videoRoutine = StartCoroutine(PlayWindowVideoRoutine());
        }
        else
        {
            Debug.LogWarning("Window Video Player is not assigned.");
        }

        if (lightRoutine != null)
            StopCoroutine(lightRoutine);

        lightRoutine = StartCoroutine(ChangeInteriorLightRoutine());

        Debug.Log("StartTimeChange called.");
    }

    private IEnumerator PlayWindowVideoRoutine()
    {
        if (windowVideoPlayer == null)
            yield break;

        windowVideoPlayer.Stop();

        yield return null;

        windowVideoPlayer.time = 0;
        windowVideoPlayer.Prepare();

        while (!windowVideoPlayer.isPrepared)
        {
            yield return null;
        }

        windowVideoPlayer.time = 0;
        windowVideoPlayer.Play();

        yield return new WaitForSeconds(0.2f);

        Debug.Log(
            "Window time video started. isPlaying: " +
            windowVideoPlayer.isPlaying +
            " / time: " +
            windowVideoPlayer.time +
            " / frame: " +
            windowVideoPlayer.frame
        );

        videoRoutine = null;
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