using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerAnimations : NetworkBehaviour
{
    [Header("Server controlled")]
    public AnimationState animationState;
    public Animator bodyAnimator;
    public float lockTimer;

    [Server]
    public void ServerChangeAnimation(AnimationState animationState)
    {
        if (this.lockTimer > 0f) return;
        this.animationState = animationState;
        bodyAnimator.SetInteger("State", (int)animationState);
    }

    [Server]
    public void ServerChangeAnimation(AnimationState animationState, float lockTimer)
    {
        if (this.lockTimer > 0f) return; 
        this.animationState = animationState;
        this.lockTimer = lockTimer;
        bodyAnimator.SetInteger("State", (int)animationState);
    }

    private void Update()
    {
        if (!isServer) return;
        if (lockTimer > 0f) lockTimer -= Time.deltaTime;
    }
}
