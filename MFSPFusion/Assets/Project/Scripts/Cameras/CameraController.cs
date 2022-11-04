using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static Action<Transform> onTargetSpawn;

    Transform target;

    void LateUpdate()
    {
        transform.position = target.position;
        transform.rotation = target.rotation;
    }

    void SetTarget(Transform target) => this.target = target;

    void OnEnable()
    {
        onTargetSpawn += SetTarget;
    }
    void OnDisable()
    {
        onTargetSpawn -= SetTarget;
        
    }
}
