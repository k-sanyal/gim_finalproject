using System.Collections;
using UnityEngine;
using TMPro;

public class SignalSequenceController : MonoBehaviour
{
    [Header("Window Time Views")]
    public GameObject morningView;
    public GameObject eveningView;
    public GameObject nightView;

    [Header("Lights")]
    public Light directionalLight;
    public Light roomLight;

    [Header("Status UI")]
    public GameObject statusPanel;
    public TMP_Text statusText;

    [Header("Audio Sources")]
    public AudioSource signalAudioSource;
    public AudioSource printerAudioSource;

    [Header("Audio Clips")]
    public AudioClip weakSignalClip;
    public AudioClip noiseClip;
    public AudioClip strongSignalClip;
    public AudioClip printerClip;

    [Header("Final Event")]
    public GameObject wowSignalPaper;

    [Header("Monitor Starter")]
    public MonitorSignalStarter monitorSignalStarter;

    [Header("Timing")]
    public float weakSignalTime = 5f;
    public float eveningTime = 10f;
    public float signalLostTime = 15f;
    public float nightTime = 22f;
    public float strongSignalTime = 28f;
    public float printerTime = 32f;

    private bool sequenceStarted = false;

    private void Start()
    {
        SetMorning();

        if (statusPanel != null)
            statusPanel.SetActive(false);

        if (wowSignalPaper != null)
            wowSignalPaper.SetActive(false);

        Debug.Log("SignalSequenceController ready.");
    }

    public void StartSequence()
    {
        if (sequenceStarted)
        {
            Debug.Log("Signal sequence already started.");
            return;
        }

        sequenceStarted = true;
        StartCoroutine(SignalRoutine());

        Debug.Log("Signal sequence coroutine started.");
    }

    private IEnumerator SignalRoutine()
    {
        ShowStatus("SIGNAL MONITORING...");
        SetMorning();

        yield return new WaitForSeconds(weakSignalTime);
        ShowStatus("WEAK ANOMALY DETECTED");
        PlaySignalSound(weakSignalClip);

        yield return new WaitForSeconds(Mathf.Max(0f, eveningTime - weakSignalTime));
        SetEvening();
        ShowStatus("SIGNAL FLUCTUATION DETECTED");
        PlaySignalSound(noiseClip);

        yield return new WaitForSeconds(Mathf.Max(0f, signalLostTime - eveningTime));
        ShowStatus("SIGNAL LOST");

        yield return new WaitForSeconds(Mathf.Max(0f, nightTime - signalLostTime));
        SetNight();
        ShowStatus("SCANNING CONTINUES...");

        yield return new WaitForSeconds(Mathf.Max(0f, strongSignalTime - nightTime));
        ShowStatus("STRONG NARROWBAND SIGNAL DETECTED");
        PlaySignalSound(strongSignalClip);

        yield return new WaitForSeconds(Mathf.Max(0f, printerTime - strongSignalTime));
        ShowStatus("CHECK THE PRINTER");

        if (monitorSignalStarter != null)
        {
            monitorSignalStarter.StopSignalVideo();
        }
        else
        {
            Debug.LogWarning("Monitor Signal Starter is not assigned.");
        }

        if (printerAudioSource != null && printerClip != null)
        {
            printerAudioSource.PlayOneShot(printerClip);
        }

        if (wowSignalPaper != null)
        {
            wowSignalPaper.SetActive(true);
        }

        Debug.Log("Signal sequence finished. Wow Signal paper appeared.");
    }

    private void ShowStatus(string message)
    {
        if (statusPanel != null)
            statusPanel.SetActive(true);

        if (statusText != null)
            statusText.text = message;

        Debug.Log("Signal Status: " + message);
    }

    private void PlaySignalSound(AudioClip clip)
    {
        if (signalAudioSource != null && clip != null)
        {
            signalAudioSource.PlayOneShot(clip);
        }
    }

    private void SetMorning()
    {
        if (morningView != null)
            morningView.SetActive(true);

        if (eveningView != null)
            eveningView.SetActive(false);

        if (nightView != null)
            nightView.SetActive(false);

        if (directionalLight != null)
        {
            directionalLight.intensity = 1.2f;
            directionalLight.color = new Color(1f, 0.95f, 0.8f);
        }

        if (roomLight != null)
            roomLight.intensity = 1f;

        Debug.Log("Time changed: Morning");
    }

    private void SetEvening()
    {
        if (morningView != null)
            morningView.SetActive(false);

        if (eveningView != null)
            eveningView.SetActive(true);

        if (nightView != null)
            nightView.SetActive(false);

        if (directionalLight != null)
        {
            directionalLight.intensity = 0.65f;
            directionalLight.color = new Color(1f, 0.55f, 0.28f);
        }

        if (roomLight != null)
            roomLight.intensity = 0.55f;

        Debug.Log("Time changed: Evening");
    }

    private void SetNight()
    {
        if (morningView != null)
            morningView.SetActive(false);

        if (eveningView != null)
            eveningView.SetActive(false);

        if (nightView != null)
            nightView.SetActive(true);

        if (directionalLight != null)
        {
            directionalLight.intensity = 0.15f;
            directionalLight.color = new Color(0.25f, 0.35f, 0.7f);
        }

        if (roomLight != null)
            roomLight.intensity = 0.25f;

        Debug.Log("Time changed: Night");
    }
}