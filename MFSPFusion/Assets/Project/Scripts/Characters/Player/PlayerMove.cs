using UnityEngine;

public class PlayerMove
{
    PlayerController controller;
    Rigidbody rb;
    float horizontal;
    float vertical;

    public PlayerMove(PlayerController controller)
    {
        this.controller = controller;
        rb = controller.GetComponent<Rigidbody>();
    }

    public void Move()
    {
        horizontal = controller.Inputs.GetHorizontal();
        vertical = controller.Inputs.GetVertical();

        controller.transform.position += (controller.transform.forward * vertical + controller.transform.right * horizontal).normalized * Time.deltaTime * controller.PlayerSpeed;

    }
}
