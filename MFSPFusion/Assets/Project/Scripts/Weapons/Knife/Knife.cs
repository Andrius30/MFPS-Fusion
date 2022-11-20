using Fusion;
using UnityEngine;

public class Knife : Weapon
{
    public float coolDown;

    int attackAnim = Animator.StringToHash("Attack");


    public override void Attack()
    {
        if (GetInput<NetworkInputs>(out var input) == false) return;

        if (input.buttons.IsSet(MyButtons.Fire) && cooldownTimer.ExpiredOrNotRunning(Runner))
        {
            if(animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }
            animator.SetTrigger(attackAnim);
            cooldownTimer = TickTimer.CreateFromSeconds(Runner, coolDown);
        }
    }


}
