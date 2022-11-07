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
    bool isClimbing = false;

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
            if (!isClimbing)
            {
                controller.ChangeState(PlayerController.PlayerStates.WALL_CLIMB);
            }
        }
        else
        {
            if (isClimbing) StopClimbing();
        }
    }
    public void Climb(NetworkInputs input)
    {
        if (wallInFront && input.buttons.IsSet(MyButtons.Forward) && currentAngle < controller.maxClimbAngle)
        {
            if (!isClimbing)
            {
                StartClimbing();
            }
        }
        if (isClimbing)
        {
            ClimbMovement();
        }
    }

    void Check() => wallInFront = Physics.SphereCast(controller.transform.position, controller.wallClimbCheckRadius, controller.cameraPosTransform.forward, out hit, controller.wallClimbDetectionLength, controller.wallMask);
    void CheckAngle() => currentAngle = Vector3.Angle(controller.cameraPosTransform.forward, -hit.normal);
    void StartClimbing()
    {
        isClimbing = true;
    }
    void ClimbMovement()
    {
        rb.velocity = new Vector3(rb.velocity.x, controller.wallClimbSpeed, rb.velocity.z);

    }
    void StopClimbing()
    {
        isClimbing = false;
        controller.ChangeState(PlayerController.PlayerStates.NORMAL);
    }
    public void Visualize()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(controller.cameraPosTransform.position, (controller.cameraPosTransform.position + controller.cameraPosTransform.forward) * controller.wallClimbDetectionLength);
        Gizmos.DrawWireSphere((controller.cameraPosTransform.position + controller.cameraPosTransform.forward) * controller.wallClimbDetectionLength, controller.wallClimbCheckRadius);
    }
}
