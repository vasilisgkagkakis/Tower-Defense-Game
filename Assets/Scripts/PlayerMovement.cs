using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Vector3 Velocity;
    private Vector3 PlayerMovementInput;
    private Vector2 PlayerMouseInput;
    private float xRot;

    [SerializeField] private CharacterController Controller;
    [SerializeField] private Transform PlayerCamera;
    [Space]
    [SerializeField] private float Speed;
    [SerializeField] private float Sensitivity;

    void Update()
    {
        // Handle player movement input
        PlayerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        PlayerMouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        MovePlayer();
        MovePlayerCamera();
    }

    private void MovePlayer()
    {
        // Speed boost with Left Shift
        float currentSpeed = Speed;
        if (Input.GetKey(KeyCode.LeftShift))
            currentSpeed = 10f; // Boosted speed
        // else keep currentSpeed as Speed (your normal speed)

        Vector3 MoveVector = transform.TransformDirection(PlayerMovementInput);

        if (Input.GetKey(KeyCode.Space))
        {
            Velocity.y = 1f;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            Velocity.y = -1f;
        }

        Controller.Move(MoveVector * currentSpeed * Time.deltaTime);
        Controller.Move(Velocity * currentSpeed * Time.deltaTime);

        Velocity.y = 0f; // Reset vertical velocity after movement
    }

    private void MovePlayerCamera()
    {
        if (Input.GetMouseButton(1))
        {
            xRot -= PlayerMouseInput.y * Sensitivity;
            transform.Rotate(0f, PlayerMouseInput.x * Sensitivity, 0f);
            PlayerCamera.transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
        }
    }
}
