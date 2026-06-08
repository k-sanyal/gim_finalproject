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
    public string message = "E키를 눌러 앉기";

    private PlayerController playerController;
    private bool playerNear = false;

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
        if (other.CompareTag("Player"))
        {
            Debug.Log("의자 범위 진입");

            playerController = other.GetComponent<PlayerController>();
            playerNear = true;
            ShowText();
        }
    }

    void OnTriggerStay(Collider other)
    {
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
            Debug.Log("의자 범위 나감");

            playerNear = false;
            playerController = null;
            HideText();
        }
    }

    void SitPlayer()
    {
        if (playerController == null)
        {
            Debug.LogWarning("PlayerController가 연결되지 않았음");
            return;
        }

        if (sitPoint == null)
        {
            Debug.LogWarning("SitPoint가 연결되지 않았음");
            return;
        }

        playerController.SitAt(sitPoint);

        HideText();
        playerNear = false;
    }

    void ShowText()
    {
        if (chairUI != null && !chairUI.activeSelf)
        {
            Debug.Log("의자 UI 표시");
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