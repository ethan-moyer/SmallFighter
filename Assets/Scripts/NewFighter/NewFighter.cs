using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(PlayerInput))]
public class NewFighter : MonoBehaviour
{
    public const int BackLayer = 7;
    public const int FrontLayer = 8;

    [HideInInspector] public UnityEvent BreakThrow;
    [HideInInspector] public bool paused;
    [HideInInspector] public Controller2D controller;
    [HideInInspector] public PlayerInput playerInput;
    [HideInInspector] public BoxCollider2D boxCollider;
    [HideInInspector] public Animator animator;
    public Vector3 velocity;
    public bool onGround;
    public bool shouldKnockdown;
    public ActionData currentAction;
    public bool beingThrown;
    public ThrowData currentThrow;
    public int currentFrame;
    [HideInInspector] public bool actionHasHit;
    [HideInInspector] public List<CollisionBox> currentHitboxes;
    [HideInInspector] public List<CollisionBox> currentHurtboxes;
    [HideInInspector] public bool blocking;

    [Header("Components & GameObjects")]
    public GameObject model;
    [Header("Fighter Attributes")]
    public float forwardWalkSpeed = 5f;
    public float backwardWalkSpeed = 5f;
    public float horizontalJumpSpeed = 8f;
    public float verticalJumpSpeed = 3f;
    public float gravity = 50f;
    public CollisionBox standingHurtbox;
    [field: SerializeField] public bool IsOnLeftSide { get; private set; }
    [Header("Universal Actions")]
    [SerializeField] public ActionData throwBreakAction;
    [Header("Fighter Actions")]
    [SerializeField] private List<ActionData> actions;
    
    private InputInterpreter interpreter;
    public FighterState currentState;
    private InputData currentInput;

    private void Awake()
    {
        BreakThrow = new UnityEvent();
        controller = GetComponent<Controller2D>();
        playerInput = GetComponent<PlayerInput>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = model.GetComponent<Animator>();
        velocity = Vector3.zero;
        currentHitboxes = new List<CollisionBox>();
        currentHurtboxes = new List<CollisionBox>();
        interpreter = new InputInterpreter(actions);

        if (playerInput.defaultControlScheme == "KeyboardWASD" || playerInput.defaultControlScheme == "KeyboardArrows")
        {
            playerInput.SwitchCurrentControlScheme(playerInput.defaultControlScheme, Keyboard.current);
        }

        if (!IsOnLeftSide)
        {
            model.transform.localScale = new Vector3(model.transform.localScale.x, model.transform.localScale.y, -model.transform.localScale.z);
        }

        SwitchState(new Walking(this));
    }

    public void SwitchState(FighterState nextState)
    {
        if (currentState != null)
        {
            currentState.OnStateExit();
        }
        currentState = nextState;
        if (currentState != null)
        {
            gameObject.name = $"Fighter - {currentState.GetType().Name}";
            currentState.OnStateEnter();
            //currentState.Update(currentInput);
        }
    }

    public bool CanBreakThrow()
    {
        return interpreter.BufferContainsAction(throwBreakAction);
    }

    public void SwitchSide(bool nowOnLeftSide)
    {
        if (currentState is Walking && IsOnLeftSide != nowOnLeftSide)
        {
            IsOnLeftSide = nowOnLeftSide;
            model.transform.localScale = new Vector3(model.transform.localScale.x, model.transform.localScale.y, -model.transform.localScale.z);
            SetModelLayer(nowOnLeftSide ? FrontLayer : BackLayer);
        }
    }

    public void SetModelLayer(int layer)
    {
        model.layer = layer;
        foreach (Transform child in model.transform)
        {
            child.gameObject.layer = layer;
        }
    }

    public void PauseFighter()
    {
        paused = true;
        animator.enabled = false;
    }

    public void UnpauseFighter()
    {
        paused = false;
        animator.enabled = true;
    }

    private void Update()
    {
        currentInput = InputHelper.GetInputData(playerInput, IsOnLeftSide);
        if (!paused)
        {
            onGround = Physics2D.Raycast(boxCollider.bounds.center, Vector2.down, boxCollider.bounds.extents.y + 0.015f, LayerMask.GetMask("Default"));
            SwitchAction(interpreter.UpdateBuffer(currentInput));
            currentState.Update(currentInput);
            controller.Move(velocity * 0.0167f);
        }        
    }

    private void SwitchAction(List<ActionData> potentialActions)
    {
        if (potentialActions == null || potentialActions.Count == 0)
            return;

        foreach (ActionData nextAction in potentialActions)
        {
            if ((currentState is Walking && !nextAction.airOkay) || (currentState is Jumping && velocity.y <= 10f && nextAction.airOkay))
            {
                if (currentAction == null || currentAction.alwaysCancelable)
                {
                    currentAction = nextAction;
                    SwitchState(new Attacking(this));
                    return;
                }
            }
            else if (currentState is Attacking && currentAction != null)
            {
                foreach (CancelsData data in currentAction.cancels)
                {
                    if (currentFrame >= data.startEndFrames.x && currentFrame <= data.startEndFrames.y && Array.IndexOf(data.actions, nextAction.actionName) != -1)
                    {
                        print("Switching");
                        currentAction = nextAction;
                        SwitchState(new Attacking(this));
                        return;
                    }
                }
            }
        }        
    }

    public void GetHit(ActionData action, int hitStunFrames, int blockStunFrames, CollisionBox hitbox)
    {
        if (!blocking)
        {
            if (onGround)
            {
                if (action.knockdown)
                {
                    animator.Play("Base Layer.Knockdown", -1, 0f);
                    SwitchState(new Knockdown(this));
                }
                else
                {
                    if (action.hitAnim == ActionData.HitAnim.Light)
                        animator.Play("Base Layer.HitLight", -1, 0f);
                    else
                        animator.Play("Base Layer.HitHeavy", -1, 0f);

                    SwitchState(new Stunned(this, hitStunFrames));
                }
            }
            else
            {
                if (action.knockdown)
                {
                    shouldKnockdown = true;
                    animator.Play("Base Layer.AirKnockdown", -1, 0f);
                }
                else
                {
                    animator.Play("Base Layer.HitAir", -1, 0f);
                }
                SwitchState(new Stunned(this, hitStunFrames));
            }            
        }
        else
        {
            animator.Play("Base Layer.Block", -1, 0f);
            SwitchState(new Stunned(this, blockStunFrames));
        }
    }

    private void OnDrawGizmos()
    {
        if (currentHurtboxes != null)
        {
            if (currentState is Stunned)
                Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
            else
                Gizmos.color = new Color(0f, 1f, 0f, 0.5f);

            foreach (CollisionBox box in currentHurtboxes)
            {
                Gizmos.DrawCube(box.Center, box.Extents * 2f);
            }
        }

        if (currentHitboxes != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            foreach (CollisionBox box in currentHitboxes)
            {
                Gizmos.DrawCube(box.Center, box.Extents * 2f);
            }
        }
    }
}
