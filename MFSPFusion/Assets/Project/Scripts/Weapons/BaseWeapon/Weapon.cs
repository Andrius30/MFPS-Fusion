using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public WeaponData data;

    public abstract void Attack();

    public void Update()
    {
        Attack();
    }

}
