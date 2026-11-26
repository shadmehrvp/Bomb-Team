using Unity.Netcode;
using UnityEngine;

public class SabotageProjectile : NetworkBehaviour
{
    [HideInInspector] public int type;
    [SerializeField] float speed = 15f;
    [SerializeField] float hitRadius = 1.2f;
    [SerializeField] float lifeTime = 3f;
    [SerializeField] float stunSeconds = 3f;

    Vector3 direction;
    float spawnTime;

    public void Launch(Vector3 dir)
    {
        direction = dir;
        spawnTime = Time.time;
    }

    void Update()
    {
        if (!IsServer) return;

        transform.position += direction * speed * Time.deltaTime;

        if (Time.time - spawnTime > lifeTime)
        {
            NetworkObject.Despawn();
            return;
        }

        BombManager bomb = BombManager.instance;
        if (bomb == null || !bomb.IsArmed.Value) return;
        if (!NetworkManager.ConnectedClients.TryGetValue(bomb.HolderId.Value, out NetworkClient client)) return;
        if (client.PlayerObject == null) return;

        if (Vector3.Distance(transform.position, client.PlayerObject.transform.position) > hitRadius) return;

        client.PlayerObject.GetComponent<PlayerState>().Stun(stunSeconds, type);
        NetworkObject.Despawn();
    }
}