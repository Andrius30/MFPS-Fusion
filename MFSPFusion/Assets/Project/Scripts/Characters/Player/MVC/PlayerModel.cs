using UnityEngine;

public class PlayerModel
{
    PlayerController controller;
    float xRotation;
    float yRotation;


    public PlayerModel(PlayerController controller)
    {
        this.controller = controller;
    }

    public void RotatePlayer(NetworkInputs input)
    {
        float mouseX = input.mousex * FusionCallbacks.runner.DeltaTime * controller.MouseSensitivity;
        float mouseY = input.mousey * FusionCallbacks.runner.DeltaTime * controller.MouseSensitivity;

        yRotation = mouseX;
        xRotation += mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        controller.cameraTransform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        controller.transform.Rotate(0, yRotation, 0);

    }
}
