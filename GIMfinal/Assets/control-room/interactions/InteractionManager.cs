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

    private InteractableObject currentObject;
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
        hoverText.text = obj.objectName + "\nK : 자세히 보기";
    }

    void HideHover()
    {
        hoverPanel.SetActive(false);
    }

    void OpenDetail(InteractableObject obj)
    {
        isDetailOpen = true;

        hoverPanel.SetActive(false);
        detailPanel.SetActive(true);

        titleText.text = obj.objectName;
        bodyText.text = obj.detailDescription;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void CloseDetail()
    {
        isDetailOpen = false;

        detailPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}