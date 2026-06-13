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
        // 시그널 단계가 열리면 기존 조사 인터랙션 전체 비활성화
        if (gameProgress != null && gameProgress.IsSignalPhaseUnlocked())
        {
            currentObject = null;

            if (hoverPanel != null)
                hoverPanel.SetActive(false);

            // 혹시 상세창이 열린 상태에서 시그널 단계가 열리면 닫기
            if (isDetailOpen)
            {
                ForceCloseDetail();
            }

            return;
        }

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
        if (hoverPanel == null || hoverText == null)
            return;

        hoverPanel.SetActive(true);
        hoverText.text = obj.objectName + "\nK : Inspect";
    }

    void HideHover()
    {
        if (hoverPanel != null)
            hoverPanel.SetActive(false);
    }

    void OpenDetail(InteractableObject obj)
    {
        // 시그널 단계가 열렸으면 상세창 열기 금지
        if (gameProgress != null && gameProgress.IsSignalPhaseUnlocked())
            return;

        isDetailOpen = true;
        openedObject = obj;

        if (hoverPanel != null)
            hoverPanel.SetActive(false);

        if (detailPanel != null)
            detailPanel.SetActive(true);

        titleText.text = obj.objectName;
        bodyText.text = obj.detailDescription;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void CloseDetail()
    {
        isDetailOpen = false;

        if (detailPanel != null)
            detailPanel.SetActive(false);

        if (openedObject != null)
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

    void ForceCloseDetail()
    {
        isDetailOpen = false;
        openedObject = null;

        if (detailPanel != null)
            detailPanel.SetActive(false);

        if (hoverPanel != null)
            hoverPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}