using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLook : MonoBehaviour
{
    public float sensitivity = 0.1f;
    public float distance = 5f;
    public float height = 1.6f;

    Transform target;
    float yaw;
    float pitch = 15f;

    void Start()
    {
        LockCursor();
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame) LockCursor();

        if (target == null)
        {
            NetworkManager nm = NetworkManager.Singleton;
            if (nm == null || nm.LocalClient == null || nm.LocalClient.PlayerObject == null) return;
            target = nm.LocalClient.PlayerObject.transform;
        }

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            yaw += delta.x * sensitivity;
            pitch = Mathf.Clamp(pitch - delta.y * sensitivity, -30f, 70f);
        }

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 pivot = target.position + Vector3.up * height;
        transform.position = pivot - rotation * Vector3.forward * distance;
        transform.rotation = rotation;
    }
}