using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Namespace for animations
/// </summary>
namespace Animations
{
    /// <summary>
    /// Base class used for updating an animation controller (Classes that inherit from this class should implement animation interfaces)
    /// </summary>
    public abstract class AnimationController : MonoBehaviour
    {

    }

    /// <summary>
    /// Interface used to play movement animations
    /// </summary>
    public interface IPlayMovementAnimation
    {
        void PlayMovementAnimation(Vector2 inputVector);

    }

    /// <summary>
    /// Interface used to play grounded / airborne animations
    /// </summary>
    public interface IPlayGroundedAnimation
    {
        void PlayGroundedAnimation(bool isGrounded);
    }

    /// <summary>
    /// Interfaced used to play jump animations
    /// </summary>
    public interface IPlayJumpAnimation
    {
        void PlayJumpAnimation();
    }

    /// <summary>
    /// Interface used to play attack animations
    /// </summary>
    public interface IPlayAttackAnimation
    {
        void PlayAttackAnimation();
    }

    /// <summary>
    /// Interface used to play death animations
    /// </summary>
    public interface IPlayDeathAnimation
    {
        void PlayDeathAnimation();
    }
}


