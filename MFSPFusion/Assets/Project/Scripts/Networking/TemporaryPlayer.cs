using Andrius.Core.Debuging;
using Fusion;
using TMPro;
using UnityEngine;
using Timer = Andrius.Core.Timers.Timer;

public class TemporaryPlayer : NetworkBehaviour
{
    public Vector3 topRightBounds;
    public Vector3 topLeftBounds;
    public Vector3 BottomRightBounds;
    public Vector3 BottomLeftBounds;


    [SerializeField] TextMeshProUGUI readyText;
    [SerializeField] float textShowTime = 1f;

    [Networked(OnChanged = nameof(OnChanged), OnChangedTargets = OnChangedTargets.All)]
    public NetworkBool isReady { get; set; }
    [Networked] public NetworkButtons inputs { get; set; }

    Timer textTimer;

    void Start()
    {
        textTimer = new Timer(0, OnDoneDisable, false, true);
        ToggleReadyText(isReady);
    }


    public static void OnChanged(Changed<TemporaryPlayer> changed)
    {
        changed.LoadNew();
        var newVal = changed.Behaviour.isReady;

        if (FusionCallbacks.runner.IsServer)
        {
            if (GameManager.instance.CheckIfPlayersReady())
            {
                LevelManager levelManager = FindObjectOfType<LevelManager>();
                levelManager.LoadLevel(3); // load to game scene
            }
        }

    }
    public override void FixedUpdateNetwork()
    {
        if (GetInput<NetworkInputs>(out var input) == false) return;
        if (input.buttons.IsSet(MyButtons.Ready))
        {
            isReady = !isReady;
            RPC_SetReady(isReady);

        }
    }

    void Update()
    {
        CheckBounds();
        if (textTimer == null) return;
        if (textTimer.IsDone()) return;
        textTimer.StartTimer();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_SetReady(bool ready)
    {
        ToggleReadyText(true);
        string txt = ready ? "Player is ready!" : "Player is not ready!";
        Color color = ready ? Color.green : Color.red;
        SetReadyText(txt, color);
        textTimer.SetTimer(textShowTime, false);
    }

    void ToggleReadyText(bool enabled) => readyText.enabled = enabled;
    void OnDoneDisable() => ToggleReadyText(false);
    void SetReadyText(string text, Color color) => readyText.text = $"{text}:{color};".Interpolate();
    void CheckBounds()
    {
        if (transform.position.x >= topRightBounds.x)
        {
            transform.position = new Vector3(topRightBounds.x, transform.position.y, transform.position.z);
        }
        else if (transform.position.x <= topLeftBounds.x)
        {
            transform.position = new Vector3(topLeftBounds.x, transform.position.y, transform.position.z);
        }
        else if (transform.position.z >= topRightBounds.z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, topRightBounds.z);
        }
        else if (transform.position.z >= topLeftBounds.z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, topLeftBounds.z);
        }
        else if (transform.position.x >= BottomRightBounds.x)
        {
            transform.position = new Vector3(BottomRightBounds.x, transform.position.y, transform.position.z);
        }
        else if (transform.position.x <= BottomLeftBounds.x)
        {
            transform.position = new Vector3(BottomLeftBounds.x, transform.position.y, transform.position.z);
        }
        else if (transform.position.z <= BottomRightBounds.z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, BottomRightBounds.z);
        }
        else if (transform.position.z <= BottomLeftBounds.z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, BottomLeftBounds.z);
        }
    }

}
