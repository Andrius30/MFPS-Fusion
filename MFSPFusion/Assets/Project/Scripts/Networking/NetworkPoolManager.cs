using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPoolManager : MonoBehaviour, INetworkObjectPool
{
    Dictionary<object, ObjectPool> pools = new Dictionary<object, ObjectPool>();
    Dictionary<NetworkObject, ObjectPool> poolsByInstance = new Dictionary<NetworkObject, ObjectPool>();

    public ObjectPool GetPool<T>(T prefab) where T : NetworkObject
    {
        ObjectPool pool;
        if (!pools.TryGetValue(prefab, out pool))
        {
            pool = new ObjectPool();
            pools[prefab] = pool;
        }
        return pool;
    }

    public NetworkObject AcquireInstance(NetworkRunner runner, NetworkPrefabInfo info)
    {
        NetworkObject prefab;
        if (NetworkProjectConfig.Global.PrefabTable.TryGetPrefab(info.Prefab, out prefab))
        {
            ObjectPool objectPool = GetPool(prefab);
            NetworkObject obj = objectPool.Get(Vector3.zero, Quaternion.identity);

            if (obj == null)
            {
                Debug.Log($"Obj null! Create new obj");
                obj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                poolsByInstance[obj] = objectPool;
            }
            obj.gameObject.SetActive(true);
            return obj;
        }
        Debug.LogError($"No prefab for {info.Prefab}");
        return null;
    }

    public void ReleaseInstance(NetworkRunner runner, NetworkObject instance, bool isSceneObject)
    {
        Debug.Log($"Releasing {instance} instance, isSceneObject={isSceneObject}");
        if (instance != null)
        {
            ObjectPool pool;
            if (poolsByInstance.TryGetValue(instance, out pool))
            {
                pool.ReturnToPool(instance);
                instance.gameObject.SetActive(false);
                instance.transform.SetParent(transform, false);
            }
            else
            {
                instance.gameObject.SetActive(false);
                instance.transform.SetParent(null, false);
                Destroy(instance.gameObject);
            }
        }
        instance.gameObject.SetActive(false);
    }
}
