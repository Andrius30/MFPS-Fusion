using UnityEngine;

public class GroundCheck
{
    Transform groundCheckTransform;
    float groundCheckRadius;
    LayerMask groundMask;

    public GroundCheck(Transform groundCheckTransform, float groundCheckRadius, LayerMask groundMask)
    {
        this.groundCheckTransform = groundCheckTransform;
        this.groundCheckRadius = groundCheckRadius;
        this.groundMask = groundMask;
    }

    public bool IsGrounded() => Physics.CheckSphere(groundCheckTransform.position, groundCheckRadius, groundMask);

    public void Visualize()
    {
        if (IsGrounded())
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }
        Gizmos.DrawWireSphere(groundCheckTransform.position, groundCheckRadius);
    }
}
