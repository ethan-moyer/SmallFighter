using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stunned : FighterState
{
    private int stunFrames;

    public Stunned(NewFighter fighter, int stunFrames) : base(fighter)
    {
        this.stunFrames = stunFrames;
    }

    public override void OnStateEnter()
    {
        fighter.velocity = Vector3.zero;
        fighter.currentHitboxes.Clear();
        fighter.currentHurtboxes.Clear();
        fighter.currentHurtboxes.Add(fighter.standingHurtbox);
        fighter.currentHurtboxes[0].Center = fighter.boxCollider.bounds.center;
        fighter.SetModelLayer(NewFighter.BackLayer);
    }

    public override void Update(InputData currentInput)
    {
        if (stunFrames <= 0)
        {
            fighter.SwitchState(new Walking(fighter));
        }
        else
        {
            fighter.currentHurtboxes[0].Center = fighter.boxCollider.bounds.center;
            
            if (fighter.velocity.y > 0f || !fighter.onGround)
            {
                fighter.velocity.y -= fighter.gravity * 0.0167f;
            }

            if (fighter.onGround && fighter.shouldKnockdown)
            {
                fighter.animator.Play("Base Layer.Rising", -1, 0f);
                fighter.SwitchState(new Knockdown(fighter));
            }

            stunFrames -= 1;
        }
    }

    public override void OnStateExit()
    {
        fighter.SetModelLayer(fighter.IsOnLeftSide ? NewFighter.FrontLayer : NewFighter.BackLayer);
    }
}
