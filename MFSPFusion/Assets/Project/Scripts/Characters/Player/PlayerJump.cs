
using UnityEngine;

public class PlayerJump
{
    PlayerController controller;
    Rigidbody rb;
    float hangTime = 0.2f; // lets the player jump after grounded check returns false with 200 miliseconds delay
    float hangCount;
    float jumpBufferLength = .1f;
    float jumpBufferCount;

    public PlayerJump(PlayerController controller)
    {
        this.controller = controller;
        rb = controller.GetComponent<Rigidbody>();
    }

    public void Jump()
    {
        var pos = rb.velocity;
        HangTimer();
        JumpBuffer();
        pos = Jumping(pos);
        pos = Gravity(pos);
        rb.velocity = pos;
    }

    Vector3 Gravity(Vector3 pos)
    {
        if (!controller.useGravity) return rb.velocity;
        if (!controller.groundCheck.IsGrounded())
        {
            pos.y += controller.gravity * Time.deltaTime;
        }
        return pos;
    }
    Vector3 Jumping(Vector3 pos)
    {
        if (jumpBufferCount >= 0 && hangCount > 0)
        {
            pos.y = controller.jumpHeight;
            jumpBufferCount = 0;
            hangCount = 0;
            controller.ChangeState(PlayerController.PlayerStates.JUMPING);
        }
        if (controller.Inputs.SpaceReleased() && rb.velocity.y > 0)
        {
            pos = new Vector3(pos.x, pos.y * .5f, pos.z);
        }
        return pos;
    }
    void JumpBuffer()
    {
        if (controller.Inputs.SpacePressed())
        {
            jumpBufferCount = jumpBufferLength;
        }
        else
        {
            jumpBufferCount -= Time.deltaTime;
        }
    }
    void HangTimer()
    {
        if (controller.groundCheck.IsGrounded())
        {
            hangCount = hangTime;
            controller.ChangeState(PlayerController.PlayerStates.NORMAL);
        }
        else
        {
            hangCount -= Time.deltaTime;
        }
    }
}
