using UnityEngine;

public class Rifle : BaseGun
{



    public void Attack()
    {
        Debug.Log($"Rifle attack Not implemented");
    }

    public override void Reload()
    {
        throw new System.NotImplementedException();
    }
}
