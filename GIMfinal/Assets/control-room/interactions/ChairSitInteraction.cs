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
        HideText();
    }

    void Update()
    {
        if (playerNear && Input.GetKeyDown(KeyCode.E))
        {
            SitPlayer();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController = other.GetComponent<PlayerController>();
            playerNear = true;
            ShowText();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;
            playerController = null;
            HideText();
        }
    }

    void SitPlayer()
    {
        if (playerController == null) return;
        if (sitPoint == null) return;

        playerController.SitAt(sitPoint);

        HideText();
        playerNear = false;
    }

    void ShowText()
    {
        chairUI.SetActive(true);
        chairText.text = message;
    }

    void HideText()
    {
        chairUI.SetActive(false);
        chairText.text = "";
    }
}