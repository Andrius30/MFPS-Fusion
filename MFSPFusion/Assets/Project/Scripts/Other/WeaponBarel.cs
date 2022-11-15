using Fusion;

public class WeaponBarel : NetworkBehaviour
{
    public int barelID;

    public override void Spawned()
    {
        barelID = Object.InputAuthority.PlayerId;
    }
}
