using Unity.Netcode;
using UnityEngine;

public class PlayerState : NetworkBehaviour
{
    public NetworkVariable<bool> IsAlive = new NetworkVariable<bool>(true);
    public NetworkVariable<double> StunnedUntil = new NetworkVariable<double>(0);
    public NetworkVariable<int> StunType = new NetworkVariable<int>(0);

    Movement movementScript;
    Rigidbody body;
    Renderer rend;
    Color normalColor;

    public bool IsStunned => NetworkManager.ServerTime.Time < StunnedUntil.Value;

    public override void OnNetworkSpawn()
    {
        movementScript = GetComponent<Movement>();
        body = GetComponent<Rigidbody>();
        rend = GetComponentInChildren<Renderer>();
        if (rend != null) normalColor = rend.material.color;

        IsAlive.OnValueChanged += OnAliveChanged;
        if (!IsAlive.Value) Eliminate();
    }

    public override void OnNetworkDespawn()
    {
        IsAlive.OnValueChanged -= OnAliveChanged;
    }

    void Update()
    {
        if (!IsSpawned || !IsAlive.Value) return;

        bool stunned = IsStunned;
        if (movementScript != null) movementScript.enabled = !stunned;

        if (stunned && IsOwner && body != null && !body.isKinematic)
            body.linearVelocity = new Vector3(0f, body.linearVelocity.y, 0f);

        if (rend != null)
            rend.material.color = stunned ? (StunType.Value == 1 ? Color.cyan : Color.yellow) : normalColor;
    }

    public void Stun(float seconds, int type)
    {
        if (!IsServer) return;
        StunnedUntil.Value = NetworkManager.ServerTime.Time + seconds;
        StunType.Value = type;
    }

    private void OnAliveChanged(bool oldValue, bool newValue)
    {
        if (!newValue) Eliminate();
    }

    private void Eliminate()
    {
        foreach (Renderer r in GetComponentsInChildren<Renderer>()) r.enabled = false;
        foreach (Collider c in GetComponentsInChildren<Collider>()) c.enabled = false;
        

        Movement movement = GetComponent<Movement>();
        if (movement != null) movement.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if(rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }
}