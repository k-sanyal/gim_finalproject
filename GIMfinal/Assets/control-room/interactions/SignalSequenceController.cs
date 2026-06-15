using System.Collections;
using UnityEngine;
using TMPro;

public class SignalSequenceController : MonoBehaviour
{
    [Header("Minigame")] // for minigame completion check, if needed
    public MinigameManager minigameManager;

    [Header("Time Change")]
    public TimeChangeController timeChangeController;

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

    [Header("Printer Animation")]
    public PrinterPaperAnimator printerPaperAnimator;

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
        if (timeChangeController != null)
        {
            timeChangeController.ResetTimeChange();
        }
        else
        {
            Debug.LogWarning("Time Change Controller is not assigned.");
        }

        if (statusPanel != null)
            statusPanel.SetActive(false);

        if (wowSignalPaper != null)
            wowSignalPaper.SetActive(false);

        Debug.Log("SignalSequenceController ready.");
    }

    public void StartWowFoundSequence()
    {
        StartCoroutine(WowFoundRoutine());
    }

    IEnumerator WowFoundRoutine()
    {
        ShowStatus("STRONG NARROWBAND SIGNAL DETECTED");
        PlaySignalSound(strongSignalClip);

        yield return new WaitForSeconds(4f);

        ShowStatus("SIGNAL LOCKED — 1420 MHz");

        yield return new WaitForSeconds(2f);

        if (statusPanel != null)
            statusPanel.SetActive(false);

        if (monitorSignalStarter != null)
            monitorSignalStarter.StopSignalVideo();

        if (printerAudioSource != null && printerClip != null)
            printerAudioSource.PlayOneShot(printerClip);

        if (printerPaperAnimator != null)
            printerPaperAnimator.StartPrint();
        else if (wowSignalPaper != null)
            wowSignalPaper.SetActive(true);
    }


    public void StartSequence()
    {
        if (sequenceStarted) return;
        sequenceStarted = true;

        if (timeChangeController != null)
            timeChangeController.StartTimeChange();

        ShowStatus("SIGNAL MONITORING...");
        StartCoroutine(DelayedMinigameStart());
    }

    IEnumerator DelayedMinigameStart()
    {
        // show signal monitoring for 8 seconds first
        yield return new WaitForSeconds(3f);
        ShowStatus("WEAK ANOMALY DETECTED");
        PlaySignalSound(weakSignalClip);

        yield return new WaitForSeconds(3f);
        ShowStatus("SIGNAL FLUCTUATION DETECTED");
        PlaySignalSound(noiseClip);

        yield return new WaitForSeconds(2f);
        ShowStatus("SCANNING...");

        yield return new WaitForSeconds(2f);

        // hide status then start minigame
        if (statusPanel != null)
            statusPanel.SetActive(false);

        if (minigameManager != null)
            minigameManager.ShowMinigame();
        else
            StartCoroutine(SignalRoutine());
    }


    // public void StartSequence() OLD
    // {
    //     if (sequenceStarted)
    //     {
    //         Debug.Log("Signal sequence already started.");
    //         return;
    //     }

    //     sequenceStarted = true;
    //     StartCoroutine(SignalRoutine());

    //     Debug.Log("Signal sequence coroutine started.");
    // }

    private IEnumerator SignalRoutine()
    {
        ShowStatus("SIGNAL MONITORING...");

        if (timeChangeController != null)
        {
            timeChangeController.StartTimeChange();
            Debug.Log("Time change started from SignalSequenceController.");
        }
        else
        {
            Debug.LogWarning("Time Change Controller is not assigned.");
        }

        yield return new WaitForSeconds(weakSignalTime);
        ShowStatus("WEAK ANOMALY DETECTED");
        PlaySignalSound(weakSignalClip);

        yield return new WaitForSeconds(Mathf.Max(0f, eveningTime - weakSignalTime));
        ShowStatus("SIGNAL FLUCTUATION DETECTED");
        PlaySignalSound(noiseClip);

        yield return new WaitForSeconds(Mathf.Max(0f, signalLostTime - eveningTime));
        ShowStatus("SIGNAL LOST");

        yield return new WaitForSeconds(Mathf.Max(0f, nightTime - signalLostTime));
        ShowStatus("SCANNING CONTINUES");

        yield return new WaitForSeconds(Mathf.Max(0f, strongSignalTime - nightTime));
        ShowStatus("STRONG NARROWBAND SIGNAL DETECTED");
        PlaySignalSound(strongSignalClip);

        yield return new WaitForSeconds(Mathf.Max(0f, printerTime - strongSignalTime));
        ShowStatus("Press P to check the Paper");

        yield return new WaitForSeconds(1.5f);

        if (statusPanel != null)
            statusPanel.SetActive(false);

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

        if (printerPaperAnimator != null)
        {
            printerPaperAnimator.StartPrint();
        }
        else
        {
            Debug.LogWarning("Printer Paper Animator is not assigned. Showing paper directly.");

            if (wowSignalPaper != null)
                wowSignalPaper.SetActive(true);
        }

        Debug.Log("Signal sequence finished. Printer event started.");
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
            signalAudioSource.PlayOneShot(clip);
    }
}