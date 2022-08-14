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
        fighter.ClearHitboxes();
        fighter.ClearHurtboxes();
        fighter.blocking = false;

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
            fighter.ClearHurtboxes();
            foreach (HurtboxData data in fighter.currentAction.hurtboxes)
            {
                if (fighter.currentFrame >= data.startEndFrames.x && fighter.currentFrame <= data.startEndFrames.y)
                {
                    if (data.useBaseCollider)
                    {
                        fighter.standingHurtbox.Center = fighter.transform.position;
                        fighter.GetAvailableHurtbox().Init(Vector2.zero, fighter.standingHurtbox.Extents * 2f);
                    }
                    else
                    {
                        Vector2 offset = new Vector2(data.offset.x * (fighter.IsOnLeftSide ? 1 : -1), data.offset.y);
                        fighter.GetAvailableHurtbox().Init(offset, data.size);
                    }
                }
            }

            // Update hitboxes
            fighter.ClearHitboxes();
            if (!fighter.actionHasHit)
            {
                foreach (HitboxData data in fighter.currentAction.hitboxes)
                {
                    if (fighter.currentFrame >= data.startEndFrames.x && fighter.currentFrame <= data.startEndFrames.y)
                    {
                        Vector2 offset = new Vector2(data.offset.x * (fighter.IsOnLeftSide ? 1 : -1), data.offset.y);
                        fighter.GetAvailableHitbox().Init(offset, data.size);
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
                            fighter.velocity = new Vector2(fighter.IsOnLeftSide ? data.movement.x : -data.movement.x, fighter.velocity.y);

                        if (data.movement.y != 0f || data.setZeroes)
                            fighter.velocity = new Vector2(fighter.velocity.x, data.movement.y);
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

            // Projectiles
            foreach (ProjectileData data in fighter.currentAction.projectiles)
            {
                if (fighter.currentFrame == data.frame)
                {
                    fighter.SpawnProjectile(data);
                }
            }

            // Audio
            if (fighter.currentAction.playWhiffSound && fighter.currentFrame == fighter.currentAction.whiffPlayFrame)
            {
                if (fighter.currentAction.customWhiffSound != null)
                    fighter.audioSource.PlayOneShot(fighter.currentAction.customWhiffSound);
                else
                    FightManager.instance.PlaySound(SoundType.Whiff, fighter.audioSource);
            }

            fighter.velocity += acceleration * 0.0167f;
            fighter.currentFrame += 1;
        }
    }

    public override void OnStateExit()
    {
        fighter.SetModelLayer(fighter.IsOnLeftSide ? NewFighter.FrontLayer : NewFighter.BackLayer);
        if (!fighter.actionHasHit || fighter.currentAction.type != ActionData.Type.Grab)
        {
            fighter.currentAction = null;
            fighter.actionHasHit = false;
        }
    }
}
