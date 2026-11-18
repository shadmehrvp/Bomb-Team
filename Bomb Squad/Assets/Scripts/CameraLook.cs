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


    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

  void LateUpdate()
    {
        if (target == null)
        {
            NetworkManager nm = NetworkManager.Singleton;
            if (nm == null || nm.LocalClient == null || nm.LocalClient.PlayerObject == null) return;

            target = nm.LocalClient.PlayerObject.transform;
            LockCursor();  
        }

        Mouse mouse = Mouse.current;
        Keyboard kb = Keyboard.current;

        // NEW: Esc frees the mouse, click locks it again
        if (kb != null && kb.escapeKey.wasPressedThisFrame) UnlockCursor();
        if (mouse != null && mouse.leftButton.wasPressedThisFrame) LockCursor();

        if (mouse != null && Cursor.lockState == CursorLockMode.Locked)
        {
            Vector2 delta = mouse.delta.ReadValue();
            yaw += delta.x * sensitivity;
            pitch = Mathf.Clamp(pitch - delta.y * sensitivity, -30f, 70f);
        }

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 pivot = target.position + Vector3.up * height;
        transform.position = pivot - rotation * Vector3.forward * distance;
        transform.rotation = rotation;
    }
}