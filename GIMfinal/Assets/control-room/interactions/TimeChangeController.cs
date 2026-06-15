using System.Collections;
using UnityEngine;

public class TimeChangeController : MonoBehaviour
{
    [Header("Window Quad Renderers")]
    public Renderer dayQuadRenderer;
    public Renderer eveningQuadRenderer;
    public Renderer nightQuadRenderer;

    [Header("Main Light")]
    public Light directionalLight;

    [Header("Interior Lights")]
    public Light roomLight;
    public Light[] extraInteriorLights;

    [Header("Day Settings")]
    public float daySunIntensity = 1.2f;
    public Color daySunColor = new Color(1f, 0.95f, 0.8f);
    public float dayRoomIntensity = 0.3f;
    public float dayExtraInteriorIntensity = 0.2f;

    [Header("Evening Settings")]
    public float eveningSunIntensity = 0.65f;
    public Color eveningSunColor = new Color(1f, 0.55f, 0.28f);
    public float eveningRoomIntensity = 0.8f;
    public float eveningExtraInteriorIntensity = 0.7f;

    [Header("Night Settings")]
    public float nightSunIntensity = 0.08f;
    public Color nightSunColor = new Color(0.25f, 0.35f, 0.7f);
    public float nightRoomIntensity = 1.6f;
    public float nightExtraInteriorIntensity = 1.4f;

    [Header("Transition Duration")]
    public float dayToEveningDuration = 6f;
    public float eveningToNightDuration = 8f;

    [Header("Debug Test")]
    public bool testWithKeys = true;

    private Material dayMat;
    private Material eveningMat;
    private Material nightMat;

    private Coroutine currentTransition;

    private void Start()
    {
        SetupMaterials();
        SetDayInstant();

        Debug.Log("TimeChangeController ready.");
    }

