using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 7f;
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
        input = Vector3.zero;
        if(kb.aKey.isPressed) input.x -= 1;
        if(kb.dKey.isPressed) input.x += 1;
        if(kb.wKey.isPressed) input.z += 1;
        if(kb.sKey.isPressed) input.z -= 1;
        input = input.normalized;
    }

    private void FixedUpdate()
    {
        if(!IsOwner) return;
        Vector3 velocity = input * moveSpeed;
        velocity.y = rb.linearVelocity.y;
        rb.linearVelocity = velocity;
    }
}
