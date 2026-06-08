using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Camera")]
    public Transform cameraTransform;

    [Header("Move Settings")]
    public float moveSpeed = 2.5f;

    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 1.5f;
    public float lookSmooth = 10f;

    [Header("Vertical Look Limit")]
    public float minPitch = -30f;
    public float maxPitch = 30f;

    private float targetYaw;
    private float targetPitch;

    private float currentYaw;
    private float currentPitch;

    void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        // 시작할 때 현재 Player의 Y 회전값을 기준으로 잡음
        targetYaw = transform.eulerAngles.y;
        currentYaw = targetYaw;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        MovePlayer();
        LookWithMouse();

        // ESC 누르면 마우스 잠금 해제
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // 다시 클릭하면 마우스 잠금
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void MovePlayer()
    {
        float horizontal = Input.GetAxisRaw("Horizontal"); // A, D
        float vertical = Input.GetAxisRaw("Vertical");     // W, S

        Vector3 moveDirection =
            transform.right * horizontal +
            transform.forward * vertical;

        moveDirection.y = 0f;
        moveDirection.Normalize();

        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    void LookWithMouse()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        targetYaw += mouseX;
        targetPitch -= mouseY;

        targetPitch = Mathf.Clamp(targetPitch, minPitch, maxPitch);

        currentYaw = Mathf.Lerp(
            currentYaw,
            targetYaw,
            Time.deltaTime * lookSmooth
        );

        currentPitch = Mathf.Lerp(
            currentPitch,
            targetPitch,
            Time.deltaTime * lookSmooth
        );

        // 좌우 회전은 Player 전체가 담당
        transform.rotation = Quaternion.Euler(0f, currentYaw, 0f);

        // 위아래 회전은 Main Camera만 담당
        cameraTransform.localRotation = Quaternion.Euler(currentPitch, 0f, 0f);
    }
}