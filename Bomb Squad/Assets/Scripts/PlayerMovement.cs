using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : NetworkBehaviour
{
    public float speed = 5f;
    public float crouchSpeed = 2f;
    public float slideSpeed = 10f;
    public float slideTime = 0.7f;
    public float jumpHeight = 1.5f;
    public float gravity = -20f;

    CharacterController cc;
    float standHeight;
    float yVelocity;
    Vector3 slideDir;
    float slideTimer;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        standHeight = cc.height;
    }

    void Update()
    {
        if (IsSpawned && !IsOwner) return;
        Keyboard k = Keyboard.current;

        float x = (k.dKey.isPressed ? 1 : 0) - (k.aKey.isPressed ? 1 : 0);
        float z = (k.wKey.isPressed ? 1 : 0) - (k.sKey.isPressed ? 1 : 0);
        Vector3 dir = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0) * new Vector3(x, 0, z).normalized;

        bool grounded = cc.isGrounded;
        bool crouching = k.leftCtrlKey.isPressed;

        if (grounded && dir != Vector3.zero && k.leftCtrlKey.wasPressedThisFrame)
        {
            slideDir = dir;
            slideTimer = slideTime;
        }
        if (!crouching) slideTimer = 0;

        cc.height = crouching ? standHeight / 2f : standHeight;
        cc.center = new Vector3(0, cc.height / 2f, 0);

        Vector3 move;
        if (slideTimer > 0)
        {
            slideTimer -= Time.deltaTime;
            move = slideDir * Mathf.Lerp(crouchSpeed, slideSpeed, slideTimer / slideTime);
        }
        else
        {
            move = dir * (crouching ? crouchSpeed : speed);
        }

        if (grounded && yVelocity < 0) yVelocity = -2f;
        if (grounded && k.spaceKey.wasPressedThisFrame)
            yVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        yVelocity += gravity * Time.deltaTime;

        cc.Move((move + Vector3.up * yVelocity) * Time.deltaTime);

        if (move != Vector3.zero) transform.forward = move.normalized;
    }
}