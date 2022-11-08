using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inputs
{
    Keyboard keyboard;
    Mouse mouse;

    public void Init()
    {
        keyboard = Keyboard.current;
        mouse = Mouse.current;
    }

    //public float GetHorizontal() => Input.GetAxis("Horizontal");
    //public float GetVertical() => Input.GetAxis("Vertical");
    //public bool SpacePressed() => keyboard.spaceKey.wasPressedThisFrame;
    //public bool SpaceReleased() => keyboard.spaceKey.wasReleasedThisFrame;
    //public bool CKeyHolding() => keyboard.cKey.isPressed;
    //public bool LeftShiftHolding() => keyboard.leftShiftKey.IsPressed();
    //internal bool LeftShiftReleased() => keyboard.leftShiftKey.wasReleasedThisFrame;
    //public bool LeftCtrlHolding() => keyboard.leftCtrlKey.IsPressed();
    //public bool LeftCtrlReleased() => keyboard.leftCtrlKey.wasReleasedThisFrame;


    // Mouse

    //public float GetMouseX() => Input.GetAxisRaw("Mouse X");
    //public float GetMouseY() => Input.GetAxisRaw("Mouse Y");

}
