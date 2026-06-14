using System.Collections;
using UnityEngine;

public class PrinterPaperAnimator : MonoBehaviour
{
    [Header("Positions")]
    public Transform startPoint;
    public Transform endPoint;

    [Header("Paper Visual")]
    public GameObject paperVisual;

    [Header("Animation")]
    public float printDuration = 3f;

    [Header("Wow Paper UI")]
    public WowSignalPaperUIInteraction wowPaperUI;
    public float openUIDelayAfterPrint = 0.5f;

    [Header("Debug")]
    public bool testWithPKey = true;

    private bool hasPrinted = false;

    // 씬에서 네가 맞춰둔 원래 회전값 저장
    private Quaternion originalRotation;

    private void Awake()
    {
        originalRotation = transform.rotation;
    }

    private void Start()
    {
        ResetPaperToStart();
        Debug.Log("PrinterPaperAnimator ready on: " + gameObject.name);
    }

    private void Update()
    {
        if (testWithPKey && Input.GetKeyDown(KeyCode.P))
        {
            hasPrinted = false;
            ResetPaperToStart();
            StartPrint();
        }
    }

    public void StartPrint()
    {
        if (hasPrinted)
            return;

        if (startPoint == null)
        {
            Debug.LogWarning("Start Point is not assigned.");
            return;
        }

        if (endPoint == null)
        {
            Debug.LogWarning("End Point is not assigned.");
            return;
        }

        hasPrinted = true;

        Debug.Log("StartPrint called.");
        Debug.Log("Moving object: " + gameObject.name);
        Debug.Log("Start world position: " + startPoint.position);
        Debug.Log("End world position: " + endPoint.position);
        Debug.Log("Distance: " + Vector3.Distance(startPoint.position, endPoint.position));

        StartCoroutine(PrintRoutine());
    }

    private IEnumerator PrintRoutine()
    {
        if (paperVisual != null)
            paperVisual.SetActive(true);

        Vector3 startPos = startPoint.position;
        Vector3 endPos = endPoint.position;

        // 회전은 씬에서 맞춰둔 각도 그대로 고정
        transform.rotation = originalRotation;
        transform.position = startPos;

        float elapsed = 0f;
        float logTimer = 0f;

        while (elapsed < printDuration)
        {
            elapsed += Time.deltaTime;
            logTimer += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / printDuration);

            transform.position = Vector3.Lerp(startPos, endPos, t);
            transform.rotation = originalRotation;

            if (logTimer >= 0.5f)
            {
                Debug.Log("Printing t=" + t + " / root position=" + transform.position);
                logTimer = 0f;
            }

            yield return null;
        }

        transform.position = endPos;
        transform.rotation = originalRotation;

        Debug.Log("Final root position: " + transform.position);
        Debug.Log("Wow Signal paper printed.");

        yield return new WaitForSeconds(openUIDelayAfterPrint);

        if (wowPaperUI != null)
        {
            wowPaperUI.ShowPaper();
            Debug.Log("Wow paper UI opened automatically.");
        }
        else
        {
            Debug.LogWarning("Wow Paper UI is not assigned in PrinterPaperAnimator.");
        }
    }

    private void ResetPaperToStart()
    {
        if (startPoint == null)
            return;

        transform.position = startPoint.position;
        transform.rotation = originalRotation;

        if (paperVisual != null)
            paperVisual.SetActive(false);
    }
}