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

        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null) movement.enabled = false;
    }
}