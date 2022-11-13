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
    Fire = 9
    //LeftCtrlReleased = 9,
}
public struct NetworkInputs : INetworkInput
{
    public NetworkButtons buttons;
    public float mousex;
    public float mousey;
    public bool leftCtrlReleased;
}
