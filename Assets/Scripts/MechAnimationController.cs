using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animations;

public class MechAnimationController : AnimationController, IPlayMovementAnimation, IPlayAttackAnimation, IPlayDeathAnimation
{
    [Tooltip("The animator of this enemy")]
    [SerializeField] Animator enemyAnimator;
    [Tooltip("The X-Axis movement speed animator parameter")]
    [SerializeField] string moveSpeedXParameter = "moveX";
    [Tooltip("The Y-Axis movement speed animator parameter")]
    [SerializeField] string moveSpeedYParameter = "moveY";
    [Tooltip("The attack animator parameter")]
    [SerializeField] string attackTriggerParameterName = "attack";
    [Tooltip("The death animator parameter")]
    [SerializeField] string deathTriggerParameter = "die";
    [Tooltip("How quickly movement animations should be interpolated (this is useful for smoothly transitioning stopped idle animations to moving animations)")]
    [SerializeField] float moveAnimationAcceleration = 3.0f;
    
    /// <summary>
    /// The cached movement vector
    /// </summary>
    Vector2 movementVector = new Vector2(0.0f,0.0f);

    public void PlayMovementAnimation(Vector2 movementVector)
    {
        // Lerp the movement vector towards the new movement vector
        this.movementVector = Vector2.Lerp(this.movementVector, movementVector, Time.deltaTime * moveAnimationAcceleration);
        // Set the x and y movement parameters
        enemyAnimator.SetFloat(moveSpeedXParameter, this.movementVector.x);
        enemyAnimator.SetFloat(moveSpeedYParameter, this.movementVector.y);
    }

    /// <summary>
    /// Plays the attack animation
    /// </summary>
    public void PlayAttackAnimation()
    {
        enemyAnimator.SetTrigger(attackTriggerParameterName);
    }

    /// <summary>
    /// Plays the death animation
    /// </summary>
    public void PlayDeathAnimation()
    {
        AIRoamingController roamController = GetComponent<AIRoamingController>();
        roamController.SetRoam(false);
        GetComponent<MechAttackScript>().StopAttackingTarget();
        enemyAnimator.SetTrigger(deathTriggerParameter);
    }
}
