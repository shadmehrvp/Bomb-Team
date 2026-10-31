using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class RoundManager : NetworkBehaviour
{
    [SerializeField] private int minimumPlayers = 2;
    [SerializeField] private float delayBetweenBombs = 3f;
    private bool isRoundStarted;
    private readonly List<ulong> alive = new List<ulong>();

    private void Update()
    {
        if (!IsServer) return;
        if (isRoundStarted) return;
        if (BombManager.instance == null) return;
        if (NetworkManager.ConnectedClientsList.Count < minimumPlayers) return;

        alive.Clear();
        foreach (NetworkClient client in NetworkManager.ConnectedClientsList)
            alive.Add(client.ClientId);

        BombManager.instance.OnExploded += HandleExploded;
        isRoundStarted = true;
        GiveBombToRandomPlayer();
    }

    private void HandleExploded(ulong victim)
    {
        alive.Remove(victim);

        if (NetworkManager.ConnectedClients.TryGetValue(victim, out NetworkClient client) && client.PlayerObject != null)
        {
            PlayerState state = client.PlayerObject.GetComponent<PlayerState>();
            if (state != null) state.IsAlive.Value = false;
        }

        if (alive.Count > 1)
            Invoke(nameof(GiveBombToRandomPlayer), delayBetweenBombs);
        else if (alive.Count == 1)
            Debug.Log("WINNER: " + alive[0]);
        else
            Debug.Log("No winner");
    }

    private void GiveBombToRandomPlayer()
    {
        if (alive.Count == 0) return;
        ulong randomClientId = alive[Random.Range(0, alive.Count)];
        BombManager.instance.Arm(randomClientId);
    }
}