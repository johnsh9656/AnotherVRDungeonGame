using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class ClimbProvider : MonoBehaviour
{
    [SerializeField] Transform leftHand;
    [SerializeField] Transform rightHand;

    public static event Action ClimbActive;
    public static event Action ClimbInactive;

    public CharacterController characterController;
    public InputActionProperty velocityRight;
    public InputActionProperty velocityLeft;

    [SerializeField] private InputActionProperty moveInput;
    [SerializeField] private InputActionProperty hookInput;

    private bool _rightActive = false;
    private bool _leftActive = false;

    private Vector3 leftGrabPos;
    private Vector3 rightGrabPos;

    private void Start()
    {
        XRDirectClimbInteractor.ClimbHandActivated += HandActivated;
        XRDirectClimbInteractor.ClimbHandDeactivated += HandDeactivated;
    }

    private void OnDestroy()
    {
        XRDirectClimbInteractor.ClimbHandActivated -= HandActivated;
        XRDirectClimbInteractor.ClimbHandDeactivated -= HandDeactivated;
    }

    private void HandActivated(string _controllerName)
    {
        if (_controllerName == "LeftHand Controller")
        {
            _leftActive = true;
            _rightActive = false;
            //leftGrabPos = leftHand.position;
        } else
        {
            _leftActive = false;
            _rightActive = true;
            //rightGrabPos = rightHand.position;
        }

        ClimbActive?.Invoke();
    }

    private void HandDeactivated(string _controllerName)
    {
        if (_rightActive && _controllerName == "RightHand Controller")
        {
            _rightActive = false;
            ClimbInactive?.Invoke();

        } else if (_leftActive && _controllerName == "LeftHand Controller")
        {
            _leftActive = false;
            ClimbInactive?.Invoke();
        }
    }

    private void Update()
    {
        if (_rightActive || _leftActive)
        {
            Climb();
        }

        if (moveInput.action.WasPerformedThisFrame())
        {
            if (_rightActive) { rightHand.GetComponent<XRDirectClimbInteractor>().CancelClimb(); }
            else if (_leftActive) { leftHand.GetComponent<XRDirectClimbInteractor>().CancelClimb(); }
        }
    }

    private void Climb()
    {
        Vector3 velocity = _leftActive ? velocityLeft.action.ReadValue<Vector3>() : velocityRight.action.ReadValue<Vector3>();
        //if (_leftActive) { leftHand.position = leftGrabPos; }
        //else { rightHand.position = rightGrabPos; }
        characterController.Move(characterController.transform.rotation * -velocity * Time.deltaTime);
    }
}
