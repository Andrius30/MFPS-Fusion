using System;
using UnityEngine;

public class HeadbobController : MonoBehaviour
{
    [SerializeField] PlayerController controller;
    [SerializeField] Transform head;
    [SerializeField] Transform cameraTransform;

    [Header("Head bobbing")]
    [SerializeField] float walkingFrequency;
    [SerializeField] float runningFrequency;
    [SerializeField] float horizontalAmplitude = .1f;
    [SerializeField] float verticalAmplitude = .1f;
    [SerializeField, Range(0, 1)] float smoothing = .1f;

    public bool isWalking;
    public bool isRunning;
    [SerializeField] float walkingTime;
    [SerializeField] Vector3 targetCameraPos;

    float frequency;

    void Update()
    {
        if (!isWalking && !isRunning && !controller.groundCheck.IsGrounded()) walkingTime = 0;
        else walkingTime += Time.deltaTime;

        frequency = isWalking ? walkingFrequency : isRunning ? runningFrequency : 0;

        targetCameraPos = head.position + CalculateHeadBobOffset(walkingTime);

        cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetCameraPos, smoothing);
        if ((cameraTransform.position - targetCameraPos).magnitude <= 0.001f)
        {
            cameraTransform.position = targetCameraPos;
        }
    }

    Vector3 CalculateHeadBobOffset(float walkingTime)
    {
        float horizontalOffset = 0;
        float verticalOffset = 0;
        Vector3 offset = Vector3.zero;

        if (walkingTime > 0)
        {
            horizontalOffset = Mathf.Cos(walkingTime * frequency) * horizontalAmplitude;
            verticalOffset = Mathf.Sin(walkingTime * frequency * 2) * verticalAmplitude;
            offset = head.right * horizontalOffset + head.up * verticalOffset;
        }
        return offset;
    }
}
