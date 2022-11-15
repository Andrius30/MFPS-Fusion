using Fusion;

public class WeaponEffects : NetworkBehaviour
{

    public override void Spawned()
    {
        if (Runner.IsServer) return;
        foreach (var barel in FindObjectsOfType<WeaponBarel>())
        {
            if(barel.barelID == Object.InputAuthority.PlayerId)
            {
                transform.SetParent(barel.transform);
                transform.localPosition = barel.transform.position;
                transform.rotation = barel.transform.rotation;
            }
        }
    }

}
