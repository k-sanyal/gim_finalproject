using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Camera")]
    public Transform cameraTransform;

    [Header("Movement")]
    public float moveSpeed = 2.5f;
    public float gravity = -9.81f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 1.0f;
    public float lookSmooth = 12f;

    [Header("Vertical Look Limit")]
    public float minPitch = -25f;
    public float maxPitch = 25f;

    [Header("State")]
    public bool isSitting = false;

    private CharacterController controller;

    private float targetYaw;
    private float targetPitch;
    private float currentYaw;
    private float currentPitch;

    private float verticalVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        targetYaw = transform.eulerAngles.y;
        currentYaw = targetYaw;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!isSitting)
        {
            MovePlayer();
        }

        LookWithMouse();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void MovePlayer()
{
    float horizontal = Input.GetAxisRaw("Horizontal");
    float vertical = Input.GetAxisRaw("Vertical");

    Vector3 moveDirection =
        transform.right * horizontal +
        transform.forward * vertical;

    moveDirection.y = 0f;
    moveDirection.Normalize();

    controller.Move(moveDirection * moveSpeed * Time.deltaTime);
}
    void LookWithMouse()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        targetYaw += mouseX;
        targetPitch -= mouseY;

        targetPitch = Mathf.Clamp(targetPitch, minPitch, maxPitch);

        currentYaw = Mathf.Lerp(currentYaw, targetYaw, Time.deltaTime * lookSmooth);
        currentPitch = Mathf.Lerp(currentPitch, targetPitch, Time.deltaTime * lookSmooth);

        transform.rotation = Quaternion.Euler(0f, currentYaw, 0f);
        cameraTransform.localRotation = Quaternion.Euler(currentPitch, 0f, 0f);
    }

    public void SitAt(Transform sitPoint)
    {
        isSitting = true;

        controller.enabled = false;

        transform.position = sitPoint.position;
        transform.rotation = sitPoint.rotation;

        controller.enabled = true;

        targetYaw = transform.eulerAngles.y;
        currentYaw = targetYaw;

        targetPitch = 0f;
        currentPitch = 0f;

        cameraTransform.localRotation = Quaternion.identity;
    }
}