
using UnityEngine;
using UnityEngine.Windows;

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

    public void Jump(NetworkInputs input)
    {
        var pos = rb.velocity;
        HangTimer();
        JumpBuffer(input);
        pos = Jumping(pos, input);
        pos = Gravity(pos);
        rb.velocity = pos;
    }

    Vector3 Gravity(Vector3 pos)
    {
        if (!controller.useGravity) return rb.velocity;
        if (!controller.groundCheck.IsGrounded())
        {
            pos.y += controller.gravity * FusionCallbacks.runner.DeltaTime;
        }
        return pos;
    }
    Vector3 Jumping(Vector3 pos, NetworkInputs input)
    {
        if (jumpBufferCount >= 0 && hangCount > 0)
        {
            controller.exitingSlope = true;
            pos.y = controller.jumpHeight;
            jumpBufferCount = 0;
            hangCount = 0;
        }
        if (input.buttons.IsSet(MyButtons.SpaceReleased) && rb.velocity.y > 0)
        {
            pos = new Vector3(pos.x, pos.y * .5f, pos.z);
            controller.exitingSlope = false;
        }
        return pos;
    }
    void JumpBuffer(NetworkInputs input)
    {
        if (input.buttons.IsSet(MyButtons.Jump))
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
        }
        else
        {
            hangCount -= Time.deltaTime;
        }
    }
}
