using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Movement")]
    public float PanSpeed = 65f;
    public float SprintMultiplier = 2.0f;
    public Vector2 PanLimit = new Vector2(100f, 100f);

    [Header("Zoom")]
    public float ScrollSpeed = 4.5f;
    public float MinY = 6f;
    public float MaxY = 60f;

    [Header("Smoothing")]
    public float Smoothing = 12f;

    private Vector3 _targetPosition;

    private void Start()
    {
        _targetPosition = transform.position;
    }

    private void Update()
    {
        Vector3 pos = _targetPosition;

        // Calculate screen-relative planar directions so WASD always moves relative to the view
        Vector3 camForward = transform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = transform.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 moveDir = Vector3.zero;

        Keyboard kb = Keyboard.current;
        float currentSpeed = PanSpeed;

        if (kb != null)
        {
            if (kb.leftShiftKey.isPressed || kb.rightShiftKey.isPressed)
                currentSpeed *= SprintMultiplier;

            if (kb.wKey.isPressed || kb.upArrowKey.isPressed)
                moveDir += camForward;
            if (kb.sKey.isPressed || kb.downArrowKey.isPressed)
                moveDir -= camForward;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed)
                moveDir += camRight;
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed)
                moveDir -= camRight;
        }

        if (moveDir.sqrMagnitude > 0.001f)
        {
            pos += moveDir.normalized * (currentSpeed * Time.deltaTime);
        }

        Mouse mouse = Mouse.current;
        if (mouse != null)
        {
            float rawScroll = mouse.scroll.ReadValue().y;
            if (Mathf.Abs(rawScroll) > 0.01f)
            {
                // Normalize: handles raw delta (+/-120) and normalized delta (+/-1)
                float scrollDelta = Mathf.Abs(rawScroll) >= 1.5f ? (rawScroll / 120f) : rawScroll;
                float zoomAmount = scrollDelta * ScrollSpeed;

                if (kb != null && (kb.leftShiftKey.isPressed || kb.rightShiftKey.isPressed))
                {
                    zoomAmount *= SprintMultiplier;
                }

                // Dolly along camera forward vector for fast, intuitive zooming
                pos += transform.forward * zoomAmount;
            }
        }

        // Clamping limits
        pos.x = Mathf.Clamp(pos.x, -PanLimit.x, PanLimit.x);
        pos.y = Mathf.Clamp(pos.y, MinY, MaxY);
        pos.z = Mathf.Clamp(pos.z, -PanLimit.y, PanLimit.y);

        _targetPosition = pos;

        // Apply smooth movement
        transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * Smoothing);
    }
}
