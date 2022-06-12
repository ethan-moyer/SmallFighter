using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ThrowData", menuName = "Fighter/ThrowData", order = 2)]
public class ThrowData : ScriptableObject
{
    public string throwName = "Throw Name";
    public int numberOfFramesThrowing;
    public int numberOfFramesThrown;
    public bool canTech = true;
    public string animationName = "Animation Name";
    
    public DamageData[] damageFrames;
    public Vector2 opponentTargetOffset;
    public int tossFrame;
    public float tossSpeed;
}
