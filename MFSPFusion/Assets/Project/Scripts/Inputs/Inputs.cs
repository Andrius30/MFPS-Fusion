using UnityEngine;
using UnityEngine.InputSystem;

public class Inputs : MonoBehaviour
{
    Keyboard keyboard;
    Mouse mouse;

    public float GetHorizontal() => Input.GetAxis("Horizontal");
    public float GetVertical() => Input.GetAxis("Vertical");


    // Mouse

    public float GetMouseX() => Input.GetAxisRaw("Mouse X");
    public float GetMouseY() => Input.GetAxisRaw("Mouse Y");
}
