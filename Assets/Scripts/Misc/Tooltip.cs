using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    [SerializeField]
    private AnimationState state_Shown, state_Hidden;

    private Animator animator;
    private float currentTime;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ShowToolTip()
    {
        currentTime = 1 - Mathf.Clamp((animator.GetCurrentAnimatorStateInfo(0).normalizedTime), -1, 1);
        animator.SetBool("Show", true);
        animator.Play("Shown", 0, currentTime);
    }

    public void HideToolTip()
    {
        currentTime = 1 - Mathf.Clamp((animator.GetCurrentAnimatorStateInfo(0).normalizedTime), -1, 1);
        animator.SetBool("Show", false);
        animator.Play("Hidden", 0, currentTime);
    }
}
