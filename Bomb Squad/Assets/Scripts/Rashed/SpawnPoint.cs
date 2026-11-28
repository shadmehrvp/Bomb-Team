using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public bool taken;

    static readonly List<SpawnPoint> all = new List<SpawnPoint>();

    void OnEnable() => all.Add(this);
    void OnDisable() => all.Remove(this);

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }

    public static SpawnPoint PickRandom()
    {
        if (all.Count == 0) return null;

        List<SpawnPoint> free = all.FindAll(p => !p.taken);
        List<SpawnPoint> pool = free.Count > 0 ? free : all;

        SpawnPoint chosen = pool[Random.Range(0, pool.Count)];
        chosen.taken = true;
        return chosen;
    }
}
