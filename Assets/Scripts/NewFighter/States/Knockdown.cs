using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockdown : FighterState
{
    private const int KnockdownLength = 65;
    private int knockdownTimer;

    public Knockdown(NewFighter fighter) : base(fighter)
    {
    }

    public override void OnStateEnter()
    {
        fighter.ClearHitboxes();
        fighter.ClearHurtboxes();
        fighter.velocity = Vector3.zero;
        fighter.shouldKnockdown = false;
        fighter.model.layer = NewFighter.BackLayer;
    }

    public override void Update(InputData currentInput)
    {
        knockdownTimer += 1;

        if (currentInput.direction == 1 || currentInput.direction == 4)
            fighter.blocking = true;
        else
            fighter.blocking = false;

        if (knockdownTimer >= KnockdownLength)
        {
            fighter.SwitchState(new Walking(fighter));
        }
    }

    public override void OnStateExit()
    {
        fighter.SetModelLayer(fighter.IsOnLeftSide ? NewFighter.FrontLayer : NewFighter.BackLayer);
    }
}
