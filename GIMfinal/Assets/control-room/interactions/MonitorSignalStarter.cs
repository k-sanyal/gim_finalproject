using UnityEngine;
using UnityEngine.Video;

public class MonitorSignalStarter : MonoBehaviour
{
    [Header("Progress")]
    public GameProgressController progressController;

    [Header("UI")]
    public GameObject pressEUI;
    public GameObject signalPlayingUI;
    public GameObject signalVideoPanel;

    [Header("Video")]
    public VideoPlayer signalVideoPlayer;

    [Header("Next Events")]
    public GameObject wowSignalPaper;

    private bool playerNear = false;
    private bool signalStarted = false;

    private void Start()
    {
        if (pressEUI != null)
            pressEUI.SetActive(false);

        if (signalPlayingUI != null)
            signalPlayingUI.SetActive(false);

        if (signalVideoPanel != null)
            signalVideoPanel.SetActive(false);

        if (wowSignalPaper != null)
            wowSignalPaper.SetActive(false);

        if (signalVideoPlayer != null)
        {
            signalVideoPlayer.playOnAwake = false;
            signalVideoPlayer.isLooping = false;
            signalVideoPlayer.Stop();
            signalVideoPlayer.loopPointReached += OnSignalVideoFinished;
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

        if (playerNear && canStartSignal)
        {
            if (pressEUI != null)
                pressEUI.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                StartSignalMonitoring();
            }
        }
        else
        {
            if (pressEUI != null)
                pressEUI.SetActive(false);
        }
    }

    private void StartSignalMonitoring()
    {
        signalStarted = true;

        if (pressEUI != null)
            pressEUI.SetActive(false);

        if (signalPlayingUI != null)
            signalPlayingUI.SetActive(true);

        if (signalVideoPanel != null)
            signalVideoPanel.SetActive(true);

        if (signalVideoPlayer != null)
        {
            signalVideoPlayer.Stop();
            signalVideoPlayer.time = 0;
            signalVideoPlayer.Play();
            Debug.Log("VideoPlayer Play called.");
        }
        else
        {
            Debug.LogWarning("Signal Video Player is not assigned.");
        }

        Debug.Log("Signal monitoring started.");
    }

    private void OnSignalVideoFinished(VideoPlayer vp)
    {
        if (signalPlayingUI != null)
            signalPlayingUI.SetActive(false);

        if (signalVideoPanel != null)
            signalVideoPanel.SetActive(false);

        if (wowSignalPaper != null)
            wowSignalPaper.SetActive(true);

        Debug.Log("Signal monitoring finished. Wow Signal paper appeared.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = true;
            Debug.Log("Player near monitor.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;

            if (pressEUI != null)
                pressEUI.SetActive(false);

            Debug.Log("Player left monitor.");
        }
    }

    private void OnDestroy()
    {
        if (signalVideoPlayer != null)
        {
            signalVideoPlayer.loopPointReached -= OnSignalVideoFinished;
        }
    }
}