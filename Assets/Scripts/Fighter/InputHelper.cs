using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

class InputHelper
{
    public static InputData GetInputData(PlayerInput playerInput, bool isOnLeftSide)
    {
        InputData data = new InputData();
        Vector2 stickDir = playerInput.actions["Stick"].ReadValue<Vector2>();

        switch (stickDir.x)
        {
            case -1:
                switch (stickDir.y)
                {
                    case -1:
                        // Down Back or Down Forward.
                        data.direction = (isOnLeftSide) ? 1 : 3;
                        break;
                    case 0:
                        // Back or Forward.
                        data.direction = (isOnLeftSide) ? 4 : 6;
                        break;
                    case 1:
                        // Up Back or Up Forward.
                        data.direction = (isOnLeftSide) ? 7 : 9;
                        break;
                }
                break;
            case 0:
                switch (stickDir.y)
                {
                    case -1:
                        // Down.
                        data.direction = 2;
                        break;
                    case 0:
                        // Neutral.
                        data.direction = 5;
                        break;
                    case 1:
                        // Up.
                        data.direction = 8;
                        break;
                }
                break;
            case 1:
                switch (stickDir.y)
                {
                    case -1:
                        // Down Forward or Down Back
                        data.direction = (isOnLeftSide) ? 3 : 1;
                        break;
                    case 0:
                        //Forward or Back.
                        data.direction = (isOnLeftSide) ? 6 : 4;
                        break;
                    case 1:
                        // Up Forward or Up Back
                        data.direction = (isOnLeftSide) ? 9 : 7;
                        break;
                }
                break;
        }

        data.aPressed = playerInput.actions["A"].WasPressedThisFrame();
        data.bPressed = playerInput.actions["B"].WasPressedThisFrame();
        data.cPressed = playerInput.actions["C"].WasPressedThisFrame();
        data.jumpPressed = playerInput.actions["Jump"].WasPressedThisFrame();
        return data;
    }
}
