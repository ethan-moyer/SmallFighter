using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Falling : FighterState
{
    private const int FallingRecovery = 4;
    private int recoveryTimer;
    private bool landed;

    public Falling(NewFighter fighter) : base(fighter)
    {
    }

    public override void OnStateEnter()
    {
        fighter.currentHitboxes.Clear();
        fighter.currentHurtboxes.Clear();
        fighter.currentHurtboxes.Add(fighter.standingHurtbox);
        fighter.currentHurtboxes[0].Center = fighter.boxCollider.bounds.center;
    }

    public override void Update(InputData currentInput)
    {
        fighter.currentHurtboxes[0].Center = fighter.boxCollider.bounds.center;
        if (!landed)
        {
            fighter.velocity.y -= fighter.gravity * 0.0167f;
            if (fighter.velocity.y <= 0f && fighter.onGround)
            {
                fighter.velocity.x = 0;
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
