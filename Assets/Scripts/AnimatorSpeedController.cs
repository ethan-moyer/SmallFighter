using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorSpeedController : MonoBehaviour
{
    private Animator animator;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();    
    }

    private void Update()
    {
        float currentFrameRate = 1f / Time.deltaTime;
        animator.speed = currentFrameRate / 60f;
    }
}
