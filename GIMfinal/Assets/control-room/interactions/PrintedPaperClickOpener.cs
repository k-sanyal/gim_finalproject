using UnityEngine;

public class PrintedPaperClickOpener : MonoBehaviour
{
    [Header("UI To Open")]
    public WowSignalPaperUIInteraction wowPaperUI;

    [Header("Prompt UI")]
    public GameObject pressEToCheckPaperUI;

    [Header("Interaction")]
    public bool canOpen = false;

    [Header("Cursor")]
    public bool unlockCursorWhenOpen = true;

    private bool hasOpened = false;

    private void Start()
    {
        DisableClick();
    }

    private void Update()
    {
        if (!canOpen)
            return;

        if (hasOpened)
            return;

        if (pressEToCheckPaperUI != null)
            pressEToCheckPaperUI.SetActive(true);

        if (Input.GetKeyDown(KeyCode.E))
        {
            OpenPaperUI();
        }
    }

    private void OpenPaperUI()
    {
        hasOpened = true;
        canOpen = false;

        if (pressEToCheckPaperUI != null)
            pressEToCheckPaperUI.SetActive(false);

        Debug.Log("Printed paper opened with E.");

        if (unlockCursorWhenOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (wowPaperUI != null)
        {
            wowPaperUI.ShowPaper();
        }
        else
        {
            Debug.LogWarning("Wow Paper UI is not assigned.");
        }
    }

    public void EnableClick()
    {
        canOpen = true;
        hasOpened = false;

        if (pressEToCheckPaperUI != null)
            pressEToCheckPaperUI.SetActive(true);

        Debug.Log("Printed paper can now be checked with E.");
    }

    public void DisableClick()
    {
        canOpen = false;
        hasOpened = false;

        if (pressEToCheckPaperUI != null)
            pressEToCheckPaperUI.SetActive(false);
    }
}