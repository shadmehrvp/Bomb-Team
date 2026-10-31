using System;
using Unity.Netcode;
using UnityEngine;

public class BombManager : NetworkBehaviour
{
    public static BombManager instance {get ; private set;}
    public const ulong NoHolder = ulong.MaxValue;
    [SerializeField] private float fuseTime = 20f;
    [SerializeField] private float passrange = 2f;
    [SerializeField] private float passCooldown = 0.5f;

    public NetworkVariable<ulong> HolderId = new NetworkVariable<ulong>(NoHolder);
    public NetworkVariable<bool> IsArmed = new NetworkVariable<bool>(false);
    public NetworkVariable<double> ExploadAt = new NetworkVariable<double>(0);
    public event Action<ulong> OnExploded ;
    private float lastPassTime = -999f;
    public double TimeLeft => Math.Max(0 , ExploadAt.Value - NetworkManager.ServerTime.Time);
    public override void OnNetworkSpawn() => instance = this;


    public override void OnNetworkDespawn()
    {
       if(instance == this ) instance = null;
    }

    private void Update()
    {
        if(!IsServer || !IsArmed.Value) return;
        if(NetworkManager.ServerTime.Time >= ExploadAt.Value)
        Expload();
        
    }

    public void Arm(ulong Id)
    {
        if(!IsServer) return;
        HolderId.Value = Id;
        ExploadAt.Value = NetworkManager.ServerTime.Time + fuseTime;
        IsArmed.Value = true;
        lastPassTime = Time.time;
        Debug.Log("ARMED: " + Id);


    }

    private void Expload()
    {
        ulong victim = HolderId.Value;
        IsArmed.Value = false;
        HolderId.Value = NoHolder;
        OnExploded?.Invoke(victim);
    }

    public bool TryPass(ulong SenderId  , out string reason)
    {
        reason = null;
        if (!IsServer) {reason = "I'm not the server"; return false ;}
        if(!IsArmed.Value) { reason = " No round"; return false; }
        if(SenderId != HolderId.Value) {reason = " You don't have it"; return false;}
        if(Time.time- lastPassTime < passCooldown) {reason = " Too fast"; return false;}
        Transform sender = NetworkManager.ConnectedClients[SenderId].PlayerObject.transform;
        ulong target = FindClossest(SenderId, sender.position);

        if(target == NoHolder) {reason = " nobody close" ; return false;}

        HolderId.Value = target;
        lastPassTime = Time.time;
        return true; 

        
        
    }

    private ulong FindClossest(ulong excludeId, Vector3 from)
    {
        ulong best = NoHolder;
        float bestDistance = passrange;
        foreach(NetworkClient client in NetworkManager.ConnectedClientsList)
        {
            if(client.ClientId == excludeId || client.PlayerObject == null ) continue;
            float d = Vector3.Distance(from , client.PlayerObject.transform.position);
            if(d < bestDistance)
            {
                bestDistance = d;
                best = client.ClientId;
            }

        }
        return best;
    }
}
