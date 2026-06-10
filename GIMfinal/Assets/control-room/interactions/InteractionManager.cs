using UnityEngine;
using TMPro;

public class InteractionManager : MonoBehaviour
{
    [Header("Raycast Settings")]
    public Camera playerCamera;
    public float interactDistance = 5f;

    [Header("Hover UI")]
    public GameObject hoverPanel;
    public TMP_Text hoverText;

    [Header("Detail UI")]
    public GameObject detailPanel;
    public TMP_Text titleText;
    public TMP_Text bodyText;

    [Header("Game Progress")]
    public GameProgressController gameProgress;

    private InteractableObject currentObject;
    private InteractableObject openedObject;

    private bool isDetailOpen = false;

    void Update()
    {
        if (isDetailOpen)
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                CloseDetail();
            }

            return;
        }

        DetectObject();

        if (currentObject != null && Input.GetKeyDown(KeyCode.K))
        {
            OpenDetail(currentObject);
        }
    }

    void DetectObject()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();

            if (interactable != null)
            {
                currentObject = interactable;
                ShowHover(interactable);
                return;
            }
        }

        currentObject = null;
        HideHover();
    }

    void ShowHover(InteractableObject obj)
    {
        hoverPanel.SetActive(true);

        if (obj.isMonitor && gameProgress != null && gameProgress.IsSignalPhaseUnlocked())
        {
            hoverText.text = obj.objectName + "\nK : Start Signal Monitoring";
        }
        else
        {
            hoverText.text = obj.objectName + "\nK : Inspect";
        }
    }

    void HideHover()
    {
        hoverPanel.SetActive(false);
    }

    void OpenDetail(InteractableObject obj)
    {
        isDetailOpen = true;
        openedObject = obj;

        hoverPanel.SetActive(false);
        detailPanel.SetActive(true);

        if (obj.isMonitor)
        {
            OpenMonitorDetail(obj);
            return;
        }

        titleText.text = obj.objectName;
        bodyText.text = obj.detailDescription;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void OpenMonitorDetail(InteractableObject obj)
    {
        titleText.text = obj.objectName;

        if (gameProgress != null && gameProgress.IsSignalPhaseUnlocked())
        {
            bodyText.text = "Signal monitoring is now available.\nThe system is ready to begin scanning incoming radio data.";
        }
        else
        {
            bodyText.text = "The monitor is not ready yet.\nReview the observation materials first.";
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void CloseDetail()
    {
        isDetailOpen = false;

        detailPanel.SetActive(false);

        if (openedObject != null && !openedObject.isMonitor)
        {
            if (gameProgress != null)
            {
                gameProgress.RegisterViewedObject(openedObject);
            }
        }

        openedObject = null;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}