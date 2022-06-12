using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ActionData", menuName = "Fighter/ActionData", order = 1)]
public class ActionData : ScriptableObject
{
    public string actionName = "Action Name";
    public bool alwaysCancelable;
    public bool airOkay;
    public bool stopOnLanding;
    public int numberOfFrames;
    public string animationName = "Idle";
    [Range(1, 15)] public int bufferLimit = 15;
    [Header("Damage & Stun")]
    public int damage = 0;
    public bool knockdown;
    public Type type;
    public HitAnim hitAnim;
    public int hitAdv = 0;
    public int blockAdv = 0;
    public float pushback = 0f;
    [Header("Throws")]
    public ThrowData throwData;
        
    [Header("Input")]
    public MotionData[] validMotions;

    [Header("Data")]
    public HurtboxData[] hurtboxes;
    public HitboxData[] hitboxes;
    public MovementData[] movements;
    public CancelsData[] cancels;
    
    public enum Type { Movement, Light, Heavy, Grab, Special };

    public enum HitAnim { Light, Heavy };
}


[System.Serializable]
public class MotionData
{
    public string[] inputs;
}

[System.Serializable]
public class HurtboxData
{
    public Vector2Int startEndFrames;
    public bool useBaseCollider;
    public Vector2 offset = Vector2.zero;
    public Vector2 size = Vector2.one;
}

[System.Serializable]
public class HitboxData
{
    public Vector2Int startEndFrames;
    public Vector2 offset = Vector2.zero;
    public Vector2 size = Vector2.one;
}

[System.Serializable]
public class MovementData
{
    public enum MovementType { Velocity, Acceleration };
    public int frame;
    public MovementType movementType;
    public Vector2 movement;
    public bool setZeroes;
}

[System.Serializable]
public class CancelsData
{
    public Vector2Int startEndFrames;
    public bool onHit;
    public bool onBlock;
    public string[] actions;
}

[System.Serializable]
public class DamageData
{
    public int frame;
    public int damage;
}