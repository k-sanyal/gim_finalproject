using UnityEngine;
using TMPro;

public class GameProgressController : MonoBehaviour
{
    [Header("Required Info Objects")]
    public InteractableObject[] requiredObjects;

    [Header("Progress State")]
    public bool signalPhaseUnlocked = false;

    [Header("Objective UI")]
    public GameObject objectivePanel;
    public TMP_Text objectiveText;

    public void StartInfoPhase()
    {
        if (objectivePanel != null)
        {
            objectivePanel.SetActive(true);
        }

        UpdateObjectiveText();
    }

   public void RegisterViewedObject(InteractableObject obj)
{
    if (obj == null)
    {
        Debug.LogWarning("RegisterViewedObject failed: obj is null");
        return;
    }

    Debug.Log("RegisterViewedObject called: " + obj.objectName);

    if (obj.requiredForSignalProgress)
    {
        obj.hasBeenViewed = true;
        Debug.Log(obj.objectName + " marked as viewed.");
    }
    else
    {
        Debug.Log(obj.objectName + " is not required for progress.");
    }

    CheckSignalPhaseUnlock();
    UpdateObjectiveText();
}

    void CheckSignalPhaseUnlock()
    {
        if (signalPhaseUnlocked) return;

        foreach (InteractableObject obj in requiredObjects)
        {
            if (obj == null) continue;

            if (!obj.hasBeenViewed)
            {
                return;
            }
        }

        signalPhaseUnlocked = true;
        Debug.Log("Signal Phase Unlocked!");
    }

    void UpdateObjectiveText()
    {
        if (objectiveText == null) return;

        if (signalPhaseUnlocked)
        {
            objectiveText.text = "Signal monitoring is now available.\nCheck the signal monitor.";
            return;
        }

        int viewedCount = 0;
        int totalCount = 0;

        foreach (InteractableObject obj in requiredObjects)
        {
            if (obj == null) continue;

            totalCount++;

            if (obj.hasBeenViewed)
            {
                viewedCount++;
            }
        }

        objectiveText.text =
            "Review the observation materials around the room.\n" +
            viewedCount + " / " + totalCount + " reviewed";
    }

    public bool IsSignalPhaseUnlocked()
    {
        return signalPhaseUnlocked;
    }
}