using UnityEngine;

public class RailAimCamera : MonoBehaviour
{
    [Header("Rotation")]
    public float sensitivity = 120f;

    [Header("Limits")]
    public float horizontalLimit = 12f;
    public float verticalLimit = 8f;

    [Header("Smoothing")]
    public float smooth = 12f;

    float targetYaw;
    float targetPitch;

    Quaternion initialRotation;

    void Start()
    {
        initialRotation = transform.localRotation;

        Cursor.lockState =
            CursorLockMode.Locked;

        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX =
            Input.GetAxisRaw("Mouse X");

        float mouseY =
            Input.GetAxisRaw("Mouse Y");

        targetYaw +=
            mouseX *
            sensitivity *
            Time.deltaTime;

        targetPitch -=
            mouseY *
            sensitivity *
            Time.deltaTime;

        targetYaw =
            Mathf.Clamp(
                targetYaw,
                -horizontalLimit,
                horizontalLimit
            );

        targetPitch =
            Mathf.Clamp(
                targetPitch,
                -verticalLimit,
                verticalLimit
            );

        Quaternion target =
            initialRotation *
            Quaternion.Euler(
                targetPitch,
                targetYaw,
                0
            );

        transform.localRotation =
            Quaternion.Slerp(
                transform.localRotation,
                target,
                smooth *
                Time.deltaTime
            );
    }
}