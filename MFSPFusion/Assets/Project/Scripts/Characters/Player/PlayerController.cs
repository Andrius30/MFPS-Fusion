using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float playerSpeed;
    [SerializeField] float mouseSensitivity;
    public Transform cameraPosTransform;

    [Header("Jump settings")]
    public float gravity = -9.81f;
    public float jumpHeight = 7f;

    #region Ground Check
    [Header("Ground Check")]
    [FoldoutGroup("Ground Check")] public LayerMask groundMask;
    [FoldoutGroup("Ground Check")] public Transform groundCheckTransform;
    [FoldoutGroup("Ground Check")] public float groundCheckRadius;
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
    public float PlayerSpeed => playerSpeed;
    public float MouseSensitivity => mouseSensitivity;

    #endregion

    #region Class objects
    PlayerModel playerModel;
    PlayerMove playerMove;
    Inputs inputs;
    [HideInInspector] public GroundCheck groundCheck;
    PlayerJump playerJump;


    #endregion

    void Awake()
    {
        playerModel = new PlayerModel(this);
        playerMove = new PlayerMove(this);
        groundCheck = new GroundCheck(groundCheckTransform, groundCheckRadius, groundMask);
        playerJump = new PlayerJump(this);

        CameraController.onTargetSpawn?.Invoke(cameraPosTransform);
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        playerModel.RotatePlayer();
        playerMove.Move();
        playerJump.Jump();
    
    
    }

    void OnDrawGizmos()
    {
        if (groundCheck == null) return;
        groundCheck.Visualize();
    }



}
