using Fusion;
using UnityEngine;

public class SendRotation : NetworkBehaviour
{
    [SerializeField] bool sendAllRotations = false;

    public override void FixedUpdateNetwork()
    {
        if (!sendAllRotations)
            RPC_SendRotationX(transform.localRotation.x);
        else
            RPC_SendRotation(transform.localRotation);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All, InvokeLocal = false)]
    public void RPC_SendRotationX(float roatationX)
    {
        var rot = transform.localRotation;
        rot.x = roatationX;
        transform.localRotation = rot;
    }


    [Rpc(RpcSources.InputAuthority, RpcTargets.All, InvokeLocal = false)]
    public void RPC_SendRotation(Quaternion rotation)
    {
        transform.localRotation = rotation;
    }
}
