using UnityEngine;

public class PlayerMove
{
    PlayerController controller;
    float horizontal;
    float vertical;

    public PlayerMove(PlayerController controller)
    {
        this.controller = controller;
    }

    public void Move()
    {
        horizontal = controller.Inputs.GetHorizontal();
        vertical = controller.Inputs.GetVertical();

        controller.moveDirection = (controller.transform.forward * vertical + controller.transform.right * horizontal).normalized;
        controller.transform.position += controller.moveDirection * Time.deltaTime * controller.currentSpeed;

        Sprint();
    }

    void Sprint()
    {
        if (controller.Inputs.LeftShiftHolding() && vertical > 0)
        {
            controller.ChangeState(PlayerController.PlayerStates.SPRINT);
        }
        else if (controller.Inputs.LeftShiftReleased())
        {
            controller.ChangeState(PlayerController.PlayerStates.NORMAL);

        }
    }
}
