using UnityEngine;
using Cinemachine;

public class MonitorInteract : MonoBehaviour
{
    public CinemachineVirtualCamera monitorCam;
    public CinemachineVirtualCamera playerCam;
    public GameObject interactPrompt;
    public MinigameManager minigame;

    private bool playerNear = false;
    private bool isActive = false;

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
            interactPrompt.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerNear = false;
            interactPrompt.SetActive(false);
        }
    }

    void EnterMonitor()
    {
        isActive = true;
        interactPrompt.SetActive(false);
        monitorCam.Priority = 20;
        playerCam.Priority = 10;
        Invoke("StartMinigame", 1.2f);
    }

    public void ExitMonitor()
    {
        isActive = false;
        monitorCam.Priority = 10;
        playerCam.Priority = 20;
        minigame.HideMinigame();
    }
}