using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animations;

[RequireComponent(typeof(CharacterController))]
/// <summary>
/// Base class used for controlling character movement (Non-Player Characters)
/// </summary>
public class CharacterMovementController : MonoBehaviour
{
    [Header("Character Game Object References")]
    [Tooltip("The character's character controller component")]
    [SerializeField] CharacterController characterController;
    [Tooltip("The reference position where the character should check if it's on the ground (should be at the feet)")]
    [SerializeField] Transform groundCheckReference;

    [Header("Character Movement Settings")]
    [Tooltip("The character's movement speed")]
    [SerializeField] float movementSpeed;
    [Tooltip("The character's jump height")]
    [SerializeField] float characterJumpHeight = 2.0f;
    [Tooltip("The gravity applied to this character")]
    [SerializeField] float gravity = -9.81f;
    [Tooltip("The ground layer mask to check against")]
    [SerializeField] LayerMask groundLayerMask;

    [Header("Animator")]
    [Tooltip("The animator controller for this character mover")]
    [SerializeField] AnimationController animator;

    /// <summary>
    /// The character's velocity
    /// </summary>
    Vector3 characterVelocity = new Vector3();
    /// <summary>
    /// The input vector of this character
    /// </summary>
    Vector2 inputVector = new Vector2(0.0f,0.0f);

    /// <summary>
    /// Indicator used if the character is on the ground
    /// </summary>
    private bool isGrounded = false;

    /// <summary>
    /// The optional movement animator interface for this character
    /// </summary>
    IPlayMovementAnimation movementAnimator = null;
    /// <summary>
    /// The optional jump animator interface for this character
    /// </summary>
    IPlayJumpAnimation jumpAnimator = null;
    /// <summary>
    /// The optional grounded animator interface for this character
    /// </summary>
    IPlayGroundedAnimation groundedAnimator = null;

    private void Awake()
    {
        // If the animator isn't null, then assign the animation interfaces
        if(animator != null)
        {
            if(animator is IPlayMovementAnimation)
            {
                movementAnimator = animator as IPlayMovementAnimation;
            }
            if(animator is IPlayJumpAnimation)
            {
                jumpAnimator = animator as IPlayJumpAnimation;
            }
            if (animator is IPlayGroundedAnimation)
            {
                groundedAnimator = animator as IPlayGroundedAnimation;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        ProcessMoveInput();
        UpdateAndApplyVelocity();
    }

    /// <summary>
    /// Method called to make this character move based on a 2D input vector
    /// </summary>
    /// <param name="inputVector"></param>
    public void MoveCharacter(Vector2 inputVector)
    {
        this.inputVector = inputVector;
    }

    /// <summary>
    /// Method called to make this character jump if the character is grounded
    /// </summary>
    public void Jump()
    {
        if (isGrounded)
        {
            jumpRequested = true;
        }
    }

    /// <summary>
    /// Whether or not a jump has been requested
    /// </summary>
    private bool jumpRequested;

    /// <summary>
    /// Method called to process the character's movement input
    /// </summary>
    private void ProcessMoveInput()
    {
        // Transform the movement vector from 2D space into 3D space
        Vector3 moveVector = inputVector.x * characterController.transform.right + inputVector.y * characterController.transform.forward;
        // Move towards the movement vector * Time.deltaTime * the movement speed
        characterController.Move(moveVector * Time.deltaTime * movementSpeed);
        // If the movement animator is not null, then play it's movement animation with the input vector
        if(movementAnimator != null)
        {
            movementAnimator.PlayMovementAnimation(inputVector);
        }
    }

    /// <summary>
    /// Updates and applies the character's velocity / physics
    /// </summary>
    private void UpdateAndApplyVelocity()
    {
        // Cache whether or not the player is grounded
        isGrounded = IsGrounded();
        // If the player is grounded, then allow the character the jump, otherwise reset the velocity
        if (isGrounded)
        {
            // Allow the jump if a jump was requested
            if (jumpRequested)
            {
                // If the character is jumping, set the velocity upwards
                // The formula for jumping at a specific height is
                // Square Root of (desiredHeigh * -2 * gravity)
                characterVelocity.y = Mathf.Sqrt(characterJumpHeight * -2.0f * gravity);
                // Play a jump animation if there's an interface for it
                if(jumpAnimator != null)
                {
                    jumpAnimator.PlayJumpAnimation();
                }
            }
            // If the player didn't jump, then keep reseting the velocity to -3
            if (characterVelocity.y < 0)
            {
                characterVelocity.y = -3.0f;
            }
        }
        // If the character is airborne, then keep applying the gravity to the character's velocity
        else
        {
            characterVelocity.y += gravity * Time.deltaTime;
        }
        // Apply the character's velocity
        characterController.Move(characterVelocity * Time.deltaTime);
        // If there's a grounded animation interface, then play a grounded animation
        if (groundedAnimator != null)
        {
            groundedAnimator.PlayGroundedAnimation(isGrounded);
        }
    }


    /// <summary>
    /// Whether or not the character is grounded
    /// </summary>
    /// <returns></returns>
    bool IsGrounded()
    {
        return Physics.CheckBox(groundCheckReference.position, groundCheckReference.transform.lossyScale, Quaternion.identity, groundLayerMask.value);
    }

    /// <summary>
    /// Transforms a NavMeshAgent's desired velocity into an input Vector2 (identical to raw player axis input)
    /// </summary>
    /// <param name="agent">The navmeshagent to simulate an input Vector2 out of</param>
    /// <returns></returns>
    public static Vector2 AgentVelocityToVector2DInput(UnityEngine.AI.NavMeshAgent agent)
    {
        // Get the NavMeshAgent's desired velocity direction relative from it's actual position
        Vector3 desiredVelocityRelativeToAgent = agent.transform.InverseTransformDirection(agent.desiredVelocity);
        // Normalize the vector so it doesn't have a magnitude beyond 1.0f
        desiredVelocityRelativeToAgent.Normalize();
        // X value will be the X value of the vector
        // Y value will be the Z value of the vector
        // Return the 2D input vector
        return new Vector2(desiredVelocityRelativeToAgent.x, desiredVelocityRelativeToAgent.z);
    }
}
