using UnityEngine;

public class KnifeAnimationEvents : MonoBehaviour
{
    [SerializeField] BoxCollider weaponTriggerCollider;

    public void KnifeAttack()
    {
        weaponTriggerCollider.enabled = true;
    }
    public void KnifeAttackEnd()
    {
        weaponTriggerCollider.enabled = false;
    }
}
