using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum PlayerStates
    {
        NORMAL,
        JUMPING,
        RUNNING,
        WALL_RUNNING,
    }
    [SerializeField] PlayerStates currentState;
    [SerializeField] float playerSpeed;
    [SerializeField] float mouseSensitivity;
    public Transform cameraPosTransform;

    [Header("Jump settings")]
    public float gravity = -9.81f;
    public float jumpHeight = 7f;

    [FoldoutGroup("Wall Running")] public LayerMask wallMask;
    [FoldoutGroup("Wall Running")] public float wallCheckDistance;
    [FoldoutGroup("Wall Running")] public float wallRunSpeed;
    [HideInInspector] public RaycastHit leftWallHit;
    [HideInInspector] public RaycastHit rightWallHit;
    [HideInInspector] public bool wallLeft;
    [HideInInspector] public bool wallRight;

    #region Ground Check
    [Header("Ground Check")]
    [FoldoutGroup("Ground Check")] public LayerMask groundMask;
    [FoldoutGroup("Ground Check")] public Transform groundCheckTransform;
    [FoldoutGroup("Ground Check")] public float groundCheckRadius;
    [FoldoutGroup("Ground Check")] public bool useGravity = true;
    #endregion

    #region Getters
    public Inputs Inputs
    {
        get
        {
            if (inputs == null)
            {
                inputs = new Inputs();
                inputs.Init();
            }

            return inputs;
        }
    }
    public float MouseSensitivity => mouseSensitivity;

    #endregion

    [HideInInspector] public bool wallRunning;
    [HideInInspector] public float currentSpeed;
    [HideInInspector] public GroundCheck groundCheck;

    #region Class objects
    PlayerModel playerModel;
    PlayerMove playerMove;
    Inputs inputs;
    PlayerJump playerJump;
    PlayerWallRun wallRun;

    #endregion

    void Awake()
    {
        playerModel = new PlayerModel(this);
        playerMove = new PlayerMove(this);
        groundCheck = new GroundCheck(groundCheckTransform, groundCheckRadius, groundMask);
        playerJump = new PlayerJump(this);
        wallRun = new PlayerWallRun(this);

        CameraController.onTargetSpawn?.Invoke(cameraPosTransform);
        ChangeState(PlayerStates.NORMAL);
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void ChangeState(PlayerStates newState)
    {
        if (newState != currentState)
        {
            currentState = newState;
        }
    }

    void ChangeSpeedbasedOnState()
    {
        switch (currentState)
        {
            case PlayerStates.NORMAL:
                currentSpeed = playerSpeed;
                break;
            case PlayerStates.WALL_RUNNING:
                currentSpeed = wallRunSpeed;
                break;
        }
    }

    void Update()
    {
        playerModel.RotatePlayer();
        playerMove.Move();
        playerJump.Jump();
        wallRun.Run();


        ChangeSpeedbasedOnState();
    }

    void OnDrawGizmos()
    {
        if (groundCheck == null) return;
        groundCheck.Visualize();
        wallRun.Visualize();
    }



}
