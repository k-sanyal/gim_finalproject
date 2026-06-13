using UnityEngine;
using Unity.Cinemachine;

public class MonitorInteract : MonoBehaviour
{
    public CinemachineCamera monitorCam;
    public GameObject interactPrompt;
    public MinigameManager minigame;

    private bool playerNear = false;
    private bool isActive = false;

    void Start()
    {
        monitorCam.Priority = -1;
        if(interactPrompt != null) interactPrompt.SetActive(false);
    }

    void Update()
    {
        if(playerNear && !isActive && Input.GetKeyDown(KeyCode.E))
            EnterMonitor();

        if(isActive && Input.GetKeyDown(KeyCode.Escape))
            ExitMonitor();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerNear = true;
            if(interactPrompt != null) interactPrompt.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerNear = false;
            if(interactPrompt != null) interactPrompt.SetActive(false);
        }
    }

    public void EnterMonitor()
    {
        isActive = true;
        if(interactPrompt != null) interactPrompt.SetActive(false);
        monitorCam.Priority = 20;
        Invoke("StartMinigame", 1.2f);
    }

    public void ExitMonitor()
    {
        isActive = false;
        monitorCam.Priority = -1;
        if(minigame != null) minigame.HideMinigame();
    }

    void StartMinigame()
    {
        if(minigame != null) minigame.ShowMinigame();
    }
}