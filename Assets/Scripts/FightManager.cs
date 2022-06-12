using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightManager : MonoBehaviour
{
    private const float HorizontalOverlapBoxWidth = 0.1f;
    private const float VerticalOverlapBoxHeight = 0.1f;

    [SerializeField] private NewFighter[] fighters = new NewFighter[2];
    [SerializeField] private GameObject hitParticlePrefab;
    [SerializeField] private GameObject blockParticlePrefab;
    private int hitstopTimer;

    private void Awake()
    {
        foreach (NewFighter fighter in fighters)
        {
            fighter.BreakThrow.AddListener(OnBreakThrow);
        }
    }

    private void Update()
    {
        if (hitstopTimer > 0)
        {
            fighters[0].PauseFighter();
            fighters[1].PauseFighter();
            hitstopTimer -= 1;
            if (hitstopTimer == 0)
            {
                fighters[0].UnpauseFighter();
                fighters[1].UnpauseFighter();
            }
        }
        else
        {
            UpdateSides();
            CheckForHits();
            Push();
        }
    }

    private void UpdateSides()
    {
        if (fighters.Length != 2)
            return;

        if (fighters[0].transform.position.x - fighters[0].boxCollider.bounds.extents.x + 0.015f > fighters[1].transform.position.x)
        {
            fighters[0].SwitchSide(false);
            fighters[1].SwitchSide(true);
        }
        else
        {
            fighters[0].SwitchSide(true);
            fighters[1].SwitchSide(false);
        }
    }

    private void OnBreakThrow()
    {
        print("Throw break!");
        
        if (fighters.Length != 2)
            return;

        foreach (NewFighter fighter in fighters)
        {
            if (fighter.beingThrown)
            {
                float offset = fighters[0].boxCollider.bounds.extents.x + fighters[1].boxCollider.bounds.extents.x;
                fighter.controller.Move(Vector3.right * (fighter.IsOnLeftSide ? -offset : offset));
            }

            fighter.controller.Move(Vector3.right * (fighter.IsOnLeftSide ? -1.5f : 1.5f));
            fighter.animator.Play("Base Layer.HitLight", -1, 0f);
            fighter.SwitchState(new Stunned(fighter, 20));
        }
    }

    private void CheckForHits()
    {
        if (fighters.Length != 2)
            return;

        //Check for hits
        bool[] hitThisFrame = new bool[2];
        CollisionBox[] hitCollisionBoxes = new CollisionBox[2];
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                if (i == j)
                    continue;

                foreach (CollisionBox hitbox in fighters[i].currentHitboxes)
                {
                    foreach (CollisionBox hurtbox in fighters[j].currentHurtboxes)
                    {
                        if (hitbox.Overlaps(hurtbox))
                        {
                            hitThisFrame[j] = true;
                            hitCollisionBoxes[i] = hitbox;
                            break;
                        }
                    }

                    if (hitThisFrame[j])
                        break;
                }
            }
        }

        // Apply hit to fighters/trade hits if attacks collide.
        ActionData fighterAAction = fighters[0].currentAction;
        int fighterAHitStunFrames = (fighterAAction != null) ? fighters[0].currentAction.numberOfFrames - fighters[0].currentFrame + fighters[0].currentAction.hitAdv : 0;
        int fighterABlockStunFrames = (fighterAAction != null) ? fighters[0].currentAction.numberOfFrames - fighters[0].currentFrame + fighters[0].currentAction.blockAdv : 0;

        ActionData fighterBAction = fighters[1].currentAction;
        int fighterBHitStunFrames = (fighterBAction != null) ? fighters[1].currentAction.numberOfFrames - fighters[1].currentFrame + 1 + fighters[1].currentAction.hitAdv : 0;
        int fighterBBlockStunFrames = (fighterBAction != null) ? fighters[1].currentAction.numberOfFrames - fighters[1].currentFrame + 1 + fighters[1].currentAction.blockAdv : 0;

        if (hitThisFrame[1] && !hitThisFrame[0] || (hitThisFrame[1] && hitThisFrame[0] && fighterAAction.type > fighterBAction.type))
        {
            fighters[0].actionHasHit = true;
            
            if (fighterAAction.type == ActionData.Type.Light || fighterAAction.type == ActionData.Type.Heavy || fighterAAction.type == ActionData.Type.Special)
            {
                Vector3 side = fighters[1].IsOnLeftSide ? Vector3.left : Vector3.right;
                RaycastHit2D wallHit = Physics2D.Raycast(fighters[1].boxCollider.bounds.center + side * fighters[1].boxCollider.bounds.extents.x * 0.95f, side, fighterAAction.pushback);
                if (wallHit)
                    fighters[0].controller.Move(side * -1f * fighterAAction.pushback);
                else
                    fighters[1].controller.Move(side * fighterAAction.pushback);

                Vector3 particlePos = fighters[1].boxCollider.bounds.center - side * fighters[1].boxCollider.bounds.extents.x;
                particlePos.y = hitCollisionBoxes[0].Center.y;
                if (!fighters[1].blocking)
                    Instantiate(hitParticlePrefab, particlePos, Quaternion.identity);
                else
                    Instantiate(blockParticlePrefab, particlePos, Quaternion.Euler(0f, fighters[1].IsOnLeftSide ? -66f : 66f, 0f));

                fighters[1].GetHit(fighterAAction, fighterAHitStunFrames, fighterABlockStunFrames, fighters[0].currentHitboxes[0]);
                hitstopTimer = 3;
            }
            else if (fighterAAction.type == ActionData.Type.Grab)
            {
                if (fighters[1].currentState is Walking || fighters[1].currentState is Attacking)
                {
                    fighters[0].currentThrow = fighterAAction.throwData;
                    fighters[0].SwitchState(new Throwing(fighters[0]));

                    fighters[1].controller.Move(fighters[0].transform.position - fighters[1].transform.position);
                    fighters[1].beingThrown = true;
                    fighters[1].currentThrow = fighterAAction.throwData;
                    fighters[1].SwitchState(new Throwing(fighters[1]));
                }                
            }
        }
        else if (hitThisFrame[0] && !hitThisFrame[1] || (hitThisFrame[0] && hitThisFrame[1] && fighterBAction.type > fighterAAction.type))
        {
            fighters[1].actionHasHit = true;

            if (fighterBAction.type == ActionData.Type.Light || fighterBAction.type == ActionData.Type.Heavy || fighterBAction.type == ActionData.Type.Special)
            {
                Vector3 side = fighters[0].IsOnLeftSide ? Vector3.left : Vector3.right;
                RaycastHit2D wallHit = Physics2D.Raycast(fighters[0].boxCollider.bounds.center + side * fighters[0].boxCollider.bounds.extents.x * 0.95f, side, fighterBAction.pushback);
                if (wallHit)
                    fighters[1].controller.Move(side * -1f * fighterBAction.pushback);
                else
                    fighters[0].controller.Move(side * fighterBAction.pushback);

                Vector3 particlePos = fighters[0].boxCollider.bounds.center - side * fighters[0].boxCollider.bounds.extents.x;
                particlePos.y = hitCollisionBoxes[1].Center.y;
                if (!fighters[0].blocking)
                    Instantiate(hitParticlePrefab, particlePos, Quaternion.identity);
                else
                    Instantiate(blockParticlePrefab, particlePos, Quaternion.Euler(0f, fighters[0].IsOnLeftSide ? -66f : 66f, 0f));

                fighters[0].GetHit(fighterBAction, fighterBHitStunFrames, fighterBBlockStunFrames, fighters[1].currentHitboxes[0]);
                hitstopTimer = 3;
            }
            else if (fighterBAction.type == ActionData.Type.Grab)
            {
                if (fighters[0].currentState is Walking || fighters[0].currentState is Attacking)
                {
                    fighters[1].currentThrow = fighterBAction.throwData;
                    fighters[1].SwitchState(new Throwing(fighters[1]));

                    fighters[0].controller.Move(fighters[1].transform.position - fighters[0].transform.position);
                    fighters[0].beingThrown = true;
                    fighters[0].currentThrow = fighterBAction.throwData;
                    fighters[0].SwitchState(new Throwing(fighters[0]));
                }
            }
        }
        else if (hitThisFrame[0] && hitThisFrame[1])
        {
            if (fighterAAction.type != ActionData.Type.Grab && fighterBAction.type != ActionData.Type.Grab)
            {
                fighters[0].actionHasHit = true;
                fighters[1].actionHasHit = true;

                fighters[0].controller.Move((fighters[0].IsOnLeftSide ? Vector3.left : Vector3.right) * fighterBAction.pushback);
                fighters[1].controller.Move((fighters[1].IsOnLeftSide ? Vector3.left : Vector3.right) * fighterAAction.pushback);

                Vector3 particlePos = new Vector3((hitCollisionBoxes[0].Center.x + hitCollisionBoxes[1].Center.x) / 2f, (hitCollisionBoxes[0].Center.y + hitCollisionBoxes[1].Center.y) / 2f, 0f);
                Instantiate(hitParticlePrefab, particlePos, Quaternion.identity);

                fighters[0].GetHit(fighterBAction, fighterBHitStunFrames, fighterBBlockStunFrames, null);
                fighters[1].GetHit(fighterAAction, fighterAHitStunFrames, fighterABlockStunFrames, null);
                hitstopTimer = 3;
            }
            else
            {
                OnBreakThrow();
            }
        }
    }

    private void Push()
    {
        if (fighters.Length != 2)
            return;

        for (int i = 0; i < 2; i++)
        {
            // Horizontal Collision Checking
            Vector2 hBoxCenter = fighters[i].boxCollider.bounds.center;
            hBoxCenter.x += (fighters[i].boxCollider.bounds.extents.x + HorizontalOverlapBoxWidth / 2f) * (fighters[i].IsOnLeftSide ? 1 : -1);

            Collider2D[] horizontalHits = Physics2D.OverlapBoxAll(hBoxCenter, new Vector2(HorizontalOverlapBoxWidth, fighters[i].boxCollider.bounds.extents.y * 2f), 0);
            foreach (Collider2D col in horizontalHits)
            {
                if (col.transform == fighters[1 - i].transform && Mathf.Sign(fighters[1 - i].boxCollider.bounds.center.x - fighters[i].boxCollider.bounds.center.x) != Mathf.Sign(fighters[1 - i].velocity.x))
                {
                    fighters[i].controller.Move(fighters[1 - i].velocity.x * (1f / 60f) * Vector3.right);
                }
            }

            // Vertical Collision Checking
            if (fighters[i].velocity.y < 0f)
            {
                Vector2 vBoxCenter = fighters[i].boxCollider.bounds.center;
                vBoxCenter.y -= fighters[i].boxCollider.bounds.extents.y + VerticalOverlapBoxHeight / 2f;
                Collider2D[] overlappingColliders = Physics2D.OverlapBoxAll(vBoxCenter, new Vector2(fighters[i].boxCollider.bounds.extents.x * 2.1f, VerticalOverlapBoxHeight), 0);
                foreach (Collider2D col in overlappingColliders)
                {
                    if (col.transform == fighters[1 - i].transform)
                    {
                        if (fighters[i].boxCollider.bounds.center.x >= fighters[1 - i].boxCollider.bounds.center.x)
                        {
                            float overlap = (fighters[1 - i].boxCollider.bounds.center.x + fighters[1 - i].boxCollider.bounds.extents.x) - (fighters[i].boxCollider.bounds.center.x - fighters[i].boxCollider.bounds.extents.x);
                            if (overlap != 0)
                            {
                                RaycastHit2D wallHit = Physics2D.Raycast(fighters[1 - i].boxCollider.bounds.center + Vector3.left * fighters[1 - i].boxCollider.bounds.extents.x * 0.9f, Vector2.left, overlap * 1.5f);
                                if (wallHit)
                                {
                                    fighters[i].controller.Move(1.5f * overlap * Vector3.right);
                                }
                                else
                                {
                                    fighters[1 - i].controller.Move(1.5f * overlap * Vector3.left);
                                }
                            }

                            if (fighters[i].velocity.x < 0f)
                                fighters[i].velocity.x = 0f;
                        }
                        else
                        {
                            float overlap = (fighters[i].boxCollider.bounds.center.x + fighters[i].boxCollider.bounds.extents.x) - (fighters[1 - i].boxCollider.bounds.center.x - fighters[1 - i].boxCollider.bounds.extents.x);
                            if (overlap != 0)
                            {
                                RaycastHit2D wallHit = Physics2D.Raycast(fighters[1 - i].boxCollider.bounds.center + Vector3.right * fighters[1 - i].boxCollider.bounds.extents.x * 0.9f, Vector2.right, overlap * 1.5f);
                                if (wallHit)
                                {
                                    fighters[i].controller.Move(1.5f * overlap * Vector3.left);
                                }
                                else
                                {
                                    fighters[1 - i].controller.Move(1.5f * overlap * Vector3.right);
                                }
                            }

                            if (fighters[i].velocity.x > 0f)
                                fighters[i].velocity.x = 0f;
                        }
                    }
                }
            }
        }

        UpdateSides();
    }
}
