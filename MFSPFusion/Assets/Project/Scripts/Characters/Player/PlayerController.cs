using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float playerSpeed;
    [SerializeField] float mouseSensitivity;
    public Transform cameraPosTransform;

    public float PlayerSpeed => playerSpeed;
    public float MouseSensitivity => mouseSensitivity;
    Inputs inputs;
    public Inputs Inputs
    {
        get
        {
            if (inputs == null)
                inputs = new Inputs();

            return inputs;
        }
    }

    PlayerModel playerModel;
    PlayerMove playerMove;

    void Awake()
    {
        playerModel = new PlayerModel(this);
        playerMove = new PlayerMove(this);
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


    }





}
