using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stunned : FighterState
{
    private int stunFrames;
    private bool startsInAir;

    public Stunned(NewFighter fighter, int stunFrames) : base(fighter)
    {
        this.stunFrames = stunFrames;
    }

    public override void OnStateEnter()
    {
        fighter.velocity = Vector3.zero;
        fighter.currentAction = null;
        fighter.ClearHitboxes();
        fighter.ClearHurtboxes();
        fighter.currentHurtboxes[0].Init(Vector2.zero, fighter.standingHurtbox.Extents * 2f);
        fighter.SetModelLayer(NewFighter.BackLayer);
        startsInAir = !fighter.onGround;
    }

    public override void Update(InputData currentInput)
    {
        if (stunFrames <= 0)
        {
            fighter.SwitchState(new Walking(fighter));
        }
        else
        {            
            if (fighter.velocity.y > 0f || !fighter.onGround)
            {
                fighter.velocity = new Vector2(fighter.velocity.x, fighter.velocity.y - fighter.gravity * 0.0167f);
            }

            if (fighter.onGround)
            {               
                if (fighter.shouldKnockdown)
                {
                    fighter.animator.Play("Base Layer.Rising", -1, 0f);
                    fighter.SwitchState(new Knockdown(fighter));
                }
                else if (startsInAir)
                {
                    fighter.SwitchState(new Walking(fighter));
                }
            }

            stunFrames -= 1;
        }
    }

    public override void OnStateExit()
    {
        fighter.SetModelLayer(fighter.IsOnLeftSide ? NewFighter.FrontLayer : NewFighter.BackLayer);
    }
}
