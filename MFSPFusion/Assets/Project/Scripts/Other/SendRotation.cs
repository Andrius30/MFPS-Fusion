using Fusion;

public class SendRotation : NetworkBehaviour
{


    public override void FixedUpdateNetwork()
    {
        RPC_SendRotation(transform.localRotation.x);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All, InvokeLocal = false)]
    public void RPC_SendRotation(float roatationX)
    {
        var rot = transform.localRotation;
        rot.x = roatationX;
        transform.localRotation = rot;
    }
}
