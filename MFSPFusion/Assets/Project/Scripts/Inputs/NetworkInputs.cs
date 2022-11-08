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
    LeftShiftReleased = 7,
    LeftCtrl = 8,
    LeftCtrlReleased = 9,
    Ready = 10
}
public struct NetworkInputs : INetworkInput
{
    public NetworkButtons buttons;
    public float mousex;
    public float mousey;
}
