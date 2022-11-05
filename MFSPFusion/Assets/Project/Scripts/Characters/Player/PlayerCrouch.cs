using UnityEngine;

public class PlayerCrouch
{
    PlayerController controller;
    Rigidbody rb;
    float startCrouchScale;
    bool isCrouching = false;

    public PlayerCrouch(PlayerController controller)
    {
        this.controller = controller;
        startCrouchScale = controller.transform.localScale.y;
        rb = controller.GetComponent<Rigidbody>();
    }

    public void CrouchInputs()
    {
        if (controller.Inputs.LeftCtrlHolding())
        {
            isCrouching = true;
        }
        else if (controller.Inputs.LeftCtrlReleased())
        {
            isCrouching = false;
            controller.transform.localScale = new Vector3(controller.transform.localScale.x, startCrouchScale, controller.transform.localScale.z);
            controller.ChangeState(PlayerController.PlayerStates.NORMAL);
        }
    }
    public void Crouch() => ScaleByHalf();

    void ScaleByHalf()
    {
        if (!isCrouching) return;
        controller.transform.localScale = new Vector3(controller.transform.localScale.x, controller.playerCrouchScale, controller.transform.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        controller.ChangeState(PlayerController.PlayerStates.CROUCH);
    }
}
