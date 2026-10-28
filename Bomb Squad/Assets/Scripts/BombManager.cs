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
        
    }
   
}
