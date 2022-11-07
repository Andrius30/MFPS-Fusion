using UnityEngine;

public class PlayerMove
{
    PlayerController controller;

    Vector3 direction;

    public PlayerMove(PlayerController controller)
    {
        this.controller = controller;
    }

    public void Move(NetworkInputs input)
    {
        direction = Vector3.zero;
        var forward = input.buttons.IsSet(MyButtons.Forward);
        var backwards = input.buttons.IsSet(MyButtons.Backward);
        var right = input.buttons.IsSet(MyButtons.Right);
        var left = input.buttons.IsSet(MyButtons.Left);

        if (forward) direction.z += 1f;
        if (backwards) direction.z -= 1f;
        if (right) direction.x += 1f;
        if (left) direction.x -= 1f;

        controller.moveDirection = (controller.transform.forward * direction.z + controller.transform.right * direction.x).normalized;
        controller.transform.position += controller.moveDirection * FusionCallbacks.runner.DeltaTime * controller.currentSpeed;

    }

    public void Sprint(NetworkInputs input)
    {
        if (input.buttons.IsSet(MyButtons.LeftShiftHolding) && input.buttons.IsSet(MyButtons.Forward))
        {
            controller.ChangeState(PlayerController.PlayerStates.SPRINT);
        }
        else if (input.buttons.IsSet(MyButtons.LeftShiftReleased))
        {
            controller.ChangeState(PlayerController.PlayerStates.NORMAL);
        }
    }
}
