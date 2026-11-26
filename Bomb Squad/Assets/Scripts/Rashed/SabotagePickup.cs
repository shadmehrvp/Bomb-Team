using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SabotagePickup : NetworkBehaviour
{
    public int type = 1;

    static readonly List<SabotagePickup> all = new List<SabotagePickup>();

    void OnEnable() => all.Add(this);
    void OnDisable() => all.Remove(this);

    public static SabotagePickup FindNear(Vector3 position, float range)
    {
        SabotagePickup best = null;
        float bestDistance = range;
        foreach (SabotagePickup p in all)
        {
            float d = Vector3.Distance(position, p.transform.position);
            if (d < bestDistance)
            {
                bestDistance = d;
                best = p;
            }
        }
        return best;
    }
}