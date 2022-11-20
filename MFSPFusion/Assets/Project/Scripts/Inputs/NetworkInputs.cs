using Fusion;

enum MyButtons
{
    Forward = 0,
    Backward = 1,
    Left = 2,
    Right = 3,
    Jump = 4,
    SpaceReleased = 5,
    LeftShiftHolding = 6,
    LeftCtrl = 7,
    Ready = 8,
    Fire = 9,
    FireHold = 10,
    Keyboard1Key = 11,
    Keyboard2Key = 12,
    Keyboard3Key = 13,
    DropWeapon = 14

}
public struct NetworkInputs : INetworkInput
{
    public NetworkButtons buttons;
    public float mousex;
    public float mousey;
    public float scrollWheel;
    public bool leftCtrlReleased;
}
