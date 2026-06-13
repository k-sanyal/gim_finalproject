using UnityEngine;
using UnityEngine.Video;

public class MonitorSignalStarter : MonoBehaviour
{
    [Header("Progress")]
    public GameProgressController progressController;

    [Header("Camera")]
    public Camera playerCamera;
    public float interactDistance = 4f;

    [Header("UI")]
    public GameObject pressEUI;
    public GameObject signalPlayingUI;

    [Header("Video")]
    public VideoPlayer signalVideoPlayer;

    [Header("Signal Sequence")]
    public SignalSequenceController signalSequenceController;

    private bool signalStarted = false;

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

        if (playerCamera == null)
            playerCamera = Camera.main;

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
        bool lookingAtMonitor = IsLookingAtThisMonitor();

        if (canStartSignal && lookingAtMonitor)
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

    private bool IsLookingAtThisMonitor()
    {
        if (playerCamera == null)
            return false;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
                return true;

            if (transform.IsChildOf(hit.transform))
                return true;
        }

        return false;
    }

    private void StartSignalMonitoring()
    {
        signalStarted = true;

        if (pressEUI != null)
            pressEUI.SetActive(false);

        if (signalPlayingUI != null)
            signalPlayingUI.SetActive(true);

        if (signalVideoPlayer != null)
        {
            signalVideoPlayer.Stop();
            signalVideoPlayer.time = 0;
            signalVideoPlayer.isLooping = true;
            signalVideoPlayer.Play();

            Debug.Log("Loop signal video started.");
        }
        else
        {
            Debug.LogWarning("Signal Video Player is not assigned.");
        }

        if (signalSequenceController != null)
        {
            signalSequenceController.StartSequence();
            Debug.Log("Signal sequence started from MonitorSignalStarter.");
        }
        else
        {
            Debug.LogWarning("Signal Sequence Controller is not assigned.");
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
}