using UnityEngine;
using UnityEngine.Video;

public class MonitorSignalStarter : MonoBehaviour
{
    [Header("Progress")]
    public GameProgressController progressController;

    [Header("UI")]
    public GameObject pressEUI;
    public GameObject signalPlayingUI;

    [Header("Video")]
    public VideoPlayer signalVideoPlayer;

    [Header("Signal Sequence")]
    public SignalSequenceController signalSequenceController;

    [Header("Prompt Delay")]
    public float eInputDelay = 1f;

    private bool signalStarted = false;
    private bool promptShown = false;
    private float promptShownTime = 0f;

    private void Start()
    {
        if (pressEUI != null)
            pressEUI.SetActive(false);

        if (signalPlayingUI != null)
            signalPlayingUI.SetActive(false);

        if (signalVideoPlayer != null)
        {
            signalVideoPlayer.playOnAwake = false;
            signalVideoPlayer.isLooping = true;
            signalVideoPlayer.Stop();
        }

        Debug.Log("MonitorSignalStarter ready.");
    }

    private void Update()
    {
        if (signalStarted)
            return;

        if (progressController == null)
        {
            Debug.LogWarning("Progress Controller is not assigned.");
            return;
        }

        bool canStartSignal = progressController.IsSignalPhaseUnlocked();

        if (!canStartSignal)
        {
            if (pressEUI != null)
                pressEUI.SetActive(false);

            promptShown = false;
            return;
        }

        if (!promptShown)
        {
            promptShown = true;
            promptShownTime = Time.time;

            if (pressEUI != null)
                pressEUI.SetActive(true);

            Debug.Log("Press E UI shown.");
        }

        if (Time.time - promptShownTime < eInputDelay)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            StartSignalMonitoring();
        }
    }

    private void StartSignalMonitoring()
    {
        signalStarted = true;

        if (pressEUI != null)
            pressEUI.SetActive(false);

        if (signalPlayingUI != null)
            signalPlayingUI.SetActive(true);

        // 먼저 시간 변화 / 시퀀스 시작
        if (signalSequenceController != null)
        {
            signalSequenceController.StartSequence();
            Debug.Log("Signal sequence started from MonitorSignalStarter.");
        }
        else
        {
            Debug.LogWarning("Signal Sequence Controller is not assigned.");
        }

        // 그 다음 모니터 신호 영상 시작
        if (signalVideoPlayer != null)
        {
            signalVideoPlayer.Stop();
            signalVideoPlayer.time = 0;
            signalVideoPlayer.isLooping = true;
            signalVideoPlayer.Play();

            Debug.Log("Loop signal video started. VideoPlayer object: " + signalVideoPlayer.gameObject.name);
        }
        else
        {
            Debug.LogWarning("Signal Video Player is not assigned.");
        }

        Debug.Log("Signal monitoring started.");
    }

    public void StopSignalVideo()
    {
        if (signalVideoPlayer != null)
        {
            signalVideoPlayer.isLooping = false;
            signalVideoPlayer.Stop();
        }

        if (signalPlayingUI != null)
            signalPlayingUI.SetActive(false);

        Debug.Log("Loop signal video stopped.");
    }

    public void StartSignalMonitoringFromOutside()
    {
        if (signalStarted)
            return;

        StartSignalMonitoring();
    }
}