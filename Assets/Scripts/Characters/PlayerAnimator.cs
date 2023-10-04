using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private int walkingId = 0;

    private void Start()
    {
        walkingId = Animator.StringToHash("Walking");
    }

    public void Walking(bool enable) 
    {
        animator.SetBool(walkingId, enable);
    }
}
