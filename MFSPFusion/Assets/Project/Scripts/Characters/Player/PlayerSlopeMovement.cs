using UnityEngine;

public class PlayerSlopeMovement
{
    PlayerController controller;
    RaycastHit hit;
    Rigidbody rb;
    float playerHeight;

    public PlayerSlopeMovement(PlayerController controller)
    {
        this.controller = controller;
        playerHeight = controller.transform.localScale.y;
        rb = controller.GetComponent<Rigidbody>();
    }


    public void SlopeMove()
    {
        bool onSlope = CheckForSlope();
        if (controller.currentState != PlayerController.PlayerStates.WALL_RUNNING)
            controller.useGravity = !onSlope;

        if (onSlope && !controller.exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * controller.currentSpeed * 20f, ForceMode.Force);
            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
            if (rb.velocity.magnitude > controller.currentSpeed)
            {
                rb.velocity = rb.velocity.normalized * controller.currentSpeed;
            }
            else // limiting speed on ground or in air
            {
                Vector3 flatVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

                // limit velocity if needed
                if (flatVelocity.magnitude > controller.currentSpeed)
                {
                    Vector3 limitedVelocity = flatVelocity.normalized * controller.currentSpeed;
                    rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
                }
            }
        }


    }

    bool CheckForSlope()
    {
        if (Physics.Raycast(controller.transform.position, Vector3.down, out hit, playerHeight * .5f + controller.slopeCheckDistance, controller.groundMask))
        {
            float angle = Vector3.Angle(Vector3.up, hit.normal);
            return angle < controller.maxSlopeAngle && angle != 0;
        }
        return false;
    }

    Vector3 GetSlopeMoveDirection() => Vector3.ProjectOnPlane(controller.moveDirection, hit.normal).normalized;

    public void Visualize()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(controller.transform.position, controller.transform.position + Vector3.down * (playerHeight * .5f + controller.slopeCheckDistance));
    }
}
