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

    [Header("Signal Start")]
    public GameObject signalStartUI; // "E : 시그널 감지 시작" UI
    public MonitorSignalStarter monitorSignalStarter;

    private InteractableObject currentObject;
    private InteractableObject openedObject;

    private bool isDetailOpen = false;
    private bool signalStarted = false;

    private void Start()
    {
        if (signalStartUI != null)
            signalStartUI.SetActive(false);
    }

    void Update()
    {
        // 시그널 단계가 열리면 기존 조사 인터랙션 전체 비활성화
        if (gameProgress != null && gameProgress.IsSignalPhaseUnlocked())
        {
            HandleSignalStartPhase();
            return;
        }

        if (signalStartUI != null)
            signalStartUI.SetActive(false);

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

    private void HandleSignalStartPhase()
    {
        currentObject = null;

        if (hoverPanel != null)
            hoverPanel.SetActive(false);

        // 혹시 상세창이 열린 상태에서 시그널 단계가 열리면 닫기
        if (isDetailOpen)
        {
            ForceCloseDetail();
        }

        if (signalStarted)
        {
            if (signalStartUI != null)
                signalStartUI.SetActive(false);

            return;
        }

        // 4개 조사 완료 후 E 안내 UI 표시
        if (signalStartUI != null)
            signalStartUI.SetActive(true);

        // E 누르면 시그널 감지 시작
        if (Input.GetKeyDown(KeyCode.E))
        {
            signalStarted = true;

            if (signalStartUI != null)
                signalStartUI.SetActive(false);

            if (monitorSignalStarter != null)
            {
                monitorSignalStarter.StartSignalMonitoringFromOutside();
            }
            else
            {
                Debug.LogWarning("Monitor Signal Starter is not assigned in InteractionManager.");
            }
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

        if (titleText != null)
            titleText.text = obj.objectName;

        if (bodyText != null)
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