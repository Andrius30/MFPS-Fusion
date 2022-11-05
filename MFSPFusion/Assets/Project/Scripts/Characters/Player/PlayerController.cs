using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum PlayerStates
    {
        NORMAL,
        SPRINT,
        CROUCH,
        WALL_RUNNING,
        WALL_CLIMB,
    }

    #region Move settings
    [FoldoutGroup("Move settings"), SerializeField] PlayerStates currentState;
    [FoldoutGroup("Move settings"), SerializeField] float playerSpeed;
    [FoldoutGroup("Move settings"), SerializeField] float playerSprintSpeed;
    [FoldoutGroup("Move settings"), SerializeField] float playerCrouchSpeed;
    [FoldoutGroup("Move settings"), SerializeField] float mouseSensitivity;
    [FoldoutGroup("Move settings")] public Transform cameraPosTransform;
    #endregion

    #region Jump settings
    [FoldoutGroup("Jump and gravity")] public float gravity = -9.81f;
    [FoldoutGroup("Jump and gravity")] public float jumpHeight = 7f;
    #endregion

    #region Wall running
    [FoldoutGroup("Wall Running")] public LayerMask wallMask;
    [FoldoutGroup("Wall Running")] public float wallCheckDistance;
    [FoldoutGroup("Wall Running")] public float wallRunSpeed;
    [HideInInspector] public RaycastHit leftWallHit;
    [HideInInspector] public RaycastHit rightWallHit;
    [HideInInspector] public bool wallLeft;
    [HideInInspector] public bool wallRight; 
    #endregion

    #region Ground Check
    [FoldoutGroup("Ground Check")] public LayerMask groundMask;
    [FoldoutGroup("Ground Check")] public Transform groundCheckTransform;
    [FoldoutGroup("Ground Check")] public float groundCheckRadius;
    [FoldoutGroup("Ground Check")] public bool useGravity = true;
    #endregion

    #region Wall climb
    [FoldoutGroup("Wall Climb")] public float wallClimbSpeed;
    [FoldoutGroup("Wall Climb")] public float wallClimbCheckRadius = .5f;
    [FoldoutGroup("Wall Climb")] public float wallClimbDetectionLength = 1f;
    [FoldoutGroup("Wall Climb")] public float maxClimbAngle = 30f; 
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

    #region hide in inspector
    [HideInInspector] public bool wallRunning;
    [HideInInspector] public float currentSpeed;
    [HideInInspector] public GroundCheck groundCheck;
    [HideInInspector] public Vector3 moveDirection; 
    #endregion

    #region Class objects
    PlayerModel playerModel;
    PlayerMove playerMove;
    Inputs inputs;
    PlayerJump playerJump;
    PlayerWallRun wallRun;
    PlayerClimb wallClimb;

    #endregion

    void Awake()
    {
        playerModel = new PlayerModel(this);
        playerMove = new PlayerMove(this);
        groundCheck = new GroundCheck(groundCheckTransform, groundCheckRadius, groundMask);
        playerJump = new PlayerJump(this);
        wallRun = new PlayerWallRun(this);
        wallClimb = new PlayerClimb(this);

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

    void ChangeSpeedBasedOnState()
    {
        switch (currentState)
        {
            case PlayerStates.NORMAL:
                currentSpeed = playerSpeed;
                playerMove.Move();
                break;
            case PlayerStates.SPRINT:
                currentSpeed = playerSprintSpeed;
                playerMove.Move();
                break;
            case PlayerStates.WALL_RUNNING:
                currentSpeed = wallRunSpeed;
                wallRun.Run();
                break;
            case PlayerStates.WALL_CLIMB:
                currentSpeed = wallClimbSpeed;
                wallClimb.Climb();
                break;

        }
    }

    void Update()
    {
        playerModel.RotatePlayer();
        playerJump.Jump();
        wallRun.WallrunChecks();
        wallClimb.ClimbChecks();
        ChangeSpeedBasedOnState();
    }

    void OnDrawGizmos()
    {
        if (groundCheck == null) return;
        groundCheck.Visualize();
        wallRun.Visualize();
        wallClimb.Visualize();
    }



}
