using Fusion;
using System;
using UnityEngine;

public class WeaponSway : NetworkBehaviour
{
    [Header("Tilt sway")]
    [SerializeField] float amount;
    [SerializeField] float maxSway;
    [SerializeField] float smoothAmount;

    [Header("Rotational sway")]
    [SerializeField] float tiltAmount;
    [SerializeField] float maxTiltSway;
    [SerializeField] float smoothAmountTilt;
    [SerializeField] bool tiltDirX, tiltDirY, tiltDirZ;

    Vector3 initialPos;
    Quaternion initialRot;

    void Start()
    {
        initialPos = transform.localPosition;
        initialRot = transform.localRotation;
    }
    public override void FixedUpdateNetwork()
    {
        if (GetInput<NetworkInputs>(out var input) == false) return;
        TiltSway(input);
        RotationalSway(input);
    }

    void TiltSway(NetworkInputs input)
    {
        float moveX = input.mousex * amount;
        float moveY = input.mousey * amount;
        moveX = Mathf.Clamp(moveX, -maxSway, maxSway);
        Vector3 finalPos = new Vector3(-moveX, 0, moveY);
        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPos + initialPos, Runner.DeltaTime * smoothAmount);
    }
    void RotationalSway(NetworkInputs input)
    {
        float tiltY = input.mousex * tiltAmount;
        float tiltX = input.mousey * tiltAmount;

        tiltY = Mathf.Clamp(tiltY, -maxTiltSway, maxTiltSway);
        tiltX = Mathf.Clamp(tiltX, -maxTiltSway, maxTiltSway);

        Quaternion finalRot = Quaternion.Euler(new Vector3(tiltDirX ? -tiltX : 0, tiltDirY ? tiltY : 0, tiltDirZ ? tiltY : 0));
        transform.localRotation = Quaternion.Slerp(transform.localRotation, finalRot * initialRot, Runner.DeltaTime * smoothAmountTilt);
    }

}
