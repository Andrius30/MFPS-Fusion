using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.Windows;

public class PlayerClimb
{
    PlayerController controller;
    bool wallInFront;

    RaycastHit hit;
    Rigidbody rb;
    float currentAngle;

    public PlayerClimb(PlayerController controller)
    {
        this.controller = controller;
        rb = controller.GetComponent<Rigidbody>();
    }

    public void ClimbChecks(NetworkInputs input)
    {
        Check();
        CheckAngle();
        if (wallInFront && input.buttons.IsSet(MyButtons.Forward) && currentAngle < controller.maxClimbAngle)
        {
            if (!controller.isClimbing)
            {
                controller.isClimbing = true;
                controller.ChangeState(PlayerController.PlayerStates.WALL_CLIMB);
            }
        }
        else
        {
            if (controller.isClimbing) StopClimbing();
        }
    }
    public void Climb(NetworkInputs input)
    {
        //if (wallInFront && input.buttons.IsSet(MyButtons.Forward) && currentAngle < controller.maxClimbAngle)
        //{
        //    if (!controller.isClimbing)
        //    {
        //        StartClimbing();
        //    }
        //}
        if (controller.isClimbing)
        {
            ClimbMovement();
        }
    }

    void Check() => wallInFront = Physics.SphereCast(controller.transform.position, controller.wallClimbCheckRadius, controller.cameraTransform.forward, out hit, controller.wallClimbDetectionLength, controller.wallMask);
    void CheckAngle() => currentAngle = Vector3.Angle(controller.cameraTransform.forward, -hit.normal);
    //void StartClimbing()
    //{
    //    controller.isClimbing = true;
    //}
    void ClimbMovement()
    {
        rb.velocity = new Vector3(rb.velocity.x, controller.wallClimbSpeed, rb.velocity.z);

    }
    void StopClimbing()
    {
        controller.isClimbing = false;
        controller.ChangeState(PlayerController.PlayerStates.NORMAL);
    }
    public void Visualize()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(controller.cameraTransform.position, (controller.cameraTransform.position + controller.cameraTransform.forward) * controller.wallClimbDetectionLength);
        Gizmos.DrawWireSphere((controller.cameraTransform.position + controller.cameraTransform.forward) * controller.wallClimbDetectionLength, controller.wallClimbCheckRadius);
    }
}
