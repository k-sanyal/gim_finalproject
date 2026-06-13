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

    [Header("Next Events")]
    public GameObject wowSignalPaper;

    private bool signalStarted = false;

    private void Start()
    {
        if (pressEUI != null)
            pressEUI.SetActive(false);

        if (signalPlayingUI != null)
            signalPlayingUI.SetActive(false);

        if (wowSignalPaper != null)
            wowSignalPaper.SetActive(false);

        if (signalVideoPlayer != null)
        {
            signalVideoPlayer.playOnAwake = false;
            signalVideoPlayer.isLooping = false;
            signalVideoPlayer.Stop();
            signalVideoPlayer.loopPointReached += OnSignalVideoFinished;
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
            // 이 스크립트가 붙은 오브젝트를 직접 맞췄거나,
            // 그 자식 오브젝트를 맞췄을 때 true
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                return true;
            }

            // 반대로 이 스크립트가 화면의 자식에 붙어 있고 부모를 맞추는 경우 대비
            if (transform.IsChildOf(hit.transform))
            {
                return true;
            }
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

        if (wowSignalPaper != null)
            wowSignalPaper.SetActive(true);

        Debug.Log("Signal monitoring finished. Wow Signal paper appeared.");
    }

    private void OnDestroy()
    {
        if (signalVideoPlayer != null)
        {
            signalVideoPlayer.loopPointReached -= OnSignalVideoFinished;
        }
    }
}