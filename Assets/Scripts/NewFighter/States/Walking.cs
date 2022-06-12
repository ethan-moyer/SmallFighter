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
        fighter.currentHitboxes.Clear();
        fighter.currentHurtboxes.Clear();
        fighter.currentHurtboxes.Add(fighter.standingHurtbox);
        fighter.currentHurtboxes[0].Center = fighter.boxCollider.bounds.center;
    }

    public override void Update(InputData currentInput)
    {
        fighter.currentHurtboxes[0].Center = fighter.boxCollider.bounds.center;
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
                Vector3 jumpVelocity = Vector3.up * fighter.verticalJumpSpeed;
                jumpVelocity.x = fighter.IsOnLeftSide ? -fighter.horizontalJumpSpeed : fighter.horizontalJumpSpeed;
                fighter.SwitchState(new Jumping(fighter, jumpVelocity));
                break;
            }
            case 9:
            {
                Vector3 jumpVelocity = Vector3.up * fighter.verticalJumpSpeed;
                jumpVelocity.x = fighter.IsOnLeftSide ? fighter.horizontalJumpSpeed : -fighter.horizontalJumpSpeed;
                fighter.SwitchState(new Jumping(fighter, jumpVelocity));
                break;
            }
            case 8:
            {
                Vector3 jumpVelocity = Vector3.up * fighter.verticalJumpSpeed;
                fighter.SwitchState(new Jumping(fighter, jumpVelocity));
                break;
            }
        }
    }
}
