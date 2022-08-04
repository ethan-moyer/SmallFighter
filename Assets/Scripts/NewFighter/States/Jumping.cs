using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumping : FighterState
{
    private const int JumpStartup = 3;
    private const int JumpRecovery = 2;

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
        fighter.ClearHitboxes();
        fighter.ClearHurtboxes();
        fighter.currentHurtboxes[0].Init(Vector2.zero, fighter.standingHurtbox.Extents * 2f);
        fighter.blocking = false;
        fighter.animator.Play("Base Layer.Jump", -1, 0f);
    }

    public override void Update(InputData currentInput)
    {
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
            fighter.velocity = new Vector2(fighter.velocity.x, fighter.velocity.y - fighter.gravity * 0.0167f);
            if (fighter.velocity.y <= 0f && fighter.onGround)
            {
                fighter.velocity = new Vector2(0f, fighter.velocity.y);
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
