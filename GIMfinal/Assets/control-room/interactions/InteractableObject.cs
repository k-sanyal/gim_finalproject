using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Header("Basic Info")]
    public string objectName;

    [TextArea(3, 10)]
    public string detailDescription;

    [Header("Progress")]
    public bool requiredForSignalProgress = true;
    public bool hasBeenViewed = false;

    [Header("Object Type")]
    public bool isMonitor = false;
}