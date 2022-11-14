using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    List<NetworkObject> pool = new List<NetworkObject>();

    public NetworkObject Get(Vector3 p, Quaternion r, Transform parent = null)
    {
        NetworkObject newt = null;

        while (pool.Count > 0 && newt == null)
        {
            var t = pool[0];
            if (t) // In case a recycled object was destroyed
            {
                Transform xform = t.transform;
                xform.SetParent(parent, false);
                xform.position = p;
                xform.rotation = r;
                newt = t;
            }
            else
            {
                Debug.LogWarning("Recycled object was destroyed - not re-using!");
            }

            pool.RemoveAt(0);
        }

        return newt;
    }

    public void ReturnToPool(NetworkObject obj) => pool.Add(obj);

}
