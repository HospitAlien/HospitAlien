using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MoveManager : MonoBehaviour
{
    private GlobalVariableManager gvm;
    public GameObject leftLocomotionControllerInteractorGroup;
    public GameObject rightLocomotionControllerInteractorGroup;

    public Transform CameraRig;
    public Transform CenterEyeAnchor;
    public float moveSpeed = 3.0f;
    public float rotationSpeed = 90.0f;

    private GameSettingsIO.moveMode _movementMode;
    private GameSettingsIO.turnMode _turnMode;
    private bool _snapTurned = false;
    public AnimationCurve smoothRotationCurve;
    private bool takeLeftInput = false;
    private bool takeRightInput = false;
    private bool holdingLeftTrigger = false;
    private bool holdingRightTrigger = false;

    void Start()
    {
        gvm = FindFirstObjectByType<GlobalVariableManager>();
        // Listen the movement mode change event
        gvm.gameSettings.OnSettingChanged += ChangeMovementStatus;
        ChangeMovementStatus(gvm.gameSettings);
    }

    void Update()
    {
        // Teleport is managed by META SDK
        if (_movementMode == GameSettingsIO.moveMode.teleport) return;

        Vector3 forward = CenterEyeAnchor.forward;
        forward.y = 0f;
        forward.Normalize();
        Vector3 right = CenterEyeAnchor.right;
        right.y = 0;
        right.Normalize();
        Vector2 primaryThumbstick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        if (Mathf.Abs(primaryThumbstick.x) < 0.1f)
        {
            primaryThumbstick.x = 0f;
        }
        if (Mathf.Abs(primaryThumbstick.y) < 0.1f)
        {
            primaryThumbstick.y = 0f;
        }
        Vector2 secondaryThumbstick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        if (Mathf.Abs(secondaryThumbstick.x) < 0.1f)
        {
            secondaryThumbstick.x = 0f;
        }
        if (Mathf.Abs(secondaryThumbstick.y) < 0.1f)
        {
            secondaryThumbstick.y = 0f;
        }
        if (_movementMode == GameSettingsIO.moveMode.walk)
        {
            MoveCameraRigWithThumbStick(primaryThumbstick, forward, right);
            RotateCameraRigWithThumbStick(secondaryThumbstick.x);
        }
        else if (_movementMode == GameSettingsIO.moveMode.both)
        {
            if (takeLeftInput) MoveCameraRigWithThumbStick(primaryThumbstick, forward, right);
            if (takeRightInput) MoveCameraRigWithThumbStick(secondaryThumbstick, forward, right);

            // if player hold the trigger and not using Thumbstick, take charge of the input system and move the camera rig
            if (IsTriggerPressed(Controller.Left, trigger.Index) || IsTriggerPressed(Controller.Left, trigger.Hand))
            {
                if (primaryThumbstick == Vector2.zero && !holdingLeftTrigger) takeLeftInput = true;
                holdingLeftTrigger = true;
            }
            else { takeLeftInput = false; holdingLeftTrigger = false; }
            if (IsTriggerPressed(Controller.Right, trigger.Index) || IsTriggerPressed(Controller.Right, trigger.Hand))
            {
                if (secondaryThumbstick == Vector2.zero && !holdingRightTrigger) takeRightInput = true;
                holdingRightTrigger = true;
            }
            else { takeRightInput = false; holdingRightTrigger = false; }
        }
    }

    private void MoveCameraRigWithThumbStick(Vector2 thumbStick, Vector3 forward, Vector3 right)
    {
        if (thumbStick == Vector2.zero) return;
        Vector3 moveDirection = right * thumbStick.x + forward * thumbStick.y;
        CameraRig.transform.position += moveSpeed * Time.deltaTime * moveDirection;
    }

    private void RotateCameraRigWithThumbStick(float thumbStickX)
    {
        if (_turnMode == GameSettingsIO.turnMode.Snap)
        {
            if (thumbStickX > 0.5f && !_snapTurned)
            {
                RotateCameraRig(45);
                _snapTurned = true;
            }
            else if (thumbStickX < -0.5f && !_snapTurned)
            {
                RotateCameraRig(-45);
                _snapTurned = true;
            }
            else if (thumbStickX < 0.5f && thumbStickX > -0.5f)
            {
                _snapTurned = false;
            }
        }
        else if (_turnMode == GameSettingsIO.turnMode.Smooth)
        {
            float smoothInput = smoothRotationCurve.Evaluate(Mathf.Abs(thumbStickX)) * Mathf.Sign(thumbStickX);
            float targetRotationAngle = smoothInput * rotationSpeed * Time.deltaTime;
            RotateCameraRig(targetRotationAngle);
        }
    }

    private void RotateCameraRig(float rotationAngle)
    {
        if (rotationAngle != 0f)
        {
            Vector3 centerEyePositionBeforeRotation = CenterEyeAnchor.transform.position;
            CameraRig.transform.Rotate(0, rotationAngle, 0);
            Vector3 centerEyePositionAfterRotation = CenterEyeAnchor.transform.position;
            Vector3 translation = centerEyePositionBeforeRotation - centerEyePositionAfterRotation;
            CameraRig.transform.position += translation;
        }
    }


    private enum Controller
    {
        Left,
        Right
    }

    private enum trigger
    {
        Index,
        Hand
    };

    // Identify if the trigger is pressed
    private bool IsTriggerPressed(Controller controller, trigger trigger)
    {
        if (controller == Controller.Left)
        {
            if (trigger == trigger.Index)
            {
                return OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > 0.0f;
            }
            else if (trigger == trigger.Hand)
            {
                return OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger) > 0.0f;
            }
        }
        else
        {
            if (trigger == trigger.Index)
            {
                return OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.0f;
            }
            else if (trigger == trigger.Hand)
            {
                return OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) > 0.0f;
            }
        }
        return false;
    }

    public void ChangeMovementStatus(GameSettingsIO settings)
    {
        if (settings.MoveModeOption == GameSettingsIO.moveMode.walk)
        {
            leftLocomotionControllerInteractorGroup.SetActive(false);
            rightLocomotionControllerInteractorGroup.SetActive(false);
        }
        else
        {
            leftLocomotionControllerInteractorGroup.SetActive(true);
            rightLocomotionControllerInteractorGroup.SetActive(true);
        }
        _movementMode = settings.MoveModeOption;
        _turnMode = settings.TurnModeOption;
    }

    void OnDestroy()
    {
        // Unsubscribe the event
        gvm.gameSettings.OnSettingChanged -= ChangeMovementStatus;
    }
}