using Fusion;

enum MyButtons
{
    Forward = 0,
    Backward = 1,
    Left = 2,
    Right = 3,
    Jump = 4,
    SpaceReleased = 5,
    CKeyHolding = 6,
    LeftShiftHolding = 7,
    LeftShiftReleased = 8,
    LeftCtrl = 9,
    LeftCtrlReleased = 10,
    Ready = 11
}
public struct NetworkInputs : INetworkInput
{
    public NetworkButtons buttons;
    public float mousex;
    public float mousey;
}
