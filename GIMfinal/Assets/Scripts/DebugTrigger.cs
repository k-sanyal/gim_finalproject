using UnityEngine;

public class DebugTrigger : MonoBehaviour
{
    public MonitorInteract monitorInteract;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            monitorInteract.EnterMonitor();
        }
    }
}