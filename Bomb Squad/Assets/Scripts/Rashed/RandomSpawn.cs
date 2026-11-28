using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class RandomSpawn : NetworkBehaviour
{
    SpawnPoint point;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        point = SpawnPoint.PickRandom();
        if (point == null) return;

        MoveToRpc(point.transform.position, point.transform.rotation);
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer && point != null) point.taken = false;
    }

    [Rpc(SendTo.Owner)]
    void MoveToRpc(Vector3 position, Quaternion rotation)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.position = position;
            rb.rotation = rotation;
        }

        transform.SetPositionAndRotation(position, rotation);

        NetworkTransform nt = GetComponent<NetworkTransform>();
        if (nt != null && nt.CanCommitToTransform)
            nt.Teleport(position, rotation, transform.localScale);
    }
}
