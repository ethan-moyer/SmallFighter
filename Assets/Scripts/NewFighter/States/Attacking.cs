using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacking : FighterState
{
    private Vector3 acceleration;

    public Attacking(NewFighter fighter) : base(fighter)
    {
    }

    public override void OnStateEnter()
    {
        fighter.SetModelLayer(NewFighter.FrontLayer);
        fighter.currentFrame = 1;
        fighter.actionHasHit = false;
        fighter.currentHitboxes.Clear();
        fighter.currentHurtboxes.Clear();

        if (fighter.currentAction != null)
            fighter.animator.Play($"Base Layer.{fighter.currentAction.animationName}", -1, 0f);
    }

    public override void Update(InputData currentInput)
    {        
        if (fighter.currentAction == null || fighter.currentFrame >= fighter.currentAction.numberOfFrames)
        {
            if (!fighter.onGround)
            {
                fighter.SwitchState(new Falling(fighter));
            }
            else
            {
                fighter.SwitchState(new Walking(fighter));
            }
        }
        else if (fighter.currentAction != null && fighter.currentAction.airOkay && fighter.currentAction.stopOnLanding && fighter.onGround)
        {
            fighter.SwitchState(new Falling(fighter));
        }
        else
        {
            // Update hurtboxes
            fighter.currentHurtboxes.Clear();
            foreach (HurtboxData data in fighter.currentAction.hurtboxes)
            {
                if (fighter.currentFrame >= data.startEndFrames.x && fighter.currentFrame <= data.startEndFrames.y)
                {
                    if (data.useBaseCollider)
                    {
                        fighter.standingHurtbox.Center = fighter.transform.position;
                        fighter.currentHurtboxes.Add(fighter.standingHurtbox);
                    }
                    else
                    {
                        Vector2 offset = new Vector2(data.offset.x * (fighter.IsOnLeftSide ? 1 : -1), data.offset.y);
                        CollisionBox hurtbox = new CollisionBox((Vector2)fighter.transform.position + offset, data.size);
                        fighter.currentHurtboxes.Add(hurtbox);
                    }
                }
            }

            // Update hitboxes
            fighter.currentHitboxes.Clear();
            if (!fighter.actionHasHit)
            {
                foreach (HitboxData data in fighter.currentAction.hitboxes)
                {
                    if (fighter.currentFrame >= data.startEndFrames.x && fighter.currentFrame <= data.startEndFrames.y)
                    {
                        Vector2 offset = new Vector2(data.offset.x * (fighter.IsOnLeftSide ? 1 : -1), data.offset.y);
                        CollisionBox hitbox = new CollisionBox((Vector2)fighter.transform.position + offset, data.size);
                        fighter.currentHitboxes.Add(hitbox);

                        /*if (HitboxTouchingOpponent(hitbox))
                        {
                            fighter.actionHasHit = true;
                            int hitStunFrames = fighter.currentAction.numberOfFrames - fighter.currentFrame + 1 + fighter.currentAction.hitAdv;
                            int blockStunFrames = fighter.currentAction.numberOfFrames - fighter.currentFrame + 1 + fighter.currentAction.blockAdv;
                            fighter.opponent.GetHit(fighter.currentAction, hitStunFrames, blockStunFrames, hitbox);
                        }*/
                    }
                }
            }

            // Movements
            foreach (MovementData data in fighter.currentAction.movements)
            {
                if (fighter.currentFrame == data.frame)
                {
                    if (data.movementType == MovementData.MovementType.Velocity)
                    {
                        if (data.movement.x != 0f || data.setZeroes)
                            fighter.velocity.x = fighter.IsOnLeftSide ? data.movement.x : -data.movement.x;

                        if (data.movement.y != 0f || data.setZeroes)
                            fighter.velocity.y = data.movement.y;
                    }
                    else
                    {
                        if (data.movement.x != 0f || data.setZeroes)
                            acceleration.x = fighter.IsOnLeftSide ? data.movement.x : -data.movement.x;

                        if (data.movement.y != 0f || data.setZeroes)
                            acceleration.y = data.movement.y;
                    }
                }
            }

            fighter.velocity += acceleration * 0.0167f;
            fighter.currentFrame += 1;
        }
    }

    public override void OnStateExit()
    {
        fighter.SetModelLayer(fighter.IsOnLeftSide ? NewFighter.FrontLayer : NewFighter.BackLayer);
        //fighter.velocity = Vector3.zero;
        fighter.currentAction = null;
    }
}
