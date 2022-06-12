using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputInterpreter
{
    private const int BufferCapacity = 15;

    private readonly List<InputData> buffer;
    private readonly List<ActionData> actions;

    public InputInterpreter(List<ActionData> actions)
    {
        this.actions = actions;

        buffer = new List<InputData>(BufferCapacity);
        for (int i = 0; i < BufferCapacity; i++)
        {
            InputData inputData = new InputData();
            inputData.direction = 5; inputData.aPressed = false; inputData.bPressed = false; inputData.jumpPressed = false;
        }
    }

    public List<ActionData> UpdateBuffer(InputData currentInput)
    {
        if (buffer.Count >= BufferCapacity)
        {
            buffer.RemoveAt(0);
        }
        buffer.Add(currentInput);

        List<ActionData> foundActions = new List<ActionData>();
        foreach (ActionData action in actions)
        {
            if (BufferContainsAction(action))
            {
                foundActions.Add(action);
            }
        }
        return foundActions;
    }

    public bool BufferContainsAction(ActionData action)
    {
        foreach (MotionData motion in action.validMotions)
        {
            if (BufferContainsMotion(motion, action.bufferLimit))
                return true;
        }
        return false;
    }

    private bool BufferContainsMotion(MotionData motion, int bufferLimit)
    {
        int bufferIndex = buffer.Count - 1;
        int inputsFound = 0;

        while (bufferIndex >= BufferCapacity - bufferLimit && inputsFound != motion.inputs.Length)
        {
            //Debug.Log($"{motion.inputs.Length} {inputsFound}");
            if (buffer[bufferIndex].Code == motion.inputs[motion.inputs.Length - inputsFound - 1])
            {
                inputsFound++;
            }
            bufferIndex--;
            //Debug.Log("Loop exited");
        }

        return inputsFound == motion.inputs.Length;
    }

    private bool InputIsNothing(InputData input)
    {
        return input.direction == 5 && !input.aPressed && !input.bPressed && !input.jumpPressed;
    }
}
