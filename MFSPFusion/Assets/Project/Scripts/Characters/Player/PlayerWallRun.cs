using UnityEngine;

public class PlayerWallRun
{
    PlayerController controller;
    Rigidbody rb;

    public PlayerWallRun(PlayerController controller)
    {
        this.controller = controller;
        rb = controller.GetComponent<Rigidbody>();
    }

    public void Run()
    {
        Check();
        StateMachine();
        if (controller.wallRunning)
        {
            WallRunMovement();
        }
    }

    void Check()
    {
        controller.wallLeft = Physics.Raycast(controller.transform.position, -controller.cameraPosTransform.right, out controller.leftWallHit, controller.wallCheckDistance, controller.wallMask);
        controller.wallRight = Physics.Raycast(controller.transform.position, controller.cameraPosTransform.right, out controller.rightWallHit, controller.wallCheckDistance, controller.wallMask);
    }
    void StateMachine()
    {
        if ((controller.wallLeft || controller.wallRight) && controller.Inputs.GetVertical() > 0 && !controller.groundCheck.IsGrounded())
        {
            if (!controller.wallRunning)
            {
                StartWallRun();
            }
        }
        else
        {
            if (controller.wallRunning)
            {
                StopWallRun();
            }
        }
    }
    void StartWallRun()
    {
        controller.wallRunning = true;
        controller.ChangeState(PlayerController.PlayerStates.WALL_RUNNING);
    }
    void WallRunMovement()
    {
        controller.useGravity = false;
        Vector3 wallNormal = controller.wallRight ? controller.rightWallHit.normal : controller.leftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, controller.transform.up);

        if ((controller.cameraPosTransform.forward - wallForward).magnitude > (controller.cameraPosTransform.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        rb.AddForce(wallForward * controller.wallRunSpeed, ForceMode.Force);
        //if (!(controller.wallLeft && controller.Inputs.GetHorizontal() > 0) && !(controller.wallRight && controller.Inputs.GetHorizontal() < 0))
        //{
        //    Debug.Log($"Push to wall");
        //    rb.AddForce(-wallNormal * 100, ForceMode.Force);
        //}
    }
    void StopWallRun()
    {
        controller.wallRunning = false;
        controller.useGravity = true;

    }
    public void Visualize()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(controller.transform.position, controller.transform.position + -controller.cameraPosTransform.right * controller.wallCheckDistance);
        Gizmos.DrawLine(controller.transform.position, controller.transform.position + controller.cameraPosTransform.right * controller.wallCheckDistance);

    }
}
