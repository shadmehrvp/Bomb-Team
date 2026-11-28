using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float turnSpeed = 720f;
    [SerializeField] private float accleration = 60f;
    [SerializeField] private float decleration = 40f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float checkDistanceFromGround = 1.1f;
    [SerializeField] private LayerMask groundLayer ;
    [SerializeField] private float bombHolderSpeedMultiplier = 1.2f;
    private Rigidbody rb;
    private Vector3 input;
    private bool jumpQueued ; 
    private bool isGrounded;



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
        if (kb.spaceKey.wasPressedThisFrame)
        {
           jumpQueued = true;
        }


        float cameraYaw = Camera.main != null ? Camera.main.transform.eulerAngles.y : 0f;
        input = Quaternion.Euler(0f , cameraYaw , 0f) * raw.normalized;
    }



    private void FixedUpdate()
    {
        if(!IsOwner) return;
        if(Physics.Raycast(transform.position , Vector3.down , checkDistanceFromGround , groundLayer))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        Vector3 horizontal = rb.linearVelocity;
        horizontal.y = 0f;
        float currentSpeed = moveSpeed;
        if(BombManager.instance != null && BombManager.instance.HolderId.Value == OwnerClientId)
        {
            currentSpeed *= bombHolderSpeedMultiplier ;
        }
        Vector3 targetVelocity = input * currentSpeed;
        float rate;
         if(input.sqrMagnitude> 0.1f)
        {
          rate = accleration;  
        }
        else
        {
            rate = decleration;
        }
        horizontal = Vector3.MoveTowards(horizontal , targetVelocity , rate * Time.fixedDeltaTime);
        Vector3 velocity = horizontal; 
        velocity.y = rb.linearVelocity.y;
        if(jumpQueued == true && isGrounded == true)
        {
            velocity.y = jumpForce;
            
        }
        jumpQueued = false;
        rb.linearVelocity = velocity;

        if(input.sqrMagnitude> 0.01f)
        {
            Quaternion target = Quaternion.LookRotation(input);
            rb.MoveRotation(Quaternion.RotateTowards(rb.rotation , target , turnSpeed * Time.fixedDeltaTime));
        }
    }
}
