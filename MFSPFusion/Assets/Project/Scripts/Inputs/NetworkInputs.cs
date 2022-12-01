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
    FireUp = 11,
    Keyboard1Key = 12,
    Keyboard2Key = 13,
    Keyboard3Key = 14,
    Keyboard4Key = 15,
    DropWeapon = 16,
    PickKey = 17

}
public struct NetworkInputs : INetworkInput
{
    public NetworkButtons buttons;
    public float mousex;
    public float mousey;
    public float scrollWheel;
    public bool leftCtrlReleased;
}
