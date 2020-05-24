using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Game Objects")]
    [SerializeField] CharacterController playerController;
    [SerializeField] Camera playerCamera;
    [SerializeField] Transform groundCheckReference;

    [Header("Player Movement Settings")]
    [SerializeField] float movementSpeed;
    [SerializeField] float playerJumpHeight = 2.0f;
    
    [SerializeField] float gravity = -9.81f;
    [SerializeField] LayerMask groundLayerMask;

    [Header("Unity Input Settings")]
    [SerializeField] string horizontalMove;
    [SerializeField] string verticalMove;
    [SerializeField] string lookXAxis;
    [SerializeField] string lookYAxis;
    [SerializeField] bool invertYAxis;
    [SerializeField] string jump;
    [SerializeField] bool jumpCountsTowardsCombos = false;
    [SerializeField] bool lockMouseCursorToScreenOnStartup = true;

    /// <summary>
    /// The player's velocity
    /// </summary>
    Vector3 playerVelocity = new Vector3();

    /// <summary>
    /// The player's Y rotation (the rotation of the player's camera)
    /// </summary>
    Vector3 playerCameraRotation;

    /// <summary>
    /// Indicator used if the player is on the ground
    /// </summary>
    private bool isGrounded = false;

    /// <summary>
    /// The main player camera
    /// </summary>
    public static Camera PlayerCamera
    {
        get;
        private set;
    }

    private void Awake()
    {
        // Set the mouse curser so it's locked
        if (lockMouseCursorToScreenOnStartup)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        // Cache the player's initial camera rotation
        playerCameraRotation = playerCamera.transform.localEulerAngles;
        PlayerCamera = playerCamera;
    }

    // Update is called once per frame
    void Update()
    {
        ProcessMouseInput();
        ProcessMoveInput();
        UpdateAndApplyVelocity();
    }

    /// <summary>
    /// Method called to process the player's mouse input (look)
    /// </summary>
    private void ProcessMouseInput()
    {
        // Get the player  game object rotation
        Vector3 playerTransformRotation = playerController.transform.eulerAngles;
        // Rotate the object y axis by the mouse x axis input
        playerTransformRotation.y += Input.GetAxis(lookXAxis);
        // Apply the player input rotation
        playerController.transform.eulerAngles = playerTransformRotation;
        // Get the mouse y axis input
        float mouseYInput = Input.GetAxis(lookYAxis);
        // Invert it if required
        if (invertYAxis) mouseYInput *= -1.0f;
        // Get the player camera rotation, add the mouse input to it
        playerCameraRotation.x = playerCameraRotation.x + mouseYInput;
        // Clamp the rotation so it's between -90 and 90 degrees
        playerCameraRotation.x = Mathf.Clamp(playerCameraRotation.x, -90.0f, 90.0f);
        // Apply its rotation
        playerCamera.transform.localEulerAngles = playerCameraRotation;
    }

    /// <summary>
    /// Method called to process the player's movement input
    /// </summary>
    private void ProcessMoveInput()
    {
        // Get the horizontal and vertical movement
        float x = Input.GetAxis(horizontalMove);
        float z = Input.GetAxis(verticalMove);
        // Get the input relative to the player's current position (player's forward and players right)
        Vector3 movement = playerController.transform.forward * z + playerController.transform.right * x;
        // Adjust the movement vector by speed and deltaTime
        movement *= movementSpeed * Time.deltaTime;
        playerController.Move(movement);
    }

    /// <summary>
    /// Updates and applies the player's velocity
    /// </summary>
    private void UpdateAndApplyVelocity()
    {
        // Cache whether or not the player is grounded
        isGrounded = IsGrounded();
        // If the player is grounded, then allow the player the jump or reset the velocity
        if (isGrounded)
        {
            // Allow the player to jump
            Jump();
            // If the player didn't jump, then keep reseting the velocity to -3
            if(playerVelocity.y < 0)
            {
                playerVelocity.y = -3.0f;
            }
        }
        // If the player is airborne, then keep applying the gravity to the player's velocity
        else
        {
            playerVelocity.y += gravity * Time.deltaTime;
        }
        // Apply the player's velocity
        playerController.Move(playerVelocity * Time.deltaTime);
    }

    /// <summary>
    /// Applies a jump
    /// </summary>
    private void Jump()
    {
        // If the player is jumping, set the velocity upwards
        // The formula for jumping at a specific height is
        // Square Root of (desiredHeigh * -2 * gravity)
        if (Input.GetButtonDown(jump))
        {
            if (jumpCountsTowardsCombos)
            {
                if (BeatSyncReceiver.BeatReceiver.RequestInputAction())
                {
                    playerVelocity.y = Mathf.Sqrt(playerJumpHeight * -2.0f * gravity);
                }
            }
            else
            {
                playerVelocity.y = Mathf.Sqrt(playerJumpHeight * -2.0f * gravity);
            }
        }
    }

    /// <summary>
    /// Whether or not the player is grounded
    /// </summary>
    /// <returns></returns>
    bool IsGrounded()
    {
        return Physics.CheckBox(groundCheckReference.position, groundCheckReference.transform.lossyScale, Quaternion.identity, groundLayerMask.value);
    }


}
