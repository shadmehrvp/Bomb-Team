using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Services.Leaderboards;
public class RoundManager : NetworkBehaviour
{
    public static RoundManager instance { get ; private set;}
    public NetworkVariable<int> RoundNumber = new NetworkVariable<int>(0);
    [SerializeField] private int minimumPlayers = 3;
    [SerializeField] private float delayBetweenBombs = 3f;
    [SerializeField] private float startDelay = 5f; // Rashed changes
    private double startAt = -1; // Rashed changes
    private bool isRoundStarted;
    private readonly List<ulong> alive = new List<ulong>();

    public override void OnNetworkSpawn() => instance = this;

    private void Update()
    {
        if (!IsServer) return;
        if (isRoundStarted) return;
        if (BombManager.instance == null) return;
        if (NetworkManager.ConnectedClientsList.Count < minimumPlayers)
        {
            startAt = -1; // Rashed changes
            return; // Rashed changes
        }

        if (startAt < 0) // Rashed changes
        {
            startAt = NetworkManager.ServerTime.Time + startDelay; // Rashed changes
            if (CountdownManager.instance != null) CountdownManager.instance.Show(1, startDelay); // Rashed changes
        }
        if (NetworkManager.ServerTime.Time < startAt) return; // Rashed changes

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
        { // Rashed changes
            if (CountdownManager.instance != null) CountdownManager.instance.Show(2, delayBetweenBombs); // Rashed changes
            Invoke(nameof(GiveBombToRandomPlayer), delayBetweenBombs);
        } // Rashed changes
        else if (alive.Count == 1)
            {
                Debug.Log("WINNER: " + alive[0]);
            WinRpc(RpcTarget.Single(alive[0], RpcTargetUse.Temp));
            }
        else
            Debug.Log("No winner");
        

         
    }

    private void GiveBombToRandomPlayer()
    {
        if (alive.Count == 0) return;
        RoundNumber.Value ++;
        ulong randomClientId = alive[Random.Range(0, alive.Count)];
        BombManager.instance.Arm(randomClientId);

    }
  
  [Rpc(SendTo.SpecifiedInParams)]
    private void WinRpc(RpcParams rpcParams)
    {
          SubmitWin();
    }

    private async void SubmitWin()
    {
        try
        {
            await LeaderboardsService.Instance.AddPlayerScoreAsync("Wins" , 1);
            Debug.Log("Wins added to leaderboard");
            
        }
        catch(System.Exception e)
        {
            Debug.LogException(e);
        }
    }
}