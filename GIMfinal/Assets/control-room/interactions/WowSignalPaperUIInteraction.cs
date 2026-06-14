using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WowSignalPaperUIInteraction : MonoBehaviour
{
    [Header("Paper UI")]
    public GameObject paperPanel;
    public RawImage circleImage;
    public RectTransform wowClickArea;

    [Header("Cursor")]
    public RectTransform redPenCursor;

    [Header("Mission UI")]
    public TextMeshProUGUI missionText;
    public string beforeText = "Find Wow Signal!";
    public string afterText = "Wow Signal Detected!";

    [Header("Options")]
    public bool hideSystemCursor = true;
    public bool closeAfterSolved = false;
    public float closeDelay = 1.5f;

    [Header("Return To Game")]
    public bool lockCursorWhenClose = true;

    private bool isActive = false;
    private bool isSolved = false;

    private void Start()
    {
        HidePaper();
    }

    private void Update()
    {
        if (!isActive)
            return;

        UpdatePenCursor();

        if (isSolved)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            CheckClick();
        }
    }

    public void ShowPaper()
    {
        isActive = true;
        isSolved = false;

        if (paperPanel != null)
            paperPanel.SetActive(true);

        if (circleImage != null)
            circleImage.gameObject.SetActive(false);

        if (redPenCursor != null)
            redPenCursor.gameObject.SetActive(true);

        if (missionText != null)
            missionText.text = beforeText;

        Cursor.lockState = CursorLockMode.None;

        if (hideSystemCursor)
            Cursor.visible = false;
        else
            Cursor.visible = true;

        Debug.Log("Wow Signal paper UI shown.");
    }

    public void HidePaper()
    {
        isActive = false;

        if (paperPanel != null)
            paperPanel.SetActive(false);

        if (redPenCursor != null)
            redPenCursor.gameObject.SetActive(false);

        if (lockCursorWhenClose)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void UpdatePenCursor()
    {
        if (redPenCursor == null)
            return;

        redPenCursor.position = Input.mousePosition;
    }

    private void CheckClick()
    {
        if (wowClickArea == null)
        {
            Debug.LogWarning("Wow Click Area is not assigned.");
            return;
        }

        bool clickedCorrectArea = RectTransformUtility.RectangleContainsScreenPoint(
            wowClickArea,
            Input.mousePosition,
            null
        );

        if (clickedCorrectArea)
        {
            SolveWowSignal();
        }
        else
        {
            Debug.Log("Wrong area clicked.");
        }
    }

    private void SolveWowSignal()
    {
        isSolved = true;

        if (circleImage != null)
            circleImage.gameObject.SetActive(true);

        if (missionText != null)
            missionText.text = afterText;

        Debug.Log("Wow Signal circled.");

        if (closeAfterSolved)
        {
            Invoke(nameof(HidePaper), closeDelay);
        }
    }
}