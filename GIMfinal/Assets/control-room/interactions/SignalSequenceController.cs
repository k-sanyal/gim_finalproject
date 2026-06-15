using System.Collections;
using UnityEngine;
using TMPro;

public class SignalSequenceController : MonoBehaviour
{
    [Header("Minigame")]
    public MinigameManager minigameManager;

    [Header("Camera")]
    public Camera playerCamera;
    public Camera monitorCamera;
    public PlayerController playerLookScript;

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
    private bool wowFoundSequenceStarted = false;
    private bool timeChangeStarted = false;

    private void Start()
    {
        if (timeChangeController != null)
        {
            timeChangeController.ResetTimeChange();
            Debug.Log("TimeChangeController reset from SignalSequenceController.");
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
public void StartSequence()
{
    if (sequenceStarted)
    {
        Debug.Log("Signal sequence already started.");
        return;
    }

    sequenceStarted = true;

    if (timeChangeController != null)
    {
        Debug.Log("E sequence requesting time change on: " + timeChangeController.gameObject.name);
        timeChangeController.StartTimeChange();
    }
    else
    {
        Debug.LogWarning("Time Change Controller is not assigned.");
    }

    ShowStatus("SIGNAL MONITORING...");

    StartCoroutine(DelayedMinigameStart());

    Debug.Log("Signal sequence started.");
}
    private IEnumerator DelayedMinigameStart()
    {
        yield return new WaitForSeconds(3f);

        ShowStatus("WEAK ANOMALY DETECTED");
        PlaySignalSound(weakSignalClip);

        yield return new WaitForSeconds(3f);

        ShowStatus("SIGNAL FLUCTUATION DETECTED");
        PlaySignalSound(noiseClip);

        yield return new WaitForSeconds(2f);

        ShowStatus("SCANNING...");

        yield return new WaitForSeconds(2f);

        MoveToMonitor();

        yield return new WaitForSeconds(0.5f);

        if (statusPanel != null)
            statusPanel.SetActive(false);

        if (minigameManager != null)
        {
            minigameManager.ShowMinigame();
        }
        else
        {
            Debug.LogWarning("Minigame Manager is not assigned. Starting old signal routine instead.");
            StartCoroutine(SignalRoutine());
        }
    }

    public void StartWowFoundSequence()
    {
        if (wowFoundSequenceStarted)
        {
            Debug.Log("Wow found sequence already started.");
            return;
        }

        wowFoundSequenceStarted = true;

        // 혹시 E 시점에서 time change가 시작 안 됐을 때 보험
        StartTimeChangeIfNeeded("StartWowFoundSequence");

        StartCoroutine(WowFoundRoutine());
    }

    private IEnumerator WowFoundRoutine()
    {
        ShowStatus("STRONG NARROWBAND SIGNAL DETECTED");
        PlaySignalSound(strongSignalClip);

        yield return new WaitForSeconds(4f);

        ShowStatus("SIGNAL LOCKED — 1420 MHz");

        yield return new WaitForSeconds(2f);

        ShowStatus("Press P to check the Paper");

        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.P));

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
            printerAudioSource.PlayOneShot(printerClip);

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

        yield return new WaitForSeconds(1f);

        MoveToPlayer();

        Debug.Log("Wow found sequence finished. Printer event started.");
    }

    private IEnumerator SignalRoutine()
    {
        ShowStatus("SIGNAL MONITORING...");

        StartTimeChangeIfNeeded("SignalRoutine");

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

        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.P));

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
            printerAudioSource.PlayOneShot(printerClip);

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

        yield return new WaitForSeconds(1f);

        MoveToPlayer();

        Debug.Log("Signal routine finished. Printer event started.");
    }

    private void StartTimeChangeIfNeeded(string callerName)
    {
        if (timeChangeStarted)
        {
            Debug.Log("Time change already started. Caller: " + callerName);
            return;
        }

        if (timeChangeController == null)
        {
            Debug.LogWarning("Time Change Controller is not assigned. Caller: " + callerName);
            return;
        }

        timeChangeStarted = true;
        timeChangeController.StartTimeChange();

        Debug.Log("Time change started. Caller: " + callerName);
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

    private void MoveToMonitor()
    {
        if (playerCamera != null)
            playerCamera.enabled = false;

        if (monitorCamera != null)
            monitorCamera.enabled = true;

        if (playerLookScript != null)
            playerLookScript.enabled = false;

        Debug.Log("Moved to monitor camera.");
    }

    private void MoveToPlayer()
    {
        if (monitorCamera != null)
            monitorCamera.enabled = false;

        if (playerCamera != null)
            playerCamera.enabled = true;

        if (playerLookScript != null)
            playerLookScript.enabled = true;

        Debug.Log("Moved to player camera.");
    }
}