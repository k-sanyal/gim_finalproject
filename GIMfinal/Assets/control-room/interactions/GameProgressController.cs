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
        // 이미 시그널 단계가 열렸으면 기존 조사 UI 다시 켜지지 않게 막음
        if (signalPhaseUnlocked)
            return;

        if (objectivePanel != null)
        {
            objectivePanel.SetActive(true);
        }

        UpdateObjectiveText();
    }

    public void RegisterViewedObject(InteractableObject obj)
    {
        // 이미 시그널 단계가 열렸으면 더 이상 조사 등록 안 함
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
            {
                return;
            }
        }

        signalPhaseUnlocked = true;

        // 4/4 완료 순간 기존 Objective UI 끄기
        if (objectivePanel != null)
            objectivePanel.SetActive(false);

        Debug.Log("Signal Phase Unlocked!");
    }

    void UpdateObjectiveText()
    {
        if (objectiveText == null)
            return;

        // 시그널 단계가 열리면 기존 조사 UI는 꺼진 상태 유지
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

    // InteractionManager에서 조사 가능 여부 확인할 때 사용
    public bool CanInspectObjects()
    {
        return !signalPhaseUnlocked;
    }
}