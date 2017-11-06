using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyAnimationController : MonoBehaviour
{
    #region References & variables
    Animator animator;
    bool isActive = false;
    #endregion

    #region Initialization
    private void Start()
    {
        FindAnimatorReference();
    }

    private void FindAnimatorReference()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }
    #endregion

    #region Activation
    public void SetActiveState(bool newState)
    {
        FindAnimatorReference();

        if (animator != null)
        {
            animator.enabled = newState;
        }

        isActive = newState;
    }
    #endregion

    #region Animation calls
    public void PlayAttackAnimation()
    {
        if (isActive)
        {
            animator.Play("");
        }
    }

    public void PlayGuardAnimation()
    {
        if (isActive)
        {
            animator.Play("");
        }
    }

    public void PlayCheckPerimeterAnimation()
    {
        if (isActive)
        {
            animator.Play("");
        }
    }
    #endregion

    #region Animation states
    public void SetMovementSpeed(float newSpeed)
    {
        if (isActive)
        {
            animator.SetFloat("MovementSpeed", newSpeed);
        }
    }
    #endregion

}
