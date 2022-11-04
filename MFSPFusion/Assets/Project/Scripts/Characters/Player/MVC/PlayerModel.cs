using Fusion;
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

    public void RotatePlayer()
    {
        float mouseX = controller.Inputs.GetMouseX() * Time.deltaTime * controller.MouseSensitivity;
        float mouseY = controller.Inputs.GetMouseY() * Time.deltaTime * controller.MouseSensitivity;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        controller.transform.rotation = Quaternion.Euler(0, yRotation, 0);
        controller.cameraPosTransform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }
}
