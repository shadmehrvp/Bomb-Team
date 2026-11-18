using Unity.Netcode;
using UnityEngine;

public class PlayerState : NetworkBehaviour
{
    public NetworkVariable<bool> IsAlive = new NetworkVariable<bool>(true);

    public override void OnNetworkSpawn()
    {
        IsAlive.OnValueChanged += OnAliveChanged;
        if (!IsAlive.Value) Eliminate();
    }

    public override void OnNetworkDespawn()
    {
        IsAlive.OnValueChanged -= OnAliveChanged;
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