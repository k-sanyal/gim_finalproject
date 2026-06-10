using UnityEngine;
using TMPro;

public class ChairSitInteraction : MonoBehaviour
{
    [Header("Sit Settings")]
    public Transform sitPoint;

    [Header("World UI")]
    public GameObject chairUI;
    public TMP_Text chairText;

    [Header("Message")]
    public string message = "Press E to sit";

    [Header("Game Progress")]
    public GameProgressController gameProgress;

    private PlayerController playerController;
    private bool playerNear = false;
    private bool hasSat = false;

    void Start()
    {
        if (chairText != null)
        {
            chairText.text = message;
        }

        if (chairUI != null)
        {
            chairUI.SetActive(false);
        }
    }

    void Update()
    {
        if (hasSat)
        {
            HideText();
            return;
        }

        if (playerNear)
        {
            ShowText();
        }

        if (playerNear && Input.GetKeyDown(KeyCode.E))
        {
            SitPlayer();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasSat) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("Chair area entered.");

            playerController = other.GetComponent<PlayerController>();
            playerNear = true;
            ShowText();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (hasSat) return;

        if (other.CompareTag("Player"))
        {
            playerNear = true;

            if (playerController == null)
            {
                playerController = other.GetComponent<PlayerController>();
            }

            ShowText();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Chair area exited.");

            playerNear = false;
            playerController = null;
            HideText();
        }
    }

    void SitPlayer()
    {
        if (playerController == null)
        {
            Debug.LogWarning("PlayerController is not assigned.");
            return;
        }

        if (sitPoint == null)
        {
            Debug.LogWarning("SitPoint is not assigned.");
            return;
        }

        playerController.SitAt(sitPoint);

        hasSat = true;
        playerNear = false;

        HideText();

        if (gameProgress != null)
        {
            gameProgress.StartInfoPhase();
        }

        // Optional: disable the chair trigger after sitting
        // Collider chairCollider = GetComponent<Collider>();
        // if (chairCollider != null)
        // {
        //     chairCollider.enabled = false;
        // }
    }

    void ShowText()
    {
        if (hasSat) return;

        if (chairUI != null && !chairUI.activeSelf)
        {
            Debug.Log("Chair UI shown.");
            chairUI.SetActive(true);
        }

        if (chairText != null)
        {
            chairText.text = message;
        }
    }

    void HideText()
    {
        if (chairUI != null)
        {
            chairUI.SetActive(false);
        }
    }
}