using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerInputManager))]
public class DevicesScreenController : MonoBehaviour
{
    [SerializeField] private GameObject labelPrefab;
    [SerializeField] private Transform labelParent;
    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform rightPoint;

    private List<DeviceLabelController> labelControllers;
    private PlayerInputManager inputManager;
    private AudioSource audioSource;
    private DeviceLabelController leftSide;
    private DeviceLabelController rightSide;

    private void Awake()
    {
        labelControllers = new List<DeviceLabelController>();
        inputManager = GetComponent<PlayerInputManager>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        foreach (InputDevice device in InputSystem.devices)
        {
            if (device.description.deviceClass == "Mouse")
                continue;

            if (device.description.deviceClass == "Keyboard")
            {
                PlayerInput wasd = PlayerInput.Instantiate(labelPrefab, controlScheme: "KeyboardWASD", pairWithDevice: device);
                wasd.SwitchCurrentControlScheme(controlScheme: "KeyboardWASD", devices: device);
                AddNewLabelController(wasd, device, "Keyboard 1");


                PlayerInput arrows = PlayerInput.Instantiate(labelPrefab, controlScheme: "KeyboardArrows", pairWithDevice: device);
                arrows.SwitchCurrentControlScheme(controlScheme: "KeyboardArrows", devices: device);
                AddNewLabelController(arrows, device, "Keyboard 2");
            }
            else
            {
                PlayerInput newLabel = PlayerInput.Instantiate(labelPrefab, controlScheme: "Gamepad", pairWithDevice: device);
                AddNewLabelController(newLabel, device, $"Device {labelControllers.Count + 1}");
            }
        }

        StartCoroutine(DelayedGetPositions());
    }

    private void AddNewLabelController(PlayerInput playerInput, InputDevice device, string labelName)
    {
        DeviceLabelController labelController = playerInput.GetComponent<DeviceLabelController>();
        labelController.device = device;
        labelController.controlScheme = playerInput.currentControlScheme;

        labelController.transform.SetParent(labelParent);
        labelController.transform.localScale = Vector3.one;

        labelController.gameObject.name = labelName;
        labelController.GetComponent<TextMeshProUGUI>().text = labelName;

        labelController.MoveLabel.AddListener(OnMoveLabel);
        labelController.PressedEnter.AddListener(OnPressedEnter);
        labelController.PressedCancel.AddListener(OnPressedCancel);

        labelControllers.Add(labelController);
    }

    private IEnumerator DelayedGetPositions()
    {
        yield return null;
        labelParent.GetComponent<VerticalLayoutGroup>().enabled = false;
        foreach (DeviceLabelController c in labelControllers)
        {
            c.initialPosition = c.transform.position;
        }
    }

    private void OnMoveLabel(DeviceLabelController labelController, float xValue)
    {
        if (xValue < -0.1f)
        {
            if (labelController == rightSide)
            {
                rightSide = null;
                StartCoroutine(LerpLabel(labelController.transform, labelController.initialPosition, .1f));
            }
            else if (leftSide == null)
            {
                leftSide = labelController;
                StartCoroutine(LerpLabel(labelController.transform, leftPoint.position, .1f));
            }
        }
        else if (xValue > 0.1f)
        {
            if (labelController == leftSide)
            {
                leftSide = null;
                StartCoroutine(LerpLabel(labelController.transform, labelController.initialPosition, .1f));
            }
            else if (rightSide == null)
            {
                rightSide = labelController;
                StartCoroutine(LerpLabel(labelController.transform, rightPoint.position, .1f));
            }
        }
    }

    private IEnumerator LerpLabel(Transform labelTransform, Vector3 endPosition, float lerpDuration)
    {
        audioSource.Play();
        Vector3 startPosition = labelTransform.position;
        float elapsedTime = 0f;

        while (elapsedTime < lerpDuration)
        {
            labelTransform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        labelTransform.position = endPosition;
    }

    private void OnPressedEnter(DeviceLabelController labelController)
    {
        if (leftSide == null && rightSide != labelController)
        {
            leftSide = labelController;
            StartCoroutine(LerpLabel(labelController.transform, leftPoint.position, 0.1f));
        }
        else if (rightSide == null && leftSide != labelController)
        {
            rightSide = labelController;
            StartCoroutine(LerpLabel(labelController.transform, rightPoint.position, 0.1f));
        }
        else if (leftSide != null && rightSide != null)
        {
            FightLoader.instance.fighterDevices[0] = leftSide.device;
            FightLoader.instance.fighterDevices[1] = rightSide.device;

            FightLoader.instance.controlSchemes[0] = leftSide.controlScheme;
            FightLoader.instance.controlSchemes[1] = rightSide.controlScheme;

            FightLoader.instance.LoadStage();
        }
    }

    private void OnPressedCancel(DeviceLabelController labelController)
    {
        if (labelController == leftSide)
        {
            leftSide = null;
            StartCoroutine(LerpLabel(labelController.transform, labelController.initialPosition, .1f));
        }
        else if (labelController == rightSide)
        {
            rightSide = null;
            StartCoroutine(LerpLabel(labelController.transform, labelController.initialPosition, .1f));
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }
}
