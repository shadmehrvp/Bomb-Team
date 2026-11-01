using Unity.Collections;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float turnSpeed = 720f;
    private Rigidbody rb;
    private Vector3 input;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        if(!IsOwner) return;
        Keyboard kb = Keyboard.current;
        if(kb == null) return;
        Vector3 raw = Vector3.zero;
        if(kb.aKey.isPressed) raw.x -= 1;
        if(kb.dKey.isPressed) raw.x += 1;
        if(kb.wKey.isPressed) raw.z += 1;
        if(kb.sKey.isPressed) raw.z -= 1;

        float cameraYaw = Camera.main != null ? Camera.main.transform.eulerAngles.y : 0f;
        input = Quaternion.Euler(0f , cameraYaw , 0f) * raw.normalized;
    }

    private void FixedUpdate()
    {
        if(!IsOwner) return;

        Vector3 velocity = input * moveSpeed;
        velocity.y = rb.linearVelocity.y;
        rb.linearVelocity = velocity;

        if(input.sqrMagnitude> 0.01f)
        {
            Quaternion target = Quaternion.LookRotation(input);
            rb.MoveRotation(Quaternion.RotateTowards(rb.rotation , target , turnSpeed * Time.fixedDeltaTime));
        }
    }
}
