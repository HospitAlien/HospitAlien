using UnityEngine;

public class MoveManager : MonoBehaviour
{
    private GlobalVariableManager gvm;
    public GameObject LeftLocomotionControllerInteractorGroup;
    public GameObject RightTeleportControllerInteractor;
    public Transform CameraRig;
    public Transform CenterEyeAnchor;
    public float moveSpeed = 2.0f;

    private GameSettingsIO.moveMode currentMovementMode;

    void Start()
    {
        gvm = FindFirstObjectByType<GlobalVariableManager>();
        // Listen the movement mode change event
        gvm.gameSettings.OnSettingChanged += ChangeMovementStatus;
    }

    void Update()
    {
        // Teleport is managed by META SDK
        if (currentMovementMode == GameSettingsIO.moveMode.teleport) return;

        Vector3 forward = CenterEyeAnchor.forward;
        forward.y = 0;
        forward.Normalize();
        Vector3 right = CenterEyeAnchor.right;
        right.y = 0;
        right.Normalize();
        if (currentMovementMode == GameSettingsIO.moveMode.walk)
        {
            Vector2 primaryThumbstick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
            MoveCameraRigWithThumbStick(primaryThumbstick, forward, right);
        }
        else if (currentMovementMode == GameSettingsIO.moveMode.both)
        {
            // Use controller joystick to move if index trigger or hand trigger is pressed
            if (IsTriggerPressed(Controller.Left, trigger.Index) || IsTriggerPressed(Controller.Left, trigger.Hand))
            {
                Vector2 primaryThumbstick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
                MoveCameraRigWithThumbStick(primaryThumbstick, forward, right);
            }
            if (IsTriggerPressed(Controller.Right, trigger.Index) || IsTriggerPressed(Controller.Right, trigger.Hand))
            {
                Vector2 secondaryThumbstick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
                MoveCameraRigWithThumbStick(secondaryThumbstick, forward, right);
            }
        }
    }

    private void MoveCameraRigWithThumbStick(Vector2 thumbStick, Vector3 forward, Vector3 right)
    {
        Vector3 moveDirection = right * thumbStick.x + forward * thumbStick.y;
        CameraRig.transform.position += moveSpeed * Time.deltaTime * moveDirection;
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
                return OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > 0.5f;
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
                return OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.5f;
            }
            else if (trigger == trigger.Hand)
            {
                return OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) > 0.5f;
            }
        }
        return false;
    }

    public void ChangeMovementStatus(GameSettingsIO settings)
    {
        if (settings.MoveModeOption == GameSettingsIO.moveMode.walk)
        {
            LeftLocomotionControllerInteractorGroup.SetActive(false);
            RightTeleportControllerInteractor.SetActive(false);
        }
        else
        {
            LeftLocomotionControllerInteractorGroup.SetActive(true);
            RightTeleportControllerInteractor.SetActive(true);
        }
        currentMovementMode = settings.MoveModeOption;
    }

    void OnDestroy()
    {
        // Unsubscribe the event
        gvm.gameSettings.OnSettingChanged -= ChangeMovementStatus;
    }
}