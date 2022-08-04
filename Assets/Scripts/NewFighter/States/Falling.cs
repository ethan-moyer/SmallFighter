using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Falling : FighterState
{
    private const int FallingRecovery = 2;
    private int recoveryTimer;
    private bool landed;

    public Falling(NewFighter fighter) : base(fighter)
    {
    }

    public override void OnStateEnter()
    {
        fighter.ClearHitboxes();
        fighter.ClearHurtboxes();
        fighter.currentHurtboxes[0].Init(Vector2.zero, fighter.standingHurtbox.Extents * 2f);
        fighter.blocking = false;
    }

    public override void Update(InputData currentInput)
    {
        if (!landed)
        {
            fighter.velocity = new Vector2(fighter.velocity.x, fighter.velocity.y - fighter.gravity * 0.0167f);
            if (fighter.velocity.y <= 0f && fighter.onGround)
            {
                fighter.velocity = new Vector2(0f, fighter.velocity.y);
                fighter.animator.Play("Base Layer.Landing", -1, 0f);
                landed = true;
            }
        }
        else
        {
            if (recoveryTimer >= FallingRecovery)
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
