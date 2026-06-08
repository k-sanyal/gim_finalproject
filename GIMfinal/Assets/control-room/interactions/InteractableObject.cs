using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Header("Basic Info")]
    public string objectName;

    [TextArea(3, 10)]
    public string detailDescription;

    [Header("Object Type")]
    public bool isProgressObject = false;
}