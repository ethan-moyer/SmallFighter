using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walking : FighterState
{
    public Walking(NewFighter fighter) : base(fighter)
    {
    }

    public override void OnStateEnter()
    {
        fighter.currentAction = null;
        fighter.actionHasHit = false;
        fighter.currentThrow = null;
        fighter.ClearHitboxes();
        fighter.ClearHurtboxes();
        fighter.currentHurtboxes[0].Init(Vector2.zero, fighter.standingHurtbox.Extents * 2f);
    }

    public override void Update(InputData currentInput)
    {
        fighter.blocking = false;
        fighter.velocity = Vector3.zero;
        
        switch (currentInput.direction)
        {
            case 2:
            case 5:
            {
                fighter.animator.Play("Base Layer.Idle");
                break;
            }
            case 3:
            case 6:
            {
                fighter.velocity = fighter.IsOnLeftSide ? (Vector3.right * fighter.forwardWalkSpeed) : (Vector3.left * fighter.forwardWalkSpeed);
                fighter.animator.Play("Base Layer.WalkForward");
                break;
            }
            case 1:
            case 4:
            {
                fighter.blocking = true;
                fighter.velocity = fighter.IsOnLeftSide ? (Vector3.left * fighter.backwardWalkSpeed) : (Vector3.right * fighter.backwardWalkSpeed);
                fighter.animator.Play("Base Layer.WalkBackward");
                break;
            }
            case 7:
            {
                if (currentInput.jumpPressed)
                {
                    Vector3 jumpVelocity = Vector3.up * fighter.verticalJumpSpeed;
                    jumpVelocity.x = fighter.IsOnLeftSide ? -fighter.horizontalJumpSpeed : fighter.horizontalJumpSpeed;
                    fighter.SwitchState(new Jumping(fighter, jumpVelocity));
                }
                else
                {
                    fighter.blocking = true;
                    fighter.velocity = fighter.IsOnLeftSide ? (Vector3.left * fighter.backwardWalkSpeed) : (Vector3.right * fighter.backwardWalkSpeed);
                    fighter.animator.Play("Base Layer.WalkBackward");
                }
                break;
            }
            case 9:
            {
                if (currentInput.jumpPressed)
                {
                    Vector3 jumpVelocity = Vector3.up * fighter.verticalJumpSpeed;
                    jumpVelocity.x = fighter.IsOnLeftSide ? fighter.horizontalJumpSpeed : -fighter.horizontalJumpSpeed;
                    fighter.SwitchState(new Jumping(fighter, jumpVelocity));
                }
                else
                {
                    fighter.velocity = fighter.IsOnLeftSide ? (Vector3.right * fighter.forwardWalkSpeed) : (Vector3.left * fighter.forwardWalkSpeed);
                    fighter.animator.Play("Base Layer.WalkForward");
                }
                break;
            }
            case 8:
            {
                if (currentInput.jumpPressed)
                {
                    Vector3 jumpVelocity = Vector3.up * fighter.verticalJumpSpeed;
                    fighter.SwitchState(new Jumping(fighter, jumpVelocity));
                }
                else
                {
                    fighter.animator.Play("Base Layer.Idle");
                }
                break;
            }
        }
    }

    public override void OnStateExit()
    {
    }
}