    private void Update()
    {
        if (!testWithKeys)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetDayInstant();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TransitionToEvening();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TransitionToNight();
        }
    }

    private void SetupMaterials()
    {
        if (dayQuadRenderer != null)
        {
            dayQuadRenderer.gameObject.SetActive(true);
            dayMat = dayQuadRenderer.material;
            dayMat.renderQueue = 3002;
        }

        if (eveningQuadRenderer != null)
        {
            eveningQuadRenderer.gameObject.SetActive(true);
            eveningMat = eveningQuadRenderer.material;
            eveningMat.renderQueue = 3001;
        }

        if (nightQuadRenderer != null)
        {
            nightQuadRenderer.gameObject.SetActive(true);
            nightMat = nightQuadRenderer.material;
            nightMat.renderQueue = 3000;
        }
    }

    public void SetDayInstant()
    {
        if (currentTransition != null)
        {
            StopCoroutine(currentTransition);
            currentTransition = null;
        }

        SetAlpha(dayMat, 1f);
        SetAlpha(eveningMat, 0f);
        SetAlpha(nightMat, 0f);

        ApplyLightSettings(
            daySunIntensity,
            daySunColor,
            dayRoomIntensity,
            dayExtraInteriorIntensity
        );

        Debug.Log("Time set instantly: Day");
    }

    public void TransitionToEvening()
    {
        StartNewTransition(TransitionRoutine(
            dayFrom: GetAlpha(dayMat),
            dayTo: 0f,

            eveningFrom: GetAlpha(eveningMat),
            eveningTo: 1f,

            nightFrom: GetAlpha(nightMat),
            nightTo: 0f,

            sunTo: eveningSunIntensity,
            sunColorTo: eveningSunColor,

            roomTo: eveningRoomIntensity,
            extraTo: eveningExtraInteriorIntensity,

            duration: dayToEveningDuration,
            label: "Evening"
        ));
    }

    public void TransitionToNight()
    {
        StartNewTransition(TransitionRoutine(
            dayFrom: GetAlpha(dayMat),
            dayTo: 0f,

            eveningFrom: GetAlpha(eveningMat),
            eveningTo: 0f,

            nightFrom: GetAlpha(nightMat),
            nightTo: 1f,

            sunTo: nightSunIntensity,
            sunColorTo: nightSunColor,

            roomTo: nightRoomIntensity,
            extraTo: nightExtraInteriorIntensity,

            duration: eveningToNightDuration,
            label: "Night"
        ));
    }

    private void StartNewTransition(IEnumerator routine)
    {
        if (currentTransition != null)
            StopCoroutine(currentTransition);

        currentTransition = StartCoroutine(routine);
    }

    private IEnumerator TransitionRoutine(
        float dayFrom,
        float dayTo,
        float eveningFrom,
        float eveningTo,
        float nightFrom,
        float nightTo,
        float sunTo,
        Color sunColorTo,
        float roomTo,
        float extraTo,
        float duration,
        string label
    )
    {
        float timer = 0f;

        float sunFrom = directionalLight != null ? directionalLight.intensity : sunTo;
        Color sunColorFrom = directionalLight != null ? directionalLight.color : sunColorTo;

        float roomFrom = roomLight != null ? roomLight.intensity : roomTo;
        float extraFrom = GetCurrentExtraInteriorIntensity();

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / duration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            SetAlpha(dayMat, Mathf.Lerp(dayFrom, dayTo, smoothT));
            SetAlpha(eveningMat, Mathf.Lerp(eveningFrom, eveningTo, smoothT));
            SetAlpha(nightMat, Mathf.Lerp(nightFrom, nightTo, smoothT));

            if (directionalLight != null)
            {
                directionalLight.intensity = Mathf.Lerp(sunFrom, sunTo, smoothT);
                directionalLight.color = Color.Lerp(sunColorFrom, sunColorTo, smoothT);
            }

            if (roomLight != null)
            {
                roomLight.intensity = Mathf.Lerp(roomFrom, roomTo, smoothT);
            }

            SetExtraInteriorLights(Mathf.Lerp(extraFrom, extraTo, smoothT));

            yield return null;
        }

        SetAlpha(dayMat, dayTo);
        SetAlpha(eveningMat, eveningTo);
        SetAlpha(nightMat, nightTo);

        ApplyLightSettings(sunTo, sunColorTo, roomTo, extraTo);

        currentTransition = null;

        Debug.Log("Time transition finished: " + label);
    }

    private void ApplyLightSettings(float sunIntensity, Color sunColor, float roomIntensity, float extraIntensity)
    {
        if (directionalLight != null)
        {
            directionalLight.intensity = sunIntensity;
            directionalLight.color = sunColor;
        }

        if (roomLight != null)
        {
            roomLight.intensity = roomIntensity;
        }

        SetExtraInteriorLights(extraIntensity);
    }

    private void SetAlpha(Material mat, float alpha)
    {
        if (mat == null)
            return;

        alpha = Mathf.Clamp01(alpha);

        // Built-in Unlit/Transparent usually uses _Color.
        if (mat.HasProperty("_Color"))
        {
            Color color = mat.GetColor("_Color");
            color.a = alpha;
            mat.SetColor("_Color", color);
        }

        // URP Unlit usually uses _BaseColor.
        if (mat.HasProperty("_BaseColor"))
        {
            Color baseColor = mat.GetColor("_BaseColor");
            baseColor.a = alpha;
            mat.SetColor("_BaseColor", baseColor);
        }
    }

    private float GetAlpha(Material mat)
    {
        if (mat == null)
            return 0f;

        if (mat.HasProperty("_Color"))
            return mat.GetColor("_Color").a;

        if (mat.HasProperty("_BaseColor"))
            return mat.GetColor("_BaseColor").a;

        return 0f;
    }

    private void SetExtraInteriorLights(float intensity)
    {
        if (extraInteriorLights == null)
            return;

        foreach (Light light in extraInteriorLights)
        {
            if (light != null)
                light.intensity = intensity;
        }
    }

    private float GetCurrentExtraInteriorIntensity()
    {
        if (extraInteriorLights == null || extraInteriorLights.Length == 0)
            return 0f;

        foreach (Light light in extraInteriorLights)
        {
            if (light != null)
                return light.intensity;
        }

        return 0f;
    }
}