using Fusion;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerController : BaseCharacter
{
    public enum PlayerStates
    {
        NORMAL,
        SPRINT,
        CROUCH,
        WALL_RUNNING,
        WALL_CLIMB,
        SLOPE,

    }

    public PlayerRef thisPlayer;

    #region Move settings
    [FoldoutGroup("Move settings")] public PlayerStates currentState;
    [FoldoutGroup("Move settings"), SerializeField] float playerSpeed;
    [FoldoutGroup("Move settings"), SerializeField] float playerSprintSpeed;
    [FoldoutGroup("Move settings")] public float playerCrouchSpeed;
    [FoldoutGroup("Move settings")] public float playerCrouchScale;
    [FoldoutGroup("Move settings")] public float maxSlopeAngle = 45f;
    [FoldoutGroup("Move settings")] public float slopeSpeed;
    [FoldoutGroup("Move settings")] public float slopeCheckDistance = 0.7f;
    [FoldoutGroup("Move settings"), SerializeField] float mouseSensitivity;
    [FoldoutGroup("Move settings")] public Transform cameraTransform;
    [FoldoutGroup("Move settings")] public bool exitingSlope = false;
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
    public float MouseSensitivity => mouseSensitivity;

    #endregion

    #region hide in inspector
    [HideInInspector] public bool wallRunning;
    [HideInInspector] public float currentSpeed;
    [HideInInspector] public GroundCheck groundCheck;
    [HideInInspector] public Vector3 moveDirection;
    [HideInInspector] public bool isCrouching = false;
    [HideInInspector] public bool isSprinting = false;
    [HideInInspector] public bool isClimbing = false;
    #endregion

    #region Class objects
    [SerializeField] HeadbobController headbobController;
    PlayerModel playerModel;
    [HideInInspector] public PlayerData playerData;
    PlayerMove playerMove;
    Inputs inputs;
    PlayerJump playerJump;
    PlayerWallRun wallRun;
    PlayerClimb wallClimb;
    PlayerCrouch playerCrouch;
    PlayerSlopeMovement playerSlopeMovement;

    #endregion


    protected override void Awake()
    {
        base.Awake();
        playerModel = new PlayerModel(this);
        playerData = new PlayerData();
        playerMove = new PlayerMove(this);
        groundCheck = new GroundCheck(groundCheckTransform, groundCheckRadius, groundMask);
        playerJump = new PlayerJump(this);
        wallRun = new PlayerWallRun(this);
        wallClimb = new PlayerClimb(this);
        playerCrouch = new PlayerCrouch(this);
        playerSlopeMovement = new PlayerSlopeMovement(this);

    }
    void Start()
    {
        if (Object.HasInputAuthority)
        {
            ChangeState(PlayerStates.NORMAL);
        }
        else
        {
            cameraTransform.GetComponentInChildren<Camera>().enabled = false;
            statsScreen.gameObject.SetActive(false);
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        if (Object.HasInputAuthority)
            playerModel.RotateLocalyX();
    }
    public void ChangeState(PlayerStates newState)
    {
        if (newState != currentState)
        {
            currentState = newState;
        }
    }
    public override void FixedUpdateNetwork()
    {
        if (GetInput<NetworkInputs>(out var input) == false) return;

        ChangeSpeedBasedOnState(input);
        playerModel.RotatePlayer(input);
        playerJump.Jump(input);
        wallRun.WallrunChecks(input);
        wallClimb.ClimbChecks(input);
        playerMove.Sprint(input);
        playerCrouch.CrouchInputs(input);
        playerCrouch.Crouch();
        playerSlopeMovement.SlopeMove();

        if (!isCrouching && !isSprinting && !isClimbing && !wallRunning)
        {
            ChangeState(PlayerStates.NORMAL);
        }
    }
    public override void Spawned()
    {
        initHealth = maxHealth;
        statsScreen.SetHealthStats(initHealth, maxHealth);
    }

    void ChangeSpeedBasedOnState(NetworkInputs input)
    {
        switch (currentState)
        {
            case PlayerStates.NORMAL:
                currentSpeed = playerSpeed;
                if (moveDirection.x > 0f || moveDirection.z > 0f)
                {
                    headbobController.isWalking = true;
                    headbobController.isRunning = false;
                }
                else
                {
                    headbobController.isWalking = false;
                    headbobController.isRunning = false;
                }
                playerMove.Move(input);
                break;
            case PlayerStates.SPRINT:
                headbobController.isWalking = false;
                headbobController.isRunning = true;
                currentSpeed = playerSprintSpeed;
                playerMove.Move(input);
                break;
            case PlayerStates.CROUCH:
                currentSpeed = playerCrouchSpeed;
                playerMove.Move(input);
                break;
            case PlayerStates.WALL_RUNNING:
                currentSpeed = wallRunSpeed;
                wallRun.Run();
                break;
            case PlayerStates.WALL_CLIMB:
                currentSpeed = wallClimbSpeed;
                wallClimb.Climb(input);
                break;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7)
        {
            var weapon = other.GetComponent<Weapon>();
            weaponsHolder.PickWeapon(weapon);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (groundCheck == null) return;
        groundCheck.Visualize();
        wallRun.Visualize();
        wallClimb.Visualize();
        playerSlopeMovement.Visualize();
    }

#endif


}
