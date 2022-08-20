using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(PlayerInput))]
public class DeviceLabelController : MonoBehaviour
{
    [HideInInspector] public UnityEvent<DeviceLabelController, float> MoveLabel;
    [HideInInspector] public UnityEvent<DeviceLabelController> PressedEnter;
    [HideInInspector] public UnityEvent<DeviceLabelController> PressedCancel;
    [HideInInspector] public InputDevice device;
    [HideInInspector] public string controlScheme;
    public Vector3 initialPosition;

    public void OnMove(CallbackContext ctx)
    {
        if (ctx.performed)
        {
            float xValue = ctx.ReadValue<Vector2>().x;
            MoveLabel.Invoke(this, xValue);
            if (xValue < -0.1f)
            {
                print($"{gameObject.name} Moved Left");
            }
            else if (xValue > 0.1f)
            {
                print($"{gameObject.name} Moved Right");
            }
        }        
    }

    public void OnEnter(CallbackContext ctx)
    {
        if (ctx.performed)
        {
            PressedEnter.Invoke(this);
        }
    }

    public void OnCancel(CallbackContext ctx)
    {
        if (ctx.performed)
        {
            PressedCancel.Invoke(this);
        }
    }
}
