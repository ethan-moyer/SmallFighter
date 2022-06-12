using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumping : FighterState
{
    private const int JumpStartup = 3;
    private const int JumpRecovery = 4;

    private Vector3 jumpVelocity;
    private int startupTimer;
    private int recoveryTimer;
    private bool inAir;

    public Jumping(NewFighter fighter, Vector3 jumpVelocity) : base(fighter)
    {
        this.jumpVelocity = jumpVelocity;
    }

    public override void OnStateEnter()
    {
        fighter.currentHitboxes.Clear();
        fighter.currentHurtboxes.Clear();
        fighter.currentHurtboxes.Add(fighter.standingHurtbox);
        fighter.currentHurtboxes[0].Center = fighter.boxCollider.bounds.center;
        fighter.animator.Play("Base Layer.Jump", -1, 0f);
    }

    public override void Update(InputData currentInput)
    {
        fighter.currentHurtboxes[0].Center = fighter.boxCollider.bounds.center;

        if (!inAir)
        {
            if (startupTimer >= JumpStartup)
            {
                fighter.velocity = jumpVelocity;
                inAir = true;
            }
            else
            {
                startupTimer += 1;
            }
        }
        else
        {
            fighter.velocity.y -= fighter.gravity * 0.0167f;
            if (fighter.velocity.y <= 0f && fighter.onGround)
            {
                fighter.velocity.x = 0f;
                if (recoveryTimer >= JumpRecovery)
                {
                    fighter.SwitchState(new Walking(fighter));
                }
                else
                {
                    recoveryTimer += 1;
                }
            }
        }
    }

    public override void OnStateExit()
    {
        
    }    
}
