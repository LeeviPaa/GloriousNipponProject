using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    #region References & variables
    Animator animator;
    bool isActive = false;
    [SerializeField]
    GameObject animatedMesh;
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
            animator = animatedMesh.GetComponent<Animator>();
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

            if (newState)
            {
                ResetAnimator();
            }
        }

        isActive = newState;
    }

    private void ResetAnimator()
    {
        animator.SetInteger("MovementSpeed", 0);
    }
    #endregion

    #region Animation calls
    public void PlayAttackAnimation()
    {
        if (isActive)
        {
            animator.SetTrigger("Attack");
        }
    }

    public void PlayAlertedAnimation()
    {
        if (isActive)
        {
            animator.SetTrigger("Alerted");
        }
    }
    
    public void StartGuardAnimation()
    {
        if (isActive)
        {
            animator.ResetTrigger("Move");
            animator.SetTrigger("StartGuarding");
        }
    }

    public void StartCheckPerimeterAnimation()
    {
        if (isActive)
        {
            animator.ResetTrigger("Move");
            animator.SetTrigger("StartCheckingPerimeter");
        }
    }

    public void StartMovementAnimation()
    {
        if (isActive)
        {
            animator.SetTrigger("Move");
        }
    }
    #endregion

    #region Animation states
    public void SetMovementSpeed(int newSpeed)
    {
        if (isActive)
        {
            animator.SetFloat("MovementSpeed", newSpeed);
        }
    }
    #endregion

}
