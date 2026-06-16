using System.Collections;
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

    [Header("Time Change")]
    public TimeChangeController timeChangeController;

    [Header("Time Change Delay")]
    public float timeChangeStartDelay = 0.3f;

    private bool timeChangeStarted = false;

    public void StartInfoPhase()
    {
        if (signalPhaseUnlocked)
            return;

        if (objectivePanel != null)
            objectivePanel.SetActive(true);

        UpdateObjectiveText();
    }

    public void RegisterViewedObject(InteractableObject obj)
    {
        if (signalPhaseUnlocked)
            return;

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
        if (signalPhaseUnlocked)
            return;

        foreach (InteractableObject obj in requiredObjects)
        {
            if (obj == null)
                continue;

            if (!obj.hasBeenViewed)
                return;
        }

        signalPhaseUnlocked = true;

        if (objectivePanel != null)
            objectivePanel.SetActive(false);

        Debug.Log("Signal Phase Unlocked!");

        StartTimeChangeFromProgress();
    }

    private void StartTimeChangeFromProgress()
    {
        if (timeChangeStarted)
        {
            Debug.Log("Time change already started from GameProgressController.");
            return;
        }

        if (timeChangeController == null)
        {
            Debug.LogWarning("Time Change Controller is not assigned in GameProgressController.");
            return;
        }

        timeChangeStarted = true;

        StartCoroutine(DelayedTimeChangeStart());

        Debug.Log("Time change scheduled when all required objects were reviewed.");
    }

    private IEnumerator DelayedTimeChangeStart()
    {
        yield return new WaitForSeconds(timeChangeStartDelay);

        if (timeChangeController != null)
        {
            Debug.Log("Delayed time change start now.");
            timeChangeController.StartTimeChange();
        }
        else
        {
            Debug.LogWarning("Time Change Controller became null before delayed start.");
        }
    }

    void UpdateObjectiveText()
    {
        if (objectiveText == null)
            return;

        if (signalPhaseUnlocked)
        {
            if (objectivePanel != null)
                objectivePanel.SetActive(false);

            return;
        }

        int viewedCount = 0;
        int totalCount = 0;

        foreach (InteractableObject obj in requiredObjects)
        {
            if (obj == null)
                continue;

            totalCount++;

            if (obj.hasBeenViewed)
                viewedCount++;
        }

        objectiveText.text =
            "Review the observation materials around the room.\n" +
            viewedCount + " / " + totalCount + " reviewed";
    }

    public bool IsSignalPhaseUnlocked()
    {
        return signalPhaseUnlocked;
    }

    public bool CanInspectObjects()
    {
        return !signalPhaseUnlocked;
    }
}