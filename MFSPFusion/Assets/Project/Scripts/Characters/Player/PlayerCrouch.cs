using UnityEngine;

public class PlayerCrouch
{
    PlayerController controller;
    Rigidbody rb;
    float startCrouchScale;

    public PlayerCrouch(PlayerController controller)
    {
        this.controller = controller;
        startCrouchScale = controller.transform.localScale.y;
        rb = controller.GetComponent<Rigidbody>();
    }


    public void Crouch()
    {
        ScaleByHalf();


    }

    void ScaleByHalf()
    {
        if (controller.Inputs.LeftCtrlHolding())
        {
            controller.transform.localScale = new Vector3(controller.transform.localScale.x, controller.playerCrouchScale, controller.transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            controller.ChangeState(PlayerController.PlayerStates.CROUCH);
        }
        else if(controller.Inputs.LeftCtrlReleased())
        {
            controller.transform.localScale = new Vector3(controller.transform.localScale.x, startCrouchScale, controller.transform.localScale.z);
            controller.ChangeState(PlayerController.PlayerStates.NORMAL);
        }
    }
}
