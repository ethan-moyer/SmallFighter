using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoadAttribute]
public class ActionViewerWindow : EditorWindow
{
    private ActionData actionData;
    private NewFighter fighter;
    private Vector3 fighterStartingPos;
    private int currentFrame = 1;

    private void Awake()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        SceneView.duringSceneGui += OnDuringSceneGui;
    }

    [MenuItem("Window/Action Viewer")]
    public static void ShowWindow()
    {
        GetWindow<ActionViewerWindow>("Action Viewer");
    }

    private void OnGUI()
    {        
        if (!EditorApplication.isPlaying)
        {
            actionData = EditorGUILayout.ObjectField("", actionData, typeof(ActionData), true) as ActionData;
            NewFighter selectedFighter = EditorGUILayout.ObjectField(fighter, typeof(NewFighter), true) as NewFighter;

            if (fighter != selectedFighter)
            {
                if (fighter == null)
                    fighterStartingPos = selectedFighter.transform.position;
                else
                {
                    fighter.transform.position = fighterStartingPos;
                    if (selectedFighter != null)
                        fighterStartingPos = selectedFighter.transform.position;
                }
            }
            fighter = selectedFighter;

            if (fighter != null && actionData != null)
            {
                currentFrame = EditorGUILayout.IntSlider("Current Frame", currentFrame, 1, actionData.numberOfFrames);
                
                Vector2 velocity = Vector2.zero;
                Vector2 acceleration = Vector2.zero;
                int lastMovementDataFrame;
                fighter.transform.position = fighterStartingPos;
                for (int i = 1; i <= currentFrame; i++)
                {
                    foreach (MovementData data in actionData.movements)
                    {
                        if (data.frame == i)
                        {
                            if (data.movementType == MovementData.MovementType.Velocity)
                            {
                                if (data.movement.x != 0f || data.setZeroes)
                                    velocity.x = fighter.IsOnLeftSide ? data.movement.x : -data.movement.x;

                                if (data.movement.y != 0f || data.setZeroes)
                                    velocity.y = data.movement.y;
                            }
                            else
                            {
                                if (data.movement.x != 0f || data.setZeroes)
                                    acceleration.x = fighter.IsOnLeftSide ? data.movement.x : -data.movement.x;

                                if (data.movement.y != 0f || data.setZeroes)
                                    acceleration.y = data.movement.y;
                            }
                            lastMovementDataFrame = data.frame;
                        }
                    }

                    velocity += acceleration * 0.0167f;
                    fighter.transform.position += (Vector3)(velocity * 0.0167f);
                }

                fighter.GetComponentInChildren<Animator>().speed = 0f;
                fighter.GetComponentInChildren<Animator>().Play($"Base Layer.{actionData.animationName}", -1, (currentFrame - 1) / (actionData.numberOfFrames - 1));
            }
        }
        else
        {
            GUILayout.Label("The action viewer cannot be used while in play mode.", EditorStyles.boldLabel);
        }
    }

    private void OnDuringSceneGui(SceneView sceneView)
    {
        if (fighter != null && actionData != null)
        {
            int side = fighter.IsOnLeftSide ? 1 : -1;

            Handles.color = Color.green;
            foreach (HurtboxData hurtbox in actionData.hurtboxes)
            {
                if (hurtbox.startEndFrames.x <= currentFrame && hurtbox.startEndFrames.y >= currentFrame)
                {
                    if (hurtbox.useBaseCollider)
                    {
                        Vector2 offset = fighter.standingHurtbox.Center;
                        offset.x *= side;
                        Handles.DrawWireCube((Vector2)fighter.transform.position + offset, fighter.standingHurtbox.Extents * 2f);
                    }
                    else
                    {
                        Vector2 offset = hurtbox.offset;
                        offset.x *= side;
                        Handles.DrawWireCube((Vector2)fighter.transform.position + offset, hurtbox.size);
                    }
                }
            }

            Handles.color = Color.red;
            foreach (HitboxData hitbox in actionData.hitboxes)
            {
                if (hitbox.startEndFrames.x <= currentFrame && hitbox.startEndFrames.y >= currentFrame)
                {
                    Vector2 offset = hitbox.offset;
                    offset.x *= side;
                    Handles.DrawWireCube((Vector2)fighter.transform.position + offset, hitbox.size);
                }
            }
        }
        HandleUtility.Repaint();
    }

    private void ResetFighter()
    {
        if (fighter != null)
        {
            fighter.transform.position = fighterStartingPos;
            fighter.GetComponentInChildren<Animator>().speed = 1f;
            fighter = null;
        }
        actionData = null;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            ResetFighter();
            SceneView.duringSceneGui -= OnDuringSceneGui;
        }
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            SceneView.duringSceneGui += OnDuringSceneGui;
        }
    }

    private void OnDestroy()
    {
        ResetFighter();
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        SceneView.duringSceneGui -= OnDuringSceneGui;
    }
}
