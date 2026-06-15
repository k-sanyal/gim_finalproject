using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;

public class MinigameManager : MonoBehaviour
{
    [Header("References")]
    public RectTransform crosshair;
    public RectTransform signalContainer;
    public GameObject signalPrefab;
    public Slider frequencyBar;
    public TMP_Text statusText;
    public PlayableDirector timeline;
    public MonitorInteract monitor;

    [Header("Settings")]
    public int signalsBeforeWow = 4;
    public float catchRadius = 40f;
    public float fillSpeed = 1.2f;

    [Header("Printer")]
    public PrinterPaperAnimator printerPaperAnimator;
    public AudioSource printerAudioSource;
    public AudioClip printerClip;
    public MonitorSignalStarter monitorSignalStarter;

    private List<GameObject> activeSignals = new List<GameObject>();
    private int caughtCount = 0;
    private bool wowActive = false;
    private bool gameActive = false;
    private GameObject wowSignal;

    [Header("Audio")]
    //public AudioClip catchSound;
    public AudioClip wowSound;
    public AudioClip scanningLoop;
    private AudioSource audioSource;

    [Header("Camera")]
    public Camera monitorCamera;

    [Header("Canvas")]
    public GameObject minigameCanvas;

    [Header("Signal Sequence")]
    public SignalSequenceController signalSequenceController;

    void Update()
    {
        if(!gameActive) return;

        MoveCrosshair();
        CheckSignalProximity();
    }

    void MoveCrosshair()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            signalContainer.parent.GetComponent<RectTransform>(),
            Input.mousePosition,
            monitorCamera,
            out localPoint
        );
        crosshair.localPosition = localPoint;
    }

    void CheckSignalProximity()
    {
        foreach(var sig in activeSignals)
        {
            if(sig == null) continue;

            float dist = Vector2.Distance(
                crosshair.localPosition,
                sig.GetComponent<RectTransform>().localPosition
            );

            if(dist < catchRadius)
            {
                frequencyBar.value += fillSpeed * Time.deltaTime;
                statusText.text = sig.GetComponent<SignalObject>().isWow ?
                    "! ANOMALOUS SIGNAL" : "SIGNAL LOCKED";

                if(frequencyBar.value >= 1f)
                    CatchSignal(sig);
                return;
            }
        }
        frequencyBar.value -= fillSpeed * 0.5f * Time.deltaTime;
        statusText.text = "SCANNING...";
    }

    void CatchSignal(GameObject sig)
    {
        bool isWow = sig.GetComponent<SignalObject>().isWow;
        activeSignals.Remove(sig);
        Destroy(sig);
        frequencyBar.value = 0f;
        caughtCount++;

        if(isWow)
        {
            StartCoroutine(TriggerWowEnding());
            return;
        }

        statusText.text = "CLASSIFIED";
        SpawnSignal(false);

        if(caughtCount >= signalsBeforeWow && !wowActive)
        {
            wowActive = true;
            Invoke("SpawnWow", 1.5f);
        }
    }

    void SpawnSignal(bool isWow)
    {
        GameObject sig = Instantiate(signalPrefab, signalContainer);
        RectTransform rt = sig.GetComponent<RectTransform>();

        float radius = 160f;
        Vector2 randPos = Random.insideUnitCircle * radius;
        rt.anchoredPosition = randPos;

        sig.GetComponent<SignalObject>().isWow = isWow;
        sig.GetComponent<SignalObject>().Init(isWow);
        activeSignals.Add(sig);
    }

    void SpawnWow()
    {
        SpawnSignal(true);
        statusText.text = "UNKNOWN SIGNAL DETECTED";
    }


    IEnumerator TriggerWowEnding()
    {
        gameActive = false;
        statusText.text = "Wow!";
        if(wowSound) audioSource.PlayOneShot(wowSound);
        audioSource.Stop();

        yield return new WaitForSeconds(2f);

        // call BEFORE HideMinigame
        if(signalSequenceController != null)
            signalSequenceController.StartWowFoundSequence();

        HideMinigame();
    }


    // IEnumerator TriggerWowEnding() (OLD)
    // {
    //     gameActive = false;
    //     statusText.text = "Wow!";
    //     if(wowSound) audioSource.PlayOneShot(wowSound);
    //     audioSource.Stop();

    //     yield return new WaitForSeconds(2f);

    //     HideMinigame();

    //     yield return new WaitForSeconds(1f);

    //     if(timeline != null) timeline.Play();
    // }

    public void ShowMinigame()
    {
        if(minigameCanvas != null) minigameCanvas.SetActive(true);

        gameObject.SetActive(true);
        gameActive = true;
        caughtCount = 0;
        wowActive = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        for(int i = 0; i < 3; i++) SpawnSignal(false);

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = scanningLoop;
        audioSource.loop = true;
        audioSource.volume = 0.3f;
        audioSource.Play();
    }

    public void HideMinigame()
    {
        gameActive = false;
        gameObject.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        foreach(var s in activeSignals)
            if(s != null) Destroy(s);
        activeSignals.Clear();

        if(minigameCanvas != null) minigameCanvas.SetActive(false);
    }

    void Start()
    {
        //ShowMinigame();
    }
}